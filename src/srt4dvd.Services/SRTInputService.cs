using srt4dvd.Services.Models;

namespace srt4dvd.Services
{
    public interface ISRTInputService
    {
        IEnumerable<Line> ReadLines(string path);
    }

    public class SRTInputService : ISRTInputService
    {
        private IIOService _ioService { get; set; }
        private IStringService _stringService { get; set; }
        public SRTInputService(IIOService ioService, IStringService stringService)
        {
            _ioService = ioService;
            _stringService = stringService;
        }

        public IEnumerable<Line> ReadLines(string path)
        {
            var rawInput = _ioService.ReadText(path);
            var rawLines = rawInput.Split(Environment.NewLine);

            var filteredLines = new List<Line>();

            double activeTimeIndex = 0;

            foreach (var l in rawLines)
            {
                if (_stringService.IsNumber(l) == true) continue;
                if (_stringService.IsEmpty(l) == true) continue;
                if (_stringService.IsTimestamp(l) == true)
                {
                    activeTimeIndex = _stringService.TimeStampToTimeIndex(l);
                    continue;
                }

                filteredLines.Add(new Line
                {
                    Value = _stringService.SanitizeInput(l),
                    Start = activeTimeIndex,
                });
            }

            return filteredLines.OrderBy(x => x.Start);
        }
    }
}