namespace Clock;

public partial class ClockPage : ContentPage
{
	private readonly IDispatcherTimer _timer = Application.Current!.Dispatcher.CreateTimer();

	public ClockPage()
	{
		InitializeComponent();
	}

	private void OnPageLoaded(object sender, EventArgs e)
	{
		if (_timer.IsRunning)
			return;

		timeLabel.Text = DateTime.Now.ToString("HH:mm");

		_timer.Interval = TimeSpan.FromMilliseconds(100);
		_timer.Tick += OnTick;

		_timer.Start();
	}

	private void OnTick(object? sender, EventArgs e)
	{
		timeLabel.Text = DateTime.Now.ToString("HH:mm");
	}
}
