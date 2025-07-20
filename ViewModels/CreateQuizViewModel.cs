using System.ComponentModel.DataAnnotations;

namespace QuizApp.ViewModels;

public class CreateQuizViewModel
{
    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [StringLength(500)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string Description { get; set; } = null!;

    public List<QuestionViewModel> Questions { get; set; } = new();
}


public class QuestionViewModel
{
    [Required(ErrorMessage = "Treść pytania jest wymagana.")]
    public string Text { get; set; } = null!;

    [Required]
    [Range(1, 100, ErrorMessage = "Odpowiedź musi być pomiędzy 1 a 100.")]
    public int Points { get; set; } = 1;

    public List<AnswerViewModel> Answers { get; set; } = new();

    [Required(ErrorMessage = "Musisz zaznaczyć jedną poprawną odpowiedź.")]
    public int? CorrectAnswerIndex { get; set; }
}

public class AnswerViewModel
{
    [Required(ErrorMessage = "Treść odpowiedzi jest wymagana.")]
    public string Text { get; set; } = null!;

    public bool isCorrect { get; set; }
}

