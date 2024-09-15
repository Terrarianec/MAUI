namespace Clock;

internal class WeekDay(string shortName, DayOfWeek dayOfWeek)
{
	public string ShortName { get; } = shortName;
	public DayOfWeek DayOfWeek { get; } = dayOfWeek;

	public bool IsActive { get; set; }
}

public partial class AlarmPage : ContentPage
{
	private readonly IDispatcherTimer _timer = Application.Current!.Dispatcher.CreateTimer();
	private readonly List<WeekDay> _weekDays = [
		new WeekDay("пн", DayOfWeek.Monday),
		new WeekDay("вт", DayOfWeek.Tuesday),
		new WeekDay("ср", DayOfWeek.Wednesday),
		new WeekDay("чт", DayOfWeek.Thursday),
		new WeekDay("пт", DayOfWeek.Friday),
		new WeekDay("сб", DayOfWeek.Saturday),
		new WeekDay("вс", DayOfWeek.Sunday)
	];

	public AlarmPage()
	{
		InitializeComponent();

		_timer.Interval = TimeSpan.FromSeconds(1);
		_timer.Tick += OnTick;

		timePicker.Time = DateTime.Now.TimeOfDay;

		foreach (var weekDay in _weekDays)
		{
			var button = new Button() { Text = weekDay.ShortName };
			var checkBox = new CheckBox()
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Opacity = 0
			};

			button.BindingContext = checkBox;
			button.SetBinding(IsEnabledProperty, "IsChecked");

			checkBox.CheckedChanged += (s, e) =>
			{
				weekDay.IsActive = e.Value;
			};

			var grid = new Grid();
			grid.Children.Add(button);
			grid.Children.Add(checkBox);

			daysOfWeek.Children.Add(grid);
		}
	}

	private void OnTick(object? sender, EventArgs e)
	{
		var now = DateTime.Now;

		if (_weekDays.Any(d => d.IsActive && d.DayOfWeek == now.DayOfWeek)
			&& Math.Abs((now - DateTime.Today.Add(timePicker.Time)).Ticks) < _timer.Interval.Ticks / 2)
			DisplayAlert("Будильник", $"Уже {timePicker.Time:hh\\:mm}\nВремя вставать", "Отмена");
	}

	private void OnToggle(object sender, ToggledEventArgs e)
	{
		if (e.Value)
			_timer.Start();
		else
			_timer.Stop();
	}
}