using WebApi.MobileApp.API.Contracts;

namespace WebApi.MobileApp.API;

public class PlantsEndpoint(HttpClient client) : BaseEndpoint<PlantDto>(client, "plants")
{
	public Task Put(PlantDto entity)
		=> Put(entity.Id, entity);
}
