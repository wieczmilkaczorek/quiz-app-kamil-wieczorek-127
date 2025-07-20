namespace QuizApp.ViewModels;

public class StatisticsViewModel
{
    public string QuizTitle { get; set; } = null!;
    public int TotalAttempts { get; set; }
    public double AverageScore { get; set; }
    public List<QuestionStatsViewModel> QuestionStats { get; set; } = new();
}

public class QuestionStatsViewModel
{
    public string QuestionText { get; set; } = null!;
    public List<AnswerStatsViewModel> AnswerStats { get; set; } = new();
}

public class AnswerStatsViewModel
{
    public string AnswerText { get; set; } = null!;
    public bool IsCorrect { get; set; }
    public int SelectionCount { get; set; }
    public double SelectionPercentage { get; set; }
}
