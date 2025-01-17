using WebApi.MobileApp.API;
using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.Pages;

public partial class EditPage : ContentPage
{
    private int _plantId;
    private MainPage _mainPage;
    private PlantsEndpoint _plants;

    public async Task<bool> SetPlant(int plantId)
    {
        try
        {
            var plant = await _plants.Get(plantId);
            BindingContext = plant;

            _plantId = plantId;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public EditPage(MainPage mainPage, PlantsEndpoint plants)
    {
        InitializeComponent();
        _mainPage = mainPage;
        _plants = plants;
    }

    private async void OnUnloaded(object sender, EventArgs e) => await _mainPage.ReloadAsync();

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is not PlantDto plant)
        {
            await DisplayAlert("Ошибка", "Изменение завершилось неудачей", "Грустить");
            return;
        }

        if (plant.Name is not { Length: > 2 and <= 50 })
        {
            await DisplayAlert("Ошибка", "Название слишком короткое или слишком длинное", "Грустить");
            return;
        }

        if (plant.Family is not { Length: > 2 and <= 50 })
        {
            await DisplayAlert("Ошибка", "Название семейства слишком короткое или слишком длинное", "Грустить");
            return;
        }

        if (plant.Section is string section && section.Length == 0)
            plant.Section = null;

        if (plant.Section != null && plant.Section is not { Length: > 2 and <= 50 })
        {
            await DisplayAlert("Ошибка", "Название раздела слишком короткое или слишком длинное", "Грустить");
            return;
        }

        try
        {
            await _plants.Put(plant);
            await DisplayAlert("Успешно", "Изменение завершилось удачей", "Вернуться");
            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Ошибка", "Изменение завершилось неудачей", "Грустить");
        }
    }
}