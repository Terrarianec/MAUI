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
		new WeekDay("��", DayOfWeek.Monday),
		new WeekDay("��", DayOfWeek.Tuesday),
		new WeekDay("��", DayOfWeek.Wednesday),
		new WeekDay("��", DayOfWeek.Thursday),
		new WeekDay("��", DayOfWeek.Friday),
		new WeekDay("��", DayOfWeek.Saturday),
		new WeekDay("��", DayOfWeek.Sunday)
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
			DisplayAlert("���������", $"��� {timePicker.Time:hh\\:mm}\n����� ��������", "������");
	}

	private void OnToggle(object sender, ToggledEventArgs e)
	{
		if (e.Value)
			_timer.Start();
		else
			_timer.Stop();
	}
}