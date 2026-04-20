using srt4dvd.Services.Models;
using System.Security.Cryptography.X509Certificates;

namespace srt4dvd.Services
{
    public interface ISRTOutputService
    {
        void WriteLines(string outputPath, IEnumerable<Caption> captions);
    }

    public class SRTOutputService : ISRTOutputService
    {
        private IIOService _ioService { get; set; }
        private IStringService _stringService { get; set; }

        public SRTOutputService(IIOService ioService, IStringService stringService)
        {
            _ioService = ioService;
            _stringService = stringService;
        }

        public void WriteLines(string outputPath, IEnumerable<Caption> captions)
        {
            var currentIndex = 0;
            var output = string.Empty;

            foreach (var c in captions)
            {
                currentIndex++;
                output = output + currentIndex.ToString() + Environment.NewLine;
                output = output + FormatTimeIndexes(c) + Environment.NewLine;
                output = output + c.Value + Environment.NewLine;
                output = output + Environment.NewLine;
            }

            _ioService.WriteText(outputPath, output);
        }

        public string FormatTimeIndexes(Caption caption)
        {
            var start = _stringService.TimeIndexToTimeStamp(caption.Start);
            var end = _stringService.TimeIndexToTimeStamp(caption.End);

            return $"{start} --> {end}";
        }
    }
}