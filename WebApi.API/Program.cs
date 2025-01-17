using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.API.Database;
using WebApi.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(o =>
{
	o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddSqlServer<PlantsContext>("Server=localhost\\SQLEXPRESS;Database=Plants;User=ÈÑÏ-41;Password=1234567890;Encrypt=False");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPlants();
app.MapCountries();

app.Run();
