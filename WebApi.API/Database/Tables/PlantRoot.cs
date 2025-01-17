namespace WebApi.API.Database.Tables;

public partial class PlantRoot
{
	public int CountryId { get; set; }

	public int PlantId { get; set; }

	public int Roots { get; set; }

	public virtual Country Country { get; set; } = null!;

	public virtual Plant Plant { get; set; } = null!;
}
