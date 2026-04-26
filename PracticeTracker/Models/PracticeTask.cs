using SQLite;

namespace PracticeTracker.Models;

public class PracticeTask
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public int Repetitions { get; set; }

    public DateTime Date { get; set; }
}