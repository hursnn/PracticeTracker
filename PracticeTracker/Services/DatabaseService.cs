using PracticeTracker.Models;
using SQLite;

namespace PracticeTracker.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public async Task InitAsync()
    {
        await _database.CreateTableAsync<Student>();
        await _database.CreateTableAsync<PracticeTask>();
    }

    public async Task<List<Student>> GetStudentsAsync()
    {
        return await _database.Table<Student>().ToListAsync();
    }

    public async Task<int> SaveStudentAsync(Student student)
    {
        if (student.Id != 0)
            return await _database.UpdateAsync(student);

        return await _database.InsertAsync(student);
    }

    public async Task<int> DeleteStudentAsync(Student student)
    {
        var tasks = await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == student.Id)
            .ToListAsync();

        foreach (var task in tasks)
        {
            await _database.DeleteAsync(task);
        }

        return await _database.ExecuteAsync(
            "DELETE FROM Student WHERE Id = ?",
            student.Id
        );
    }

    public async Task<List<PracticeTask>> GetTasksByStudentAsync(int studentId)
    {
        return await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<int> SavePracticeTaskAsync(PracticeTask task)
    {
        if (task.Id != 0)
            return await _database.UpdateAsync(task);

        return await _database.InsertAsync(task);
    }

    public async Task<int> GetTotalRepetitionsForDateAsync(int studentId, DateTime date)
    {
        var tasks = await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == studentId)
            .ToListAsync();

        return tasks
            .Where(t => t.Date.Date == date.Date)
            .Sum(t => t.Repetitions);
    }

    public async Task<int> GetTotalRepetitionsForWeekAsync(int studentId)
    {
        DateTime today = DateTime.Today;
        DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);

        var tasks = await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == studentId)
            .ToListAsync();

        return tasks
            .Where(t => t.Date.Date >= weekStart.Date && t.Date.Date <= today.Date)
            .Sum(t => t.Repetitions);
    }

    public async Task<int> GetPracticeDaysThisWeekAsync(int studentId)
    {
        DateTime today = DateTime.Today;
        DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);

        var tasks = await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == studentId)
            .ToListAsync();

        return tasks
            .Where(t => t.Date.Date >= weekStart.Date && t.Date.Date <= today.Date)
            .Select(t => t.Date.Date)
            .Distinct()
            .Count();
    }

    public async Task<string> GetMostPracticedTaskThisWeekAsync(int studentId)
    {
        DateTime today = DateTime.Today;
        DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);

        var tasks = await _database.Table<PracticeTask>()
            .Where(t => t.StudentId == studentId)
            .ToListAsync();

        var result = tasks
            .Where(t => t.Date.Date >= weekStart.Date && t.Date.Date <= today.Date)
            .GroupBy(t => t.TaskName)
            .Select(g => new
            {
                TaskName = g.Key,
                Total = g.Sum(t => t.Repetitions)
            })
            .OrderByDescending(g => g.Total)
            .FirstOrDefault();

        return result == null ? "No practice yet" : result.TaskName;
    }
}