using Microsoft.AspNetCore.Identity;

namespace QuizApp.Models;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual List<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
