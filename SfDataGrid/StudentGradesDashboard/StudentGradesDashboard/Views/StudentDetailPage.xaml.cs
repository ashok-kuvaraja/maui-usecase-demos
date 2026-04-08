using StudentGradesDashboard.ViewModels;

namespace StudentGradesDashboard.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentDetailPage : ContentPage
{
	private readonly StudentDetailViewModel _viewModel;
	private int _studentId;

	public int StudentId
	{
		get => _studentId;
		set
		{
			_studentId = value;
			_viewModel.StudentId = value;
		}
	}

	public StudentDetailPage(StudentDetailViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		// Initialize student data when page appears
		if (_viewModel.InitializeCommand.CanExecute(null))
		{
			await _viewModel.InitializeCommand.ExecuteAsync(null);
		}
	}
}
