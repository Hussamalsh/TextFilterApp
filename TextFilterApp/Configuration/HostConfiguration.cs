using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TextFilterApp.Filters;
using TextFilterApp.Interfaces;

namespace TextFilterApp.Configuration;

public static class HostConfiguration
{
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Bind configuration to settings object
                var appSettings = new AppSettings();
                hostContext.Configuration.GetSection("AppSettings").Bind(appSettings);

                // Register settings as a singleton
                services.AddSingleton(appSettings);

                // Register filters based on settings
                services.AddSingleton<ITextFilter, TextFilter>(serviceProvider =>
                {
                    var filters = new List<IFilterStrategy>();
                    if (appSettings.Filters.EnableVowelMiddleFilter)
                    {
                        filters.Add(new VowelMiddleFilter());
                    }
                    if (appSettings.Filters.EnableMinLengthFilter)
                    {
                        filters.Add(new MinLengthFilter());
                    }
                    if (appSettings.Filters.EnableContainsTFilter)
                    {
                        filters.Add(new ContainsTFilter());
                    }
                    return new TextFilter(filters);
                });

                services.AddTransient<IFileProcessor, TextProcessor>();
                services.AddTransient<IFileSystem, FileSystem>();
            });
}

