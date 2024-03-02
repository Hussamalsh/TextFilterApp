using Microsoft.Extensions.DependencyInjection;
using TextFilterApp.Configuration;
using TextFilterApp.Interfaces;

namespace TextFilterApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = HostConfiguration.CreateHostBuilder(args).Build();

            // Retrieve the file processor service
            var fileProcessor = host.Services.GetRequiredService<IFileProcessor>();
            if (fileProcessor == null)
            {
                Console.WriteLine("File processor service is not available.");
                return;
            }

            // Get the file path from the configuration
            var appSettings = host.Services.GetRequiredService<AppSettings>();
            var filePath = appSettings.FilePath;

            // Process the file
            try
            {
                await fileProcessor.ProcessFileAsync(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}
