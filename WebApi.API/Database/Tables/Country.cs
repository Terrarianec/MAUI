namespace WebApi.API.Database.Tables;

public partial class Country
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public string Mainland { get; set; } = null!;

	public string? Capital { get; set; }

	public virtual List<PlantRoot> PlantsRoots { get; set; } = [];
}
