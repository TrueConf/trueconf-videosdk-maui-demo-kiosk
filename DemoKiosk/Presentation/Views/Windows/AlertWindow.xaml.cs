using DemoKiosk.Presentation.Views.Pages;

namespace DemoKiosk.Presentation;

public partial class AlertWindow : Window
{
	public AlertWindow(string title, string text)
	{
		InitializeComponent();

		AlertPage.Loaded += (s, e) =>
		{
			AlertPage.ViewModel.Title = title;
			AlertPage.ViewModel.Text = text;
		};
	}
}