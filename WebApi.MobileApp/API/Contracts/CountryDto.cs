using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.MobileApp.API.Contracts;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public record class CountryDto(int Id, string Name, string Mainland, string? Capital);