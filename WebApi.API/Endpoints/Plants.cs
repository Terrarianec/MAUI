using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.API.Contracts;
using WebApi.API.Database;
using WebApi.API.Database.Tables;

namespace WebApi.API.Endpoints;

public static class Plants
{
	public static void MapPlants(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("plants");

		group.MapGet(string.Empty, GetPlants);
		group.MapGet("{id}", GetPlant);
		group.MapPut("{id}", PutPlant);
		group.MapPost(string.Empty, PostPlant);
		group.MapDelete("{id}", DeletePlant);
	}

	public static async Task<IResult> GetPlants(PlantsContext context) => TypedResults.Ok(await context.Plants.Include(p => p.PlantsRoots).Select(p => PlantDto.FromEntity(p)).ToListAsync());

	public static async Task<IResult> GetPlant(PlantsContext context, int id)
	{
		var plant = await context.Plants.Include(p => p.PlantsRoots).FirstAsync(p => p.Id == id);

		if (plant == null)
			return TypedResults.NotFound((PlantDto)null!);

		return TypedResults.Ok(PlantDto.FromEntity(plant));
	}

	public static async Task<IResult> PutPlant(PlantsContext context, int id, [FromBody] PlantDto plantDto)
	{
		if (id != plantDto.Id)
		{
			return Results.BadRequest();
		}

		var plant = await context.Plants.Include(p => p.PlantsRoots).FirstAsync(p => p.Id == id);

		plant.Name = plantDto.Name;
		plant.Family = plantDto.Family;
		plant.Section = plantDto.Section;
		plant.PlantsRoots = plantDto.PlantRoots.Select(dto => new PlantRoot { CountryId = dto.CountryId, Roots = dto.Roots, PlantId = plantDto.Id }).ToList();

		try
		{
			await context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!PlantExists(context, id))
				return Results.NotFound();
			else
				throw;
		}

		return Results.NoContent();
	}

	public static async Task<IResult> PostPlant(PlantsContext context, [FromBody] PlantDto plantDto)
	{
		var plant = new Plant
		{
			Id = plantDto.Id,
			Name = plantDto.Name,
			Family = plantDto.Family,
			Section = plantDto.Section,
			PlantsRoots = plantDto.PlantRoots.Select(dto => new PlantRoot { CountryId = dto.CountryId, Roots = dto.Roots, PlantId = plantDto.Id }).ToList()
		};

		context.Plants.Add(plant);
		await context.SaveChangesAsync();

		return Results.Created();
	}

	public static async Task<IResult> DeletePlant(PlantsContext context, int id)
	{
		var plant = await context.Plants.FindAsync(id);

		if (plant == null)
			return Results.NotFound();

		context.Plants.Where(p => p.Id == id).ExecuteDelete();

		return Results.NoContent();
	}

	private static bool PlantExists(PlantsContext context, int id) => context.Plants.Any(p => p.Id == id);
}