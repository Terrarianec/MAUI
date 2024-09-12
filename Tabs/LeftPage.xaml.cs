namespace Tabs
{
	public partial class LeftPage : ContentPage
	{
		private readonly string[] _seasons = ["зима", "весна", "лето", "осень"];

		public LeftPage()
		{
			InitializeComponent();
		}

		private void OnPageLoaded(object sender, EventArgs e)
		{
			if (Months.Children.Count == 0)
			{
				foreach (string month in new string[] { "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" })
				{
					var radioButton = new RadioButton { Content = $"{char.ToUpper(month[0])}{month[1..]}" };
					radioButton.CheckedChanged += OnMonthSelected;

					Months.Children.Add(radioButton);
				}
			}
		}

		private void OnMonthSelected(object? sender, CheckedChangedEventArgs e)
		{
			if (sender is not RadioButton radioButton || e.Value == false)
				return;

			var index = Months.Children.IndexOf(radioButton);
			MonthNumber.Text = $"{index + 1}-й";
			Season.Text = $"{_seasons[(index + 1) / 3 % 4]}";
		}
	}

}
