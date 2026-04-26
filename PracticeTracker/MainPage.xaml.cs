using PracticeTracker.Models;
using PracticeTracker.Services;

namespace PracticeTracker;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _databaseService.InitAsync();
        await LoadStudentsAsync();
    }

    private async Task LoadStudentsAsync()
    {
        var students = await _databaseService.GetStudentsAsync();

        StudentsCollectionView.ItemsSource = null;
        StudentsCollectionView.ItemsSource = students;
    }

    private async void OnAddStudentClicked(object sender, EventArgs e)
    {
        string studentName = StudentNameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(studentName))
        {
            await DisplayAlert("Error", "Please enter a student name.", "OK");
            return;
        }

        var student = new Student
        {
            Name = studentName
        };

        await _databaseService.SaveStudentAsync(student);

        StudentNameEntry.Text = string.Empty;

        await LoadStudentsAsync();
    }

    private async void OnOpenStudentClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Student student)
        {
            await Navigation.PushAsync(new StudentOverviewPage(_databaseService, student));
        }
    }

    private async void OnDeleteStudentClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Student student)
        {
            string name = string.IsNullOrWhiteSpace(student.Name) ? "this student" : student.Name;

            bool confirm = await DisplayAlert(
                "Delete Student",
                $"Are you sure you want to delete {name}?",
                "Delete",
                "Cancel");

            if (!confirm)
                return;

            await _databaseService.DeleteStudentAsync(student);
            await LoadStudentsAsync();
        }
    }
}