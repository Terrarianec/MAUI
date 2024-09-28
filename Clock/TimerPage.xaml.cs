namespace Clock;

public partial class TimerPage : ContentPage
{
	private readonly IDispatcherTimer _timer = Application.Current!.Dispatcher.CreateTimer();
	private bool _paused = false;

	private TimeSpan _time = TimeSpan.Zero;

	public TimerPage()
	{
		InitializeComponent();

		_timer.Interval = TimeSpan.FromMilliseconds(100);
		_timer.Tick += OnTick;
	}

	private void OnTick(object? sender, EventArgs e)
	{
		if (_time <= TimeSpan.Zero)
		{
			EndTime();
			DisplayAlert("Таймер", "Время истекло", "Жаль");
			return;
		}

		if (!_paused)
			_time -= _timer.Interval;

		timerLabel.BindingContext = _time;
	}

	private void OnIncreaseMinutesButtonClicked(object sender, EventArgs e)
	{
		minutesLabel.Text = ((int.Parse(minutesLabel.Text) + 1) % 60).ToString("00");
	}

	private void OnDecreaseMinutesButtonClicked(object sender, EventArgs e)
	{
		minutesLabel.Text = ((int.Parse(minutesLabel.Text) + 59) % 60).ToString("00");
	}

	private void OnIncreaseSecondsButtonClicked(object sender, EventArgs e)
	{
		secondsLabel.Text = ((int.Parse(secondsLabel.Text) + 1) % 60).ToString("00");
	}

	private void OnDecreaseSecondsButtonClicked(object sender, EventArgs e)
	{
		secondsLabel.Text = ((int.Parse(secondsLabel.Text) + 59) % 60).ToString("00");
	}

	private void OnPauseButtonClick(object sender, EventArgs e)
	{
		_paused = !_paused;

		timerLabel.IsEnabled = !_paused;
	}

	private void OnStartButtonClicked(object sender, EventArgs e)
	{
		if (_paused)
		{
			_paused = false;
			timerLabel.IsEnabled = !_paused;
			return;
		}

		if (_timer.IsRunning)
		{
			_paused = false;
			EndTime();
			return;
		}

		_timer.Start();
		_paused = false;

		_time = TimeSpan.FromMinutes(int.Parse(minutesLabel.Text)).Add(TimeSpan.FromSeconds(int.Parse(secondsLabel.Text)));

		foreach (View child in timeSelection.Children)
			child.IsVisible = false;

		timerLabel.IsVisible = true;
	}

	private void EndTime()
	{
		foreach (View child in timeSelection.Children)
			child.IsVisible = true;

		timerLabel.IsVisible = false;

		_timer.Stop();
	}
}