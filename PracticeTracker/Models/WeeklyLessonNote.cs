using SQLite;

namespace PracticeTracker.Models;

public class WeeklyLessonNote
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string WeekTitle { get; set; } = string.Empty;
    public string WeeklyGoal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
}