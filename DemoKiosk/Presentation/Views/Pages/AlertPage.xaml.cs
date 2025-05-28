using DemoKiosk.Presentation.ViewModels;

namespace DemoKiosk.Presentation.Views.Pages;

public partial class AlertPage : ContentPage
{
    internal AlertViewModel ViewModel
    {
        get
        {
            if (_viewModel == null)
            {
                _viewModel = BindingContext as AlertViewModel;
            }

            return _viewModel;
        }
    }
	private AlertViewModel _viewModel;

	public AlertPage()
	{
        InitializeComponent();
	}

    private void okBtn_Clicked(object sender, EventArgs e)
    {
		Application.Current?.CloseWindow(this.Window);
    }
}