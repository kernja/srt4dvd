using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using srt4dvd.Services;

namespace srt4dvd.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arguments = ProcessArgs(args);

            var services = CreateServices();
            var srt = services.GetRequiredService<ISRTService>();

            srt.ProcessFile(arguments.sourceFile, arguments.destinationFile);

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

        private static (string sourceFile, string destinationFile) ProcessArgs(string[] args)
        {
            if (args.Length != 1) throw new Exception("Invalid number of arguments.");
            var splitArgs = args[0].Split(".");
            if (splitArgs.Length != 2) throw new Exception("Please make sure that the input file and path only contains one period.");
            if (splitArgs[1].ToUpperInvariant() != "SRT") throw new Exception("Only SRT files are supported.");

            return (args[0], splitArgs[1] + "-Sanitized.srt");
        }
    }
}
