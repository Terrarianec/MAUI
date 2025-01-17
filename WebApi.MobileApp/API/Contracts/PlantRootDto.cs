using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.MobileApp.API.Contracts;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public record class PlantRootDto(int CountryId, int Roots);