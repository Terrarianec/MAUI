namespace Circle;

public partial class MainPage : ContentPage
{
	public const double PI = 3.14;

	public MainPage()
	{
		InitializeComponent();
	}

	private void TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!double.TryParse(SInput.Text, out var s) || s <= 0)
		{
			Results.IsVisible = false;
			NotExistsLabel.IsVisible = true;

			NotExistsLabel.Text = s == 0
				? "Это точка"
				: "Круг не существует";

			return;
		}

		Results.IsVisible = true;
		NotExistsLabel.IsVisible = false;

		var d = Math.Sqrt(s / PI * 4);
		var l = PI * d;

		DOutput.Text = d.ToString();
		LOutput.Text = l.ToString();
	}
}
