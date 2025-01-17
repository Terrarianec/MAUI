using Microsoft.Extensions.Logging;
using WebApi.MobileApp.API;
using WebApi.MobileApp.Pages;

namespace WebApi.MobileApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost:5130") });
		builder.Services.AddSingleton<PlantsEndpoint>();
		builder.Services.AddSingleton<CountriesEndpoint>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<EditPage>();
		builder.Services.AddTransient<AddPage>();

		return builder.Build();
	}
}
