using PracticeTracker.Models;
using PracticeTracker.Services;

namespace PracticeTracker;

public partial class PracticeRunnerPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly Student _student;
    private int _currentRepetitions = 0;

    public PracticeRunnerPage(DatabaseService databaseService, Student student)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _student = student;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        StudentNameLabel.Text = _student.Name;
        await LoadTodayTotalAsync();
        UpdateRepetitionLabel();
    }

    private void OnAddRepetitionClicked(object sender, EventArgs e)
    {
        _currentRepetitions++;
        UpdateRepetitionLabel();
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (_currentRepetitions > 0)
        {
            _currentRepetitions--;
            UpdateRepetitionLabel();
        }
    }

    private void UpdateRepetitionLabel()
    {
        CurrentRepsLabel.Text = $"Current repetitions: {_currentRepetitions}";
    }

    private async Task LoadTodayTotalAsync()
    {
        int total = await _databaseService.GetTotalRepetitionsForDateAsync(_student.Id, DateTime.Today);
        TodayTotalLabel.Text = $"Today's total repetitions: {total}";
    }

    private async void OnSavePracticeClicked(object sender, EventArgs e)
    {
        string taskName = TaskNameEntry.Text?.Trim() ?? string.Empty;
        string targetText = TargetEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(taskName))
        {
            await DisplayAlert("Error", "Please enter a task name.", "OK");
            return;
        }

        if (!int.TryParse(targetText, out int targetRepetitions) || targetRepetitions <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid target repetition number.", "OK");
            return;
        }

        if (_currentRepetitions <= 0)
        {
            await DisplayAlert("Error", "Please add at least one repetition.", "OK");
            return;
        }

        var task = new PracticeTask
        {
            StudentId = _student.Id,
            TaskName = taskName,
            Repetitions = _currentRepetitions,
            Date = DateTime.Now
        };

        await _databaseService.SavePracticeTaskAsync(task);

        if (_currentRepetitions >= targetRepetitions)
        {
            CompleteLabel.Text = "Day complete! Target reached.";
        }
        else
        {
            CompleteLabel.Text = "Saved. Keep practicing to reach your target.";
        }

        _currentRepetitions = 0;
        TaskNameEntry.Text = string.Empty;
        TargetEntry.Text = string.Empty;

        UpdateRepetitionLabel();
        await LoadTodayTotalAsync();

        await DisplayAlert("Saved", "Today's practice was saved.", "OK");
    }
}