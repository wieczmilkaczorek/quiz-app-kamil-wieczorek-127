using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.ViewModels;

namespace QuizApp.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(p => p.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quiz == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (quiz.AuthorId != currentUserId)
        {
            return Forbid();
        }

        var totalAttempts = await _context.QuizAttempts.CountAsync(a => a.QuizId == id);
        var averageScore = totalAttempts > 0 ? await _context.QuizAttempts.Where(a => a.QuizId == id).AverageAsync(a => a.Score) : 0;

        var answerCounts = await _context.UserAnswers
            .Where(ua => ua.QuizAttempt.QuizId == id)
            .GroupBy(ua => ua.SelectedAnswerId)
            .Select(g => new { AnswerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AnswerId, x => x.Count);

        var viewModel = new StatisticsViewModel
        {
            QuizTitle = quiz.Title,
            TotalAttempts = totalAttempts,
            AverageScore = averageScore
        };

        foreach (var question in quiz.Questions)
        {
            var questionStats = new QuestionStatsViewModel { QuestionText = question.Text };

            foreach (var answer in question.Answers)
            {
                var selectionCount = answerCounts.GetValueOrDefault(answer.Id, 0);
                var answerStats = new AnswerStatsViewModel
                {
                    AnswerText = answer.Text,
                    IsCorrect = answer.IsCorrect,
                    SelectionCount = selectionCount,
                    SelectionPercentage = totalAttempts > 0 ? (double)selectionCount / totalAttempts * 100 : 0
                };
                questionStats.AnswerStats.Add(answerStats);
            }
            viewModel.QuestionStats.Add(questionStats);
        }

        return View(viewModel);
    }
}
