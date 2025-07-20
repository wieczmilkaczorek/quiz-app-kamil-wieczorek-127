using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.ViewModels;

namespace QuizApp.Controllers;

[Authorize]
public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateQuizViewModel();
        model.Questions.Add(new QuestionViewModel
        {
            Answers = { new AnswerViewModel(), new AnswerViewModel() }
        });
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQuizViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var currentUser = await _userManager.GetUserAsync(User);


        if (currentUser == null)
        {
            return Challenge();
        }

        var quiz = new Quiz
        {
            Title = model.Title,
            Description = model.Description,
            AuthorId = currentUser.Id
        };

        foreach (var questionViewModel in model.Questions)
        {
            var question = new Question
            {
                Text = questionViewModel.Text,
                Points = questionViewModel.Points
            };

            for (int i = 0; i < questionViewModel.Answers.Count; i++)
            {
                var answerViewModel = questionViewModel.Answers[i];
                var answer = new Answer
                {
                    Text = answerViewModel.Text,
                    IsCorrect = (i == questionViewModel.CorrectAnswerIndex)
                };
                question.Answers.Add(answer);
            }
            quiz.Questions.Add(question);
        }

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyQuizzes));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var quizzes = await _context.Quizzes.Include(q => q.Author).Include(q => q.Questions).ToListAsync();
        return View(quizzes);
    }

    [Authorize]
    public async Task<IActionResult> MyQuizzes()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var userQuizzes = await _context.Quizzes
            .Where(q => q.AuthorId == currentUser.Id)
            .ToListAsync();

        return View(userQuizzes);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Take(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(p => p.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quiz == null)
        {
            return NotFound();
        }

        return View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Submit(int id, Dictionary<int, int> selectedAnswers)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(p => p.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quiz == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return Challenge();
        }

        int score = 0;

        var quizAttempt = new QuizAttempt
        {
            QuizId = quiz.Id,
            UserId = currentUser.Id
        };

        foreach (var question in quiz.Questions)
        {
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

            if (selectedAnswers.TryGetValue(question.Id, out int selectedAnswerId))
            {
                if (correctAnswer != null && correctAnswer.Id == selectedAnswerId)
                {
                    score += question.Points;
                }

                var userAnswer = new UserAnswer
                {
                    QuizAttempt = quizAttempt,
                    QuestionId = question.Id,
                    SelectedAnswerId = selectedAnswerId
                };
                _context.UserAnswers.Add(userAnswer);
            }
        }

        quizAttempt.Score = score;
        _context.QuizAttempts.Add(quizAttempt);
        await _context.SaveChangesAsync();

        return RedirectToAction("Results", new { attemptId = quizAttempt.Id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Results(int attemptId)
    {
        var attempt = await _context.QuizAttempts
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Questions)
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        if (attempt == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (attempt.UserId != currentUserId)
        {
            return Forbid();
        }

        ViewBag.TotalPoints = attempt.Quiz.Questions.Sum(q => q.Points);

        return View(attempt);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id);
        if (quiz == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (quiz.AuthorId != currentUserId) return Forbid();

        return View(quiz);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id);
        if (quiz == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (quiz.AuthorId != currentUserId) return Forbid();

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyQuizzes));
    }
}
