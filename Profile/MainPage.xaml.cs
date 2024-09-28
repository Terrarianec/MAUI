namespace Profile;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnPageLoaded(object sender, EventArgs e)
	{
		dateOfBirthPicker.MaximumDate = DateTime.Today - TimeSpan.FromDays(365 * 16);
		dateOfBirthPicker.MinimumDate = DateTime.MinValue;

		LoadData();
	}

	private void LoadData()
	{
		var preferences = Preferences.Default;

		foreach (var cell in new List<EntryCell> { surnameCell, firstnameCell, patronymicCell })
		{
			cell.Text = preferences.Get(cell.Label, string.Empty);
		}

		genderPicker.SelectedIndex = preferences.Get(genderPicker.Title, -1);

		dateOfBirthPicker.Date = preferences.Get("birth", DateTime.Today);

		if (preferences.ContainsKey("photo"))
		{
			var path = preferences.Get<string>("photo", null);
			image.ImageSource = ImageSource.FromFile(path);
			image.BindingContext = path;
		}

		foreach (var cell in new List<SwitchCell> { hostelRequiredSwitch, isHeadmanSwitch })
		{
			cell.On = preferences.Get(cell.Text, false);
		}

		if (preferences.ContainsKey(marksSection.Title))
		{
			var marks = preferences.Get(marksSection.Title, string.Empty).Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse);

			foreach (var mark in marks)
			{
				var label = new Label { VerticalOptions = LayoutOptions.CenterAndExpand };
				var stepper = new Stepper { Minimum = 2, Maximum = 5, Increment = 1, Value = mark };

				label.SetBinding(Label.TextProperty, new Binding { Source = stepper, Path = "Value", StringFormat = "Оценка: {0}" });

				var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 20 };
				layout.Children.Add(label);
				layout.Children.Add(stepper);

				var cell = new ViewCell { View = layout };

				marksSection.Add(cell);
			}
		}
	}

	public void SaveData()
	{
		var preferences = Preferences.Default;

		foreach (var cell in new List<EntryCell> { surnameCell, firstnameCell, patronymicCell })
		{
			preferences.Set(cell.Label, cell.Text);
		}

		preferences.Set(genderPicker.Title, genderPicker.SelectedIndex);

		preferences.Set("birth", dateOfBirthPicker.Date);

		if (image.ImageSource is ImageSource)
			preferences.Set("photo", (string)image.BindingContext);

		foreach (var cell in new List<SwitchCell> { hostelRequiredSwitch, isHeadmanSwitch })
		{
			preferences.Set(cell.Text, cell.On);
		}

		var marks = marksSection.Select(cell => (((cell as ViewCell).View as StackLayout).Children[0] as Label).Text.Last().ToString());

		preferences.Set(marksSection.Title, string.Join(',', marks));
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

		label.SetBinding(Label.TextProperty, new Binding { Source = stepper, Path = "Value", StringFormat = "Оценка: {0}" });

		var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 20 };
		layout.Children.Add(label);
		layout.Children.Add(stepper);

		var cell = new ViewCell { View = layout };

		marksSection.Add(cell);
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
}