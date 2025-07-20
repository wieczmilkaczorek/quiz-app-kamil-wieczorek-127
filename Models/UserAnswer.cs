using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models;

public class UserAnswer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuizAttemptId { get; set; }

    [ForeignKey("QuizAttemptId")]
    public virtual QuizAttempt QuizAttempt { get; set; } = null!;

    [Required]
    public int QuestionId { get; set; }

    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    [Required]
    public int SelectedAnswerId { get; set; }

    [ForeignKey("SelectedAnswerId")]
    public virtual Answer SelectedAnswer { get; set; } = null!;
}
