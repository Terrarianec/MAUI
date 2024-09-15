namespace Clock;

public partial class StopwatchPage : ContentPage
{
	private readonly IDispatcherTimer _timer = Application.Current!.Dispatcher.CreateTimer();
	private DateTime _startTime = DateTime.UtcNow;

	public StopwatchPage()
	{
		InitializeComponent();

		_timer.Interval = TimeSpan.FromMilliseconds(10);
		_timer.Tick += OnTick;
	}

	private void OnTick(object? sender, EventArgs e)
	{
		timeLabel.Text = (DateTime.UtcNow - _startTime).ToString(@"mm\:ss\.fff");
	}

	private void OnStartButtonClick(object sender, EventArgs e)
	{
		if (_timer.IsRunning)
		{
			EndTimer();
			return;
		}

		timestamps.Children.Clear();
		_timer.Start();
		_startTime = DateTime.UtcNow;
		flagButton.IsEnabled = true;
	}

	private void OnFlagButtonClick(object sender, EventArgs e)
	{
		timestamps.Children.Add(new Label
		{
			Text = (DateTime.UtcNow - _startTime).ToString(@"mm\:ss\.fff"),
			FontSize = 36,
			IsEnabled = false
		});
	}

	private void EndTimer()
	{
		flagButton.IsEnabled = false;
		_timer.Stop();
		timeLabel.Text = TimeSpan.Zero.ToString(@"mm\:ss\.fff");
	}
}