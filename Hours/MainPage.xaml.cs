namespace Hours;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(NInput.Text, out var s) || s < 0)
        {
            Results.IsVisible = false;
            NotExistsLabel.IsVisible = true;

            return;
        }

        Results.IsVisible = true;
        NotExistsLabel.IsVisible = false;

        var hours = s / 3600;

        HoursOutput.Text = hours.ToString();
    }

    private void OnButtonClick(object sender, EventArgs e)
    {
        DisplayAlert("Задание", "С начала суток прошло N секунд (N — целое). Найти количество полных часов, прошедших сначала суток.", "Понятно");
    }
}
