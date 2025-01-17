using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.MobileApp.API.Contracts;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class PlantDto(int id, string name, string family, string? section, List<PlantRootDto> plantRoots)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Family { get; set; } = family;
    public string? Section { get; set; } = section;
    public List<PlantRootDto> PlantRoots { get; set; } = plantRoots;
}