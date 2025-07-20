using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models;

public class Answer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;
}
