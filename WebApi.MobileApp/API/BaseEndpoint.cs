using Newtonsoft.Json;
using System.Text;

namespace WebApi.MobileApp.API;

public abstract class BaseEndpoint<T>(HttpClient client, string endpoint)
{
	private readonly HttpClient _client = client;
	private readonly string _endpoint = endpoint;

	public async Task<T> Get(int id)
	{
		var result = await _client.GetAsync($"{_endpoint}/{id}");

		if (!result.IsSuccessStatusCode)
		{
			throw new HttpRequestException(result.ReasonPhrase);
		}

		var entity = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync())!;

		return entity;
	}

	public async Task<IEnumerable<T>> GetAll()
	{
		var result = await _client.GetAsync($"{_endpoint}");

		if (!result.IsSuccessStatusCode)
		{
			throw new HttpRequestException(result.ReasonPhrase);
		}

		var entity = JsonConvert.DeserializeObject<IEnumerable<T>>(await result.Content.ReadAsStringAsync())!;

		return entity;
	}

	protected async Task Put(int id, T entity)
	{
		var result = await _client.PutAsync($"{_endpoint}/{id}", new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json"));

		if (!result.IsSuccessStatusCode)
		{
			throw new HttpRequestException(result.ReasonPhrase);
		}
	}

	public async Task Post(T entity)
	{
		var result = await _client.PostAsync($"{_endpoint}", new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json"));

		if (!result.IsSuccessStatusCode)
		{
			throw new HttpRequestException(result.ReasonPhrase);
		}
	}

	public async Task Delete(int id)
	{
		var result = await _client.DeleteAsync($"{_endpoint}/{id}");

		if (!result.IsSuccessStatusCode)
		{
			throw new HttpRequestException(result.ReasonPhrase);
		}
	}
}
