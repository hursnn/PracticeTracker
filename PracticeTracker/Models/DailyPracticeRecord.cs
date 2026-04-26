using SQLite;

namespace PracticeTracker.Models;

public class DailyPracticeRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int PracticeTaskId { get; set; }
    public DateTime PracticeDate { get; set; }
    public int CompletedRepetitions { get; set; }
}
