using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApi.API.Database.Tables;

namespace WebApi.API.Contracts;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public record class CountryDto(int Id, string Name, string Mainland, string? Capital)
{
	public static CountryDto FromModel(Country country) => new(country.Id, country.Name.Trim(), country.Mainland.Trim(), country.Capital?.Trim());
}