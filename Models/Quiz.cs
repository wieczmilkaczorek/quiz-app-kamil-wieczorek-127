using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models;

public class Quiz
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string Description { get; set; } = null!;

    [Required]
    public string AuthorId { get; set; } = null!;

    [ForeignKey("AuthorId")]
    public virtual ApplicationUser Author { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual List<Question> Questions { get; set; } = new List<Question>();
}
