namespace WebApi.API.Database.Tables;

public partial class Plant
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public string Family { get; set; } = null!;

	public string? Section { get; set; }

	public virtual List<PlantRoot> PlantsRoots { get; set; } = [];
}
