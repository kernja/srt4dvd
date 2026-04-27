using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using srt4dvd.Services;

namespace srt4dvd.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = CreateServices();
            var srt = services.GetRequiredService<ISRTService>();

            // loop through each argument
            foreach (var i in args)
            {
                // sanitize
                var sanitizedArg = SanitizeArgs(i);
                try
                {
                    // process
                    srt.ProcessFile(sanitizedArg.sourceFile, sanitizedArg.destinationFile);
                    Console.WriteLine($"- Processed file '{sanitizedArg.sourceFile}' as '{sanitizedArg.destinationFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"- Error processing file '{sanitizedArg.sourceFile}' due to the following exception: {ex.Message}");
                }
            }
           

        }

        private static ServiceProvider CreateServices()
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .Build();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IIOService, IOService>()
                .AddTransient<IStringService, StringService>()
                .AddTransient<ISRTInputService, SRTInputService>()
                .AddTransient<ISRTOutputService, SRTOutputService>()
                .AddTransient<ISRTService, SRTService>()
                .AddSingleton<IConfiguration>(configuration);

            return serviceProvider.BuildServiceProvider();
        }

        private static (string sourceFile, string destinationFile) SanitizeArgs(string arg)
        {
            var splitArgs = arg.Split(".");
            if (splitArgs.Length != 2) throw new Exception("Please make sure that the input file and path only contains one period.");
            if (splitArgs[1].ToUpperInvariant() != "SRT") throw new Exception("Only SRT files are supported.");

            return (arg, splitArgs[0] + "-Sanitized.srt");
        }
    }
}
