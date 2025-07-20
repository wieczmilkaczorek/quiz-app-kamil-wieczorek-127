using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizApp.Models;

namespace QuizApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
    public DbSet<UserAnswer> UserAnswers { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Quiz>()
            .HasOne<ApplicationUser>(q => q.Author)
            .WithMany(u => u.Quizzes)
            .HasForeignKey(q => q.AuthorId);

        builder.Entity<UserAnswer>()
            .HasOne(ua => ua.QuizAttempt)
            .WithMany(qa => qa.UserAnswers)
            .HasForeignKey(ua => ua.QuizAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<QuizAttempt>()
            .HasMany<UserAnswer>()
            .WithOne(ua => ua.QuizAttempt)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
