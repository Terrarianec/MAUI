using WebApi.MobileApp.API;
using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.Pages;

public partial class AddPage : ContentPage
{
	private MainPage _mainPage;
	private PlantsEndpoint _plants;

	public AddPage(MainPage mainPage, PlantsEndpoint plants)
	{
		InitializeComponent();
		_mainPage = mainPage;
		_plants = plants;

		BindingContext = new PlantDto(0, string.Empty, string.Empty, null, []);
	}

	private async void OnUnloaded(object sender, EventArgs e) => await _mainPage.ReloadAsync();

	private async void OnSaveButtonClicked(object sender, EventArgs e)
	{
		if (BindingContext is not PlantDto plant)
		{
			await DisplayAlert("Îøèáêà", "Äîáàâëåíèå çàâåğøèëîñü íåóäà÷åé", "Ãğóñòèòü");
			return;
		}

		if (plant.Name is not { Length: > 2 and <= 50 })
		{
			await DisplayAlert("Îøèáêà", "Íàçâàíèå ñëèøêîì êîğîòêîå èëè ñëèøêîì äëèííîå", "Ãğóñòèòü");
			return;
		}

		if (plant.Family is not { Length: > 2 and <= 50 })
		{
			await DisplayAlert("Îøèáêà", "Íàçâàíèå ñåìåéñòâà ñëèøêîì êîğîòêîå èëè ñëèøêîì äëèííîå", "Ãğóñòèòü");
			return;
		}

		if (plant.Section is string section && section.Length == 0)
			plant.Section = null;

		if (plant.Section != null && plant.Section is not { Length: > 2 and <= 50 })
		{
			await DisplayAlert("Îøèáêà", "Íàçâàíèå ğàçäåëà ñëèøêîì êîğîòêîå èëè ñëèøêîì äëèííîå", "Ãğóñòèòü");
			return;
		}

		try
		{
			await _plants.Post(plant);
			await DisplayAlert("Óñïåøíî", "Äîáàâëåíèå çàâåğøèëîñü óäà÷åé", "Âåğíóòüñÿ");
			await Navigation.PopAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Îøèáêà", "Äîáàâëåíèå çàâåğøèëîñü íåóäà÷åé", "Ãğóñòèòü");
#if DEBUG
			throw;
#endif
		}
	}
}