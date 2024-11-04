using Profile.Storage;

namespace Profile;

public partial class MainPage : ContentPage
{
	private IStorage? _previousStorage = null;

	public string Marks
	{
		get => (string)(GetValue(MarksProperty) ?? "ещё не получены");
		set
		{
			SetValue(MarksProperty, string.Join(", ", marksSection.Where(c => c is ViewCell cell && cell.View is StackLayout).Select(c =>
			{
				ViewCell cell = (ViewCell)c;
				var layout = (StackLayout)cell.View;

				return ((Stepper)layout.Children.Last()).Value.ToString();
			})));
		}
	}
	public IStorage Storage => _storages.TryGetValue(storagePicker.SelectedItem?.ToString() ?? string.Empty, out IStorage storage) ? storage : null;
	private readonly Dictionary<string, IStorage> _storages = [];

	public static BindableProperty MarksProperty = BindableProperty.Create(nameof(Marks), typeof(string), typeof(MainPage));

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnPageLoaded(object sender, EventArgs e)
	{
		dateOfBirthPicker.MaximumDate = DateTime.Today - TimeSpan.FromDays(365 * 16);
		dateOfBirthPicker.MinimumDate = DateTime.MinValue;

		var storages = new IStorage[] { new PreferenceStorage(Preferences.Default), new AppDataStorage(FileSystem.Current.AppDataDirectory) };

		foreach (var storage in storages)
		{
			var name = storage.GetType().Name;

			_storages.Add(name, storage);
			storagePicker.Items.Add(name);
		}

		storagePicker.SelectedIndex = 0;

		LoadData(Storage);
	}

	private void LoadData(IStorage storage)
	{
		if (storage == null)
			return;

		foreach (var entry in new List<Entry> { lastname, firstname, patronymic })
		{
			var label = (Label)((StackLayout)entry.Parent).Children.First();
			entry.Text = storage.Get(label.Text, string.Empty);
		}

		genderPicker.SelectedIndex = storage.Get(genderPicker.Title, -1);

		dateOfBirthPicker.Date = storage.Get("birth", DateTime.Today);

		if (storage.ContainsKey("photo"))
		{
			var path = storage.Get<string>("photo", null!);
			if (path != null)
			{
				image.ImageSource = ImageSource.FromFile(path);
				image.BindingContext = path;
			}
		}
		else
		{
			image.ImageSource = null;
		}

		foreach (var cell in new List<SwitchCell> { hostelRequiredSwitch, isHeadmanSwitch })
		{
			cell.On = storage.Get(cell.Text, false);
		}

		if (storage.ContainsKey(marksSection.Title))
		{
			var marks = storage.Get(marksSection.Title, string.Empty).Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse);

			foreach (var mark in marks)
			{
				var label = new Label { VerticalOptions = LayoutOptions.CenterAndExpand };
				var stepper = new Stepper { Minimum = 2, Maximum = 5, Increment = 1, Value = mark };
				stepper.ValueChanged += (s, e) => Marks = string.Empty;

				label.SetBinding(Label.TextProperty, new Binding { Source = stepper, Path = "Value", StringFormat = "Оценка: {0}" });

				var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 20 };
				layout.Children.Add(label);
				layout.Children.Add(stepper);

				var cell = new ViewCell { View = layout };

				marksSection.Add(cell);
			}

			Marks = string.Empty;
		}
	}

	public void SaveData(IStorage storage)
	{
		if (storage == null)
			return;

		foreach (var entry in new List<Entry> { lastname, firstname, patronymic })
		{
			var label = (Label)((StackLayout)entry.Parent).Children.First();
			storage.Set(label.Text, entry.Text);
		}

		storage.Set(genderPicker.Title, genderPicker.SelectedIndex);

		storage.Set("birth", dateOfBirthPicker.Date);

		if (image.ImageSource is not null)
			storage.Set("photo", (string)image.BindingContext);

		foreach (var cell in new List<SwitchCell> { hostelRequiredSwitch, isHeadmanSwitch })
		{
			storage.Set(cell.Text, cell.On);
		}

		var marks = marksSection.Select(cell => (((cell as ViewCell).View as StackLayout).Children[0] as Label).Text.Last().ToString());

		storage.Set(marksSection.Title, string.Join(',', marks));
	}

	private void OnDateOfBirthSelected(object sender, DateChangedEventArgs e)
	{
		var age = DateTime.Now.Year - e.NewDate.Year;

		if (DateTime.Now.DayOfYear < e.NewDate.DayOfYear)
			age--;

		ageLabel.Text = age.ToString("0 лет");
	}

	private void OnAddMarkButtonClick(object sender, EventArgs e)
	{
		var label = new Label { VerticalOptions = LayoutOptions.CenterAndExpand };
		var stepper = new Stepper { Minimum = 2, Maximum = 5, Increment = 1 };
		stepper.ValueChanged += (s, e) => Marks = string.Empty;

		label.SetBinding(Label.TextProperty, new Binding { Source = stepper, Path = "Value", StringFormat = "Оценка: {0}" });

		var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 20 };
		layout.Children.Add(label);
		layout.Children.Add(stepper);

		var cell = new ViewCell { View = layout };

		marksSection.Add(cell);
		Marks = string.Empty;
	}

	private async void OnImagePickerClicked(object sender, EventArgs e)
	{
		if (sender is not Button button)
			return;

		var result = await FilePicker.PickAsync(new PickOptions { FileTypes = FilePickerFileType.Images, PickerTitle = button.Text });
		if (result == null)
			return;

		image.ImageSource = ImageSource.FromFile(result.FullPath);
		image.BindingContext = result.FullPath;
	}

	private async void OnCompressButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button button)
			return;

		button.IsEnabled = false;

		var elements = new List<VisualElement> { table, card }.OrderByDescending(e => e.ScaleY);

		foreach (VisualElement element in elements)
			await element.ScaleYTo(Math.Round(1 - element.ScaleY), 250);

		button.IsEnabled = true;

		SaveData(Storage);
	}

	private void OnSelectedStorageChanged(object sender, EventArgs e)
	{
		if (_previousStorage != null)
			SaveData(_previousStorage);

		_previousStorage = Storage;

		if (Storage != null)
			LoadData(Storage);
	}
}