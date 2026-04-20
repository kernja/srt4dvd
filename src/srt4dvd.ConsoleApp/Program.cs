using srt4dvd.Services;
using srt4dvd.Services.Formats;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace srt4dvd.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arguments = ProcessArgs(args);

            var services = CreateServices();
            var jpg2kService = services.GetRequiredService<IJPG2KService>();
            var bz2Service = services.GetRequiredService<IBZ2Service>();
            var pngService = services.GetRequiredService<IPNGService>();
            var jfifService = services.GetRequiredService<IJFIFService>();
            var hpiService = services.GetRequiredService<IHPIService>();
            var gzipService = services.GetRequiredService<IGZIPService>();
            var wmfService = services.GetRequiredService<IWMFService>();
            var emfService = services.GetRequiredService<IEMFService>();
            var mp3Service = services.GetRequiredService<IMP3InPDFService>();
            var io = services.GetRequiredService<IIOService>();

            var sourceFile = arguments.sourceFile;
            var outputFolder = arguments.targetDirectory;

            // Used to process directories
            /*
            for (var i = 1; i <= 32; i++)
            {
                if ((i == 21) == false) continue;

                sourceFile = arguments.sourceFile + (i <= 9 ? $"c0{i}.pdf" : $"c{i}.pdf");
                outputFolder = arguments.targetDirectory + (i <= 9 ? $"CD0{i}" : $"CD{i}");
            */

            using (var s = io.GetStream(sourceFile))
            {

                int b = s.ReadByte();
                while (b >= 0)
                {
                    if (arguments.mode.Contains("WMF")) ProcessStreams(io, outputFolder, wmfService.ProcessStream(b));
                    if (arguments.mode.Contains("EMF")) ProcessStreams(io, outputFolder, emfService.ProcessStream(b));
                    if (arguments.mode.Contains("GZIP")) ProcessStreams(io, outputFolder, gzipService.ProcessStream(b));
                    if (arguments.mode.Contains("HPI")) ProcessStreams(io, outputFolder, hpiService.ProcessStream(b));
                    if (arguments.mode.Contains("JFIF")) ProcessStreams(io, outputFolder, jfifService.ProcessStream(b));
                    if (arguments.mode.Contains("PNG")) ProcessStreams(io, outputFolder, pngService.ProcessStream(b));
                    if (arguments.mode.Contains("BZ2")) ProcessStreams(io, outputFolder, bz2Service.ProcessStream(b));
                    if (arguments.mode.Contains("JP2")) ProcessStreams(io, outputFolder, jpg2kService.ProcessStream(b));
                    if (arguments.mode.Contains("MP3-PDF")) ProcessStreams(io, outputFolder, mp3Service.ProcessStream(b));


                    b = s.ReadByte();
                }

            }

            if (arguments.mode.Contains("GZIP")) ProcessStreams(io, outputFolder, gzipService.Flush());
            //}

        }

        private static ServiceProvider CreateServices()
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .Build();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IIOService, IOService>()
                .AddTransient<IJPG2KService, JPG2KService>()
                .AddTransient<IBZ2Service, BZ2Service>()
                .AddTransient<IPNGService, PNGService>()
                .AddTransient<IJFIFService, JFIFService>()
                .AddTransient<IHPIService, HPIService>()
                .AddTransient<IGZIPService, GZIPService>()
                .AddTransient<IWMFService, WMFService>()
                .AddTransient<IEMFService, EMFService>()
                .AddTransient<IMP3InPDFService, MP3InPDFService>()
                .AddTransient<IStreamExtractService, StreamExtractService>()
                .AddSingleton<IConfiguration>(configuration);

            return serviceProvider.BuildServiceProvider();
        }

        private static (string sourceFile, string targetDirectory, string mode) ProcessArgs(string[] args)
        {
            if (args.Length != 3) throw new Exception("Invalid number of arguments.");
            return (args[0], args[1], args[2].ToUpper());
        }

        private static void ProcessStreams(IIOService ioService, string outputFolder, (bool withFile, byte[]? fileBytes, string? extension) result)
        {
            if (result.withFile == false) return;

            ioService.WriteBinary($"{outputFolder}\\{DateTime.Now.ToString("MM-dd-yyyy-hh-mm-ss-f")}{result.extension!}", result.fileBytes!);
        }
    }
}
