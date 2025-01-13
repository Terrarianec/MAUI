namespace Navigation;

public partial class GuardModalPage : ContentPage
{
	private AccessManager _accessManager;

	public GuardModalPage(AccessManager accessManager)
	{
		InitializeComponent();
		_accessManager = accessManager;
	}

	private async void OnPasswordEntered(object sender, EventArgs e)
	{
		if (sender is not Entry)
			return;

		if (passwordEntry.Text == "123")
		{
			_accessManager.Accept();
			await Navigation.PopModalAsync();
		}
		else
		{
			_accessManager.Decline();
		}
	}
}