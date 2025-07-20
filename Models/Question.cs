using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models;

public class Question
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    [Required]
    [Range(1, 100)]
    public int Points { get; set; }

    [Required]
    public int QuizId { get; set; }

    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    public virtual List<Answer> Answers { get; set; } = new List<Answer>();
}

