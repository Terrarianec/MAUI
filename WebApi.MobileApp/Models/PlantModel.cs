using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.Models;

public class PlantModel(PlantDto dto, IEnumerable<CountryDto> countries)
{
    public int Id { get; set; } = dto.Id;
    public string Name { get; set; } = dto.Name;
    public string Family { get; set; } = dto.Family;
    public string? Section { get; set; } = dto.Section;
    public List<PlantRootModel> PlantRoots { get; set; } = dto.PlantRoots.Select(pr => new PlantRootModel(countries.First(c => c.Id == pr.CountryId), pr.Roots)).ToList();
}
