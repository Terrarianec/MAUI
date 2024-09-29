﻿namespace Profile;

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

		var images = new List<string> {
			"image_01_neuro.png",
			"image_02_neuro.png",
			"image_03_neuro.png",
			"image_04_neuro.png",
			"image_05_neuro.png",
			"image_06_neuro.png",
			"image_07_neuro.png"
		}.Select(path => ImageSource.FromFile(path)).ToList();
		imagePathPicker.ItemsSource = images;
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
}