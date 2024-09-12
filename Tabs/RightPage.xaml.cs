namespace Tabs;

public partial class RightPage : ContentPage
{
	private readonly string[] _seasons = ["����", "�����", "����", "�����"];
	private int _selectedIndex = -1;

	public RightPage()
	{
		InitializeComponent();
	}

	private void OnPageLoaded(object sender, EventArgs e)
	{
		if (Months.Children.Count == 0)
		{
			foreach (string month in new string[] { "������", "�������", "����", "������", "���", "����", "����", "������", "��������", "�������", "������", "�������" })
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

		if (_selectedIndex != index)
		{
			foreach (var entry in new Entry[] { MonthNumber, Season })
				entry.Text = string.Empty;
		}

		_selectedIndex = index;
	}

	private void OnButtonClicked(object sender, EventArgs e)
	{
		if (_selectedIndex == -1)
		{
			DisplayAlert("������", "������� ����� ������� ���� �����-�� �����", "�����");
			return;
		}

		MonthNumber.Text = $"{_selectedIndex + 1}-�";
		Season.Text = $"{_seasons[(_selectedIndex + 1) / 3 % 4]}";
	}
}