using srt4dvd.Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srt4dvd.Services
{
    public interface ISRTService
    {
        void ProcessFile(string inputPath, string outputPath)
    }
    public class SRTService : ISRTService
    {
        private ISRTInputService _inputService { get; set; }
        private IStringService _stringService { get; set; }
        private ISRTOutputService _outputService { get; set; }

        const int cMaxLines = 3;
        const double cBuffer = 0.25;
        const double cLookAhead = 2.0;
        const double cBaseCharTime = 0.6;
        const double cCharsPerSecond = 15;

        const double cOneLineTime = 1.3;
        const double cTwoLineTime = 3;
        const double cThreeLineTime = 6;

        int counter = 1;
        double currentTimeIndex = 0;
        IList<Caption> captions;
        IList<Line> workingLines;


        public SRTService(ISRTInputService inputService, ISRTOutputService outputService, IStringService stringService)
        {
            _inputService = inputService;
            _outputService = outputService;
            _stringService = stringService;
        }

        public void ProcessFile(string inputPath, string outputPath)
        {
            var rawLines = _inputService.ReadLines(inputPath);

            // reset working variables
            counter = 1;
            currentTimeIndex = 0;
            captions = new List<Caption>();
            workingLines = new List<Line>();

            // perform our logic
            foreach (var current in rawLines)
            {
                if (workingLines.Count == 0)
                {
                    workingLines.Add(current);
                    continue;
                }

                bool shouldFlush =
                    workingLines.Count == cMaxLines ||
                    current.Start > workingLines[0].Start + cLookAhead;

                if (shouldFlush)
                {
                    FlushCaptions();
                }

                workingLines.Add(current);
            }

            // final flush
            if (workingLines.Count > 0)
            {
                FlushCaptions();
            }

            // write results
            _outputService.WriteLines(outputPath, captions);
        }

        private void FlushCaptions()
        {
            // get how many lines are in this caption
            int lineCount = workingLines.Count;
            
            // if we don't have any, bail
            if (lineCount == 0) return;

            // get our first line and use it along with the current time index to determine our start timestamp
            var first = workingLines[0];
            double start = Math.Max(first.Start, currentTimeIndex + cBuffer);

            // Get number characters
            int charCount = workingLines.Sum(x => x.Value.Length);

            // determine how long the caption should be up for (based on lines alone)
            double baseDuration = lineCount switch
            {
                1 => cOneLineTime,
                2 => cTwoLineTime,
                3 => cThreeLineTime,
                _ => cLookAhead
            };

            // now do an estimate based on number of characters
            double readingTime = cBaseCharTime + (charCount / cCharsPerSecond);
            // now select which value should be used
            double duration = Math.Max(baseDuration, readingTime);

            // make sure that it exists within our range
            duration = Math.Clamp(duration, cOneLineTime, cThreeLineTime);
            // set our end time stamp
            double end = start + duration;

            // add our caption
            captions.Add(new Caption
            {
                Start = start,
                End = end,
                Value = string.Join(Environment.NewLine, workingLines.Select(x => x.Value))
            });
            
            // update current time index
            currentTimeIndex = end;

            // clear out working lines
            workingLines.Clear();

        }
    }
}