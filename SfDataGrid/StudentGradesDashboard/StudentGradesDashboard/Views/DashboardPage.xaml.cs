using StudentGradesDashboard.Models;
using StudentGradesDashboard.ViewModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Exporting;
using Syncfusion.Maui.Themes;
using System.IO;

namespace StudentGradesDashboard.Views;

public partial class DashboardPage : ContentPage
{
	private readonly DashboardViewModel _viewModel;

	public DashboardPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
        InfoButton.Text = "💡";
        // Initialize theme toggle button text based on Syncfusion theme (preferred), fallback to app theme
        try
        {
            if (ThemeToggleButton != null && Application.Current != null)
            {
                var mergedDictionaries = Application.Current.Resources?.MergedDictionaries;
                var sfTheme = mergedDictionaries?.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
                if (sfTheme != null)
                {
                    // If current visual theme is dark, show sun (indicating pressing will switch to light)
                    ThemeToggleButton.Text = sfTheme.VisualTheme is SfVisuals.MaterialDark ? "☀️" : "🌙";
                }
                else
                {
                    var cur = Application.Current.UserAppTheme;
                    ThemeToggleButton.Text = cur == AppTheme.Dark ? "☀️" : "🌙";
                }
            }
        }
        catch
        {
            // ignore init errors
        }
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		// Initialize data when page appears
		if (_viewModel.InitializeCommand.CanExecute(null))
		{
			await _viewModel.InitializeCommand.ExecuteAsync(null);
		}
	}

    // Row taps no longer trigger navigation; navigation is handled by the
    // ImageButton in the template column so users explicitly tap the arrow.


    // Show popup to choose export format (with entrance animation)
    private async void ShowExportPopup_Clicked(object? sender, EventArgs e)
    {
        ExportPopupOverlay.IsVisible = true;
        dataGrid.Opacity = 0.35;
        dataGrid.IsEnabled = false;

        ExportPopupCard.Scale = 0.90;
        ExportPopupCard.Opacity = 0;
        await ExportPopupCard.ScaleTo(1, 200, Easing.CubicOut);
        await ExportPopupCard.FadeTo(1, 180, Easing.CubicIn);
    }

    // Export to PDF (called from popup)
    private async void ExportPdf_Clicked(object? sender, EventArgs e)
    {
        // Close popup with animation first
        await ExportPopupCard.ScaleTo(0.94, 120, Easing.CubicIn);
        await ExportPopupCard.FadeTo(0, 120, Easing.CubicOut);
        ExportPopupOverlay.IsVisible = false;
        dataGrid.Opacity = 1;
        dataGrid.IsEnabled = true;

        // Perform export
        MemoryStream stream = new MemoryStream();
        DataGridPdfExportingController pdfExport = new DataGridPdfExportingController();
        DataGridPdfExportingOption option = new DataGridPdfExportingOption();
        var pdfDoc = pdfExport.ExportToPdf(this.dataGrid, option);
        pdfDoc.Save(stream);
        pdfDoc.Close(true);
        SaveService saveService = new();
        saveService.SaveAndView("ExportFeature.pdf", "application/pdf", stream);
    }

    // Export to Excel (called from popup)
    private async void ExportExcel_Clicked(object? sender, EventArgs e)
    {
        // Close popup with animation first
        await ExportPopupCard.ScaleTo(0.94, 120, Easing.CubicIn);
        await ExportPopupCard.FadeTo(0, 120, Easing.CubicOut);
        ExportPopupOverlay.IsVisible = false;
        dataGrid.Opacity = 1;
        dataGrid.IsEnabled = true;

        // Perform export
        DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
        DataGridExcelExportingOption option = new DataGridExcelExportingOption();
        var excelEngine = excelExport.ExportToExcel(this.dataGrid, option);
        var workbook = excelEngine.Excel.Workbooks[0];
        MemoryStream stream = new MemoryStream();
        workbook.SaveAs(stream);
        workbook.Close();
        excelEngine.Dispose();
        string OutputFilename = "ExportFeature.xlsx";
        SaveService saveService = new();
        saveService.SaveAndView(OutputFilename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream);
    }

    private async void CancelExportPopup_Clicked(object? sender, EventArgs e)
    {
        await ExportPopupCard.ScaleTo(0.94, 120, Easing.CubicIn);
        await ExportPopupCard.FadeTo(0, 120, Easing.CubicOut);
        ExportPopupOverlay.IsVisible = false;
        dataGrid.Opacity = 1;
        dataGrid.IsEnabled = true;
    }

    // Show a brief information alert when user taps the info icon
    private async void InfoButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "💡 Tap the arrow at the end of a row to view/edit student details", "OK");
    }

    // Toggle app theme between Light and Dark
    private void ThemeToggleButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current == null)
                return;

            var mergedDictionaries = Application.Current.Resources?.MergedDictionaries;
            var sfTheme = mergedDictionaries?.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();

            if (sfTheme != null)
            {
                // Toggle Syncfusion visual theme and keep Application theme in sync
                if (sfTheme.VisualTheme is SfVisuals.MaterialDark)
                {
                    sfTheme.VisualTheme = SfVisuals.MaterialLight;
                    Application.Current.UserAppTheme = AppTheme.Light;
                    if (ThemeToggleButton != null)
                        ThemeToggleButton.Text = "🌙"; // show moon to indicate pressing will go to Dark
                }
                else
                {
                    sfTheme.VisualTheme = SfVisuals.MaterialDark;
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    if (ThemeToggleButton != null)
                        ThemeToggleButton.Text = "☀️"; // show sun to indicate pressing will go to Light
                }
            }
            else
            {
                // No Syncfusion theme found — fallback to toggling the app theme
                var cur = Application.Current.UserAppTheme;
                if (cur == AppTheme.Dark)
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    if (ThemeToggleButton != null)
                        ThemeToggleButton.Text = "🌙";
                }
                else
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    if (ThemeToggleButton != null)
                        ThemeToggleButton.Text = "☀️";
                }
            }
        }
        catch
        {
            // swallow any errors to avoid crashing the UI
        }
    }


    private void ClearFilterButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            // Explicitly clear the entry control text (fallback for controls that don't react to null)
            if (filterText != null)
            {
                filterText.Value = string.Empty;
            }
        }
        catch
        {
            // ignore UI clearing errors
        }

        // Ensure ViewModel clears its filters as well
        try
        {
            if (_viewModel.ClearFilterCommand.CanExecute(null))
            {
                _viewModel.ClearFilterCommand.Execute(null);
            }
        }
        catch
        {
            // ignore
        }
    }



    private async void ImageButton_Clicked(object sender, TappedEventArgs e)
    {
        if (sender is ImageButton btn && btn.BindingContext is Student student)
        {
            if (_viewModel.NavigateToDetailCommand.CanExecute(student))
            {
                await _viewModel.NavigateToDetailCommand.ExecuteAsync(student);
            }
        }
    }

    private async void dataGrid_CellTapped(object sender, DataGridCellTappedEventArgs e)
    {
        try
        {
            // Column index for the "Show Details" template column (last column)
            const int ShowDetailsColumnIndex = 8;

            // Make sure this tap was on the Show Details column
            var tappedColumnIndex = e.RowColumnIndex.ColumnIndex;
            if (tappedColumnIndex != ShowDetailsColumnIndex)
            {
                return;
            }

            // Prefer the RowData if available
            if (e.RowData is Student studentFromRow)
            {
                if (_viewModel.NavigateToDetailCommand.CanExecute(studentFromRow))
                {
                    await _viewModel.NavigateToDetailCommand.ExecuteAsync(studentFromRow);
                }
                return;
            }

            // Fallback: attempt to resolve the item by row index from the ViewModel's collection
            var rowIndex = e.RowColumnIndex.RowIndex;
            if (rowIndex >= 0)
            {
                var collection = _viewModel?.FilteredStudents;
                if (collection != null && rowIndex < collection.Count)
                {
                    var record = collection[rowIndex];
                    if (record != null && _viewModel.NavigateToDetailCommand.CanExecute(record))
                    {
                        await _viewModel.NavigateToDetailCommand.ExecuteAsync(record);
                    }
                }
            }
        }
        catch
        {
            // swallow any unexpected errors to avoid crashing the UI
        }
    }
}
