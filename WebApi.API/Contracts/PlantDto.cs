using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApi.API.Database.Tables;

namespace WebApi.API.Contracts;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public record class PlantDto(int Id, string Name, string Family, string? Section, List<PlantRootDto> PlantRoots)
{
	public static PlantDto FromEntity(Plant plant)
		=> new(plant.Id,
				plant.Name.Trim(),
				plant.Family.Trim(),
				plant.Section?.Trim(),
				plant.PlantsRoots.Select(pr => new PlantRootDto(pr.CountryId, pr.Roots)).ToList());

}