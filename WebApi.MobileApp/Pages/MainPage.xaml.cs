using WebApi.MobileApp.API;
using WebApi.MobileApp.Models;

namespace WebApi.MobileApp.Pages;

public partial class MainPage : ContentPage
{
	private readonly IServiceProvider _services;
	private readonly PlantsEndpoint _plants;
	private readonly CountriesEndpoint _countries;

	public MainPage(IServiceProvider services)
	{
		InitializeComponent();
		_services = services;

		_plants = services.GetRequiredService<PlantsEndpoint>();
		_countries = services.GetRequiredService<CountriesEndpoint>();
	}

	private async void OnLoaded(object sender, EventArgs e) => await ReloadAsync();

	private async void OnAddButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button)
			return;

		var addpage = _services.GetRequiredService<AddPage>();

		await Navigation.PushAsync(addpage);
	}

	private async void OnEditButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button button || !int.TryParse(button.ClassId, out var plantId))
			return;

		var editPage = _services.GetRequiredService<EditPage>();
		if (await editPage.SetPlant(plantId))
		{
			await Navigation.PushAsync(editPage);
			return;
		}

		await DisplayAlert("Ошибка", "Не удалось открыть страницу редактирования", "Грустить");
	}

	private async void OnRemoveButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button button || !int.TryParse(button.ClassId, out var plantId))
			return;

		if (await DisplayAlert("Удаление растения", "Вы точно хотите истребить это растение?", "Истребить", "Сбежать"))
		{
			try
			{
				await _plants.Delete(plantId);
				await ReloadAsync();
			}
			catch
			{
				await DisplayAlert("Ошибка", "Удаление завершилось неудачей", "Грустить");
			}
		}
	}

	public async Task ReloadAsync()
	{
		var countries = await _countries.GetAll();
		var dtos = await _plants.GetAll();

		var models = dtos.Select(dto => new PlantModel(dto, countries)).ToList();
		plantsListView.ItemsSource = models;
	}
}