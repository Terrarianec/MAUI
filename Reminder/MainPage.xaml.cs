namespace Reminder
{
	public partial class MainPage : ContentPage
	{
		private readonly IDispatcherTimer _timer = Application.Current!.Dispatcher.CreateTimer();

		public MainPage()
		{
			InitializeComponent();

			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += OnTick;
			_timer.Start();
		}

		private void OnTick(object? sender, EventArgs e)
		{
			if (Math.Abs((DateTime.Now - datePicker.Date.Add(timePicker.Time)).Ticks) < _timer.Interval.Ticks / 2)
				DisplayAlert("Напоминалка", $"{datePicker.Date.Add(timePicker.Time):dd.MM.yyyy HH:mm}\n{messageEntry.Text}", "Ладно");
		}
	}

}
