using System.Text.RegularExpressions;

namespace srt4dvd.Services
{
    public interface IStringService
    {
        bool IsEmpty(string value);
        bool IsTimestamp(string value);
        bool IsNumber(string value);
        bool IsText(string value);
        double TimeStampToTimeIndex(string value);
        string TimeIndexToTimeStamp(double value);

        string SanitizeInput(string value);
    }

    public class StringService : IStringService
    {
        public bool IsEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public bool IsTimestamp(string value)
        {
            return (IsEmpty(value) == false) && (value.Contains("-->") && value.Contains(":") && value.Contains(",") && (value.Any(char.IsLetter) == false));
        }
        public bool IsNumber(string value)
        {
            return (IsEmpty(value) == false) && (int.TryParse(value, out _));
        }

        public bool IsText(string value)
        {
            return (IsEmpty(value) == false && IsNumber(value) == false && IsTimestamp(value) == false);
        }

        public double TimeStampToTimeIndex(string value)
        {
            if (this.IsTimestamp(value) == false) return 0;

            var t1 = value.Split("-->")[0].Trim();
            var commaSplit = t1.Split(',');
            var ms = double.Parse(commaSplit[1].Trim());

            var colonSplit = commaSplit[0].Split(":");
            var hh = double.Parse(colonSplit[0].Trim());
            var mm = double.Parse(colonSplit[1].Trim());
            var ss = double.Parse(colonSplit[2].Trim());

            var time = (hh * 3600) + (mm * 60) + ss + (ms * 0.001);

            return time;
        }

        public string TimeIndexToTimeStamp(double value)
        {
            var ts = TimeSpan.FromSeconds(value);
            ts = TimeSpan.FromMilliseconds(Math.Round(ts.TotalMilliseconds));
            return $"{ts:hh\\:mm\\:ss},{ts.Milliseconds:D3}";
        }

        public int CountNewLines(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return value.Split(Environment.NewLine).Length;
        }

        public string SanitizeInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            // remove trailing whitespace
            value = value.Trim();
            // remove HTML tags
            value = Regex.Replace(value, @"<.*?>", "", RegexOptions.IgnoreCase);
            // sanitize characters
            value = Regex.Replace(value, @"[^\p{L}\p{N}\[\]\(\),\.\?!♫♪'\-\s]", "");
            // normalize whitespace
            value = Regex.Replace(value, @"\s+", " ").Trim();

            return value;
        }
    }
}
