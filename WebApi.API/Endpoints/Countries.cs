using Microsoft.EntityFrameworkCore;
using WebApi.API.Contracts;
using WebApi.API.Database;

namespace WebApi.API.Endpoints;

public static class Countries
{
	public static void MapCountries(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("countries");

		group.MapGet(string.Empty, GetCountries);
	}

	public static async Task<IEnumerable<CountryDto>> GetCountries(PlantsContext context) => await context.Countries.Select(c => CountryDto.FromModel(c)).ToListAsync();
}