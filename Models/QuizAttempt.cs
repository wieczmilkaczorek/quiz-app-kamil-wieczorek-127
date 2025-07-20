using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models;

public class QuizAttempt
{
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    [Key]
    public int Id { get; set; }

    [Required]
    public int QuizId { get; set; }

    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    public int Score { get; set; }
    public DateTime AttemptedOn { get; set; } = DateTime.UtcNow;
}
