using PracticeTracker.Models;
using PracticeTracker.Services;

namespace PracticeTracker;

public partial class StudentOverviewPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly Student _student;

    public StudentOverviewPage(DatabaseService databaseService, Student student)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _student = student;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        StudentNameLabel.Text = _student.Name;
        await LoadPracticeDataAsync();
    }

    private async Task LoadPracticeDataAsync()
    {
        var tasks = await _databaseService.GetTasksByStudentAsync(_student.Id);

        TasksCollectionView.ItemsSource = tasks;

        int todayTotal = await _databaseService.GetTotalRepetitionsForDateAsync(_student.Id, DateTime.Today);
        int weeklyTotal = await _databaseService.GetTotalRepetitionsForWeekAsync(_student.Id);
        int practiceDays = await _databaseService.GetPracticeDaysThisWeekAsync(_student.Id);
        string mostPracticed = await _databaseService.GetMostPracticedTaskThisWeekAsync(_student.Id);

        TodayTotalLabel.Text = $"Today's total repetitions: {todayTotal}";
        WeeklyTotalLabel.Text = $"This week's total repetitions: {weeklyTotal}";
        PracticeDaysLabel.Text = $"Practice days this week: {practiceDays}/7";
        MostPracticedLabel.Text = $"Most practiced task: {mostPracticed}";
    }

    private async void OnRunPracticeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PracticeRunnerPage(_databaseService, _student));
    }
}