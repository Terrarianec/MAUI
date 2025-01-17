using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.Models;

public class PlantRootModel(CountryDto country, int roots)
{
    public CountryDto Country { get; set; } = country;
    public int Roots { get; set; } = roots;
}
