using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.API;

public class CountriesEndpoint(HttpClient client) : BaseEndpoint<CountryDto>(client, "countries")
{
	public Task Put(CountryDto countryDto)
		=> Put(countryDto.Id, countryDto);
}
