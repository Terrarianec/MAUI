namespace Circle;

public partial class MainPage : ContentPage
{
    public const double PI = 3.14;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!double.TryParse(SInput.Text, out var s) || s <= 0)
        {
            Results.IsVisible = false;
            NotExistsLabel.IsVisible = true;

            NotExistsLabel.Text = s == 0 && !string.IsNullOrEmpty(SInput.Text)
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

    private void OnButtonClick(object sender, EventArgs e)
    {
        DisplayAlert("Задание", "Дана площадь S круга. Найти его диаметр D и длину L окружности, ограничивающей этот круг, учитывая, что L = PI*D, S = PI*D^2/4. В качестве значения PI использовать 3.14.", "Понятно");
    }
}
