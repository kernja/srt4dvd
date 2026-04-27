using System.Text;

namespace srt4dvd.Services
{
    public interface IIOService
    {
        string ReadText(string path);
        byte[] ReadBinary(string path);
        void WriteText(string path, string[] content);
        void WriteText(string path, string content);
        void WriteBinary(string path, byte[] content);
      
    }
    public class IOService : IIOService
    {
        public string ReadText(string path)
        {
            try
            {
                var bytes = ReadBinary(path);

                // Try UTF-8 first
                var utf8 = Encoding.UTF8.GetString(bytes);
                if (HasValidEncoding(utf8)) return utf8;

                // Try Windows encoding
                var ansi = Encoding.Default.GetString(bytes);
                if (HasValidEncoding(ansi)) return ansi;

                // Try UTF-7 (last resort)
                var utf7 = Encoding.UTF7.GetString(bytes);
                return utf7;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load file, see inner exception.", ex);
            }
        }

        public byte[] ReadBinary(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (File.Exists(path) == false) throw new FileNotFoundException(path);

            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load file, see inner exception.", ex);
            }
        }

        public void WriteText(string path, string[] content)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            
            try
            {
                File.WriteAllLines(path, content, new System.Text.UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load file, see inner exception.", ex);
            }

        }
        public void WriteText(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
           
            try
            {
                File.WriteAllText(path, content, new System.Text.UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load file, see inner exception.", ex);
            }

        }

        public void WriteBinary(string path, byte[] content)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            
            try
            {
                File.WriteAllBytes(path, content);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to write file, see inner exception.", ex);
            }

        }

        private bool HasValidEncoding(string text)
        {
            return !text.Contains("�") && text.Any(char.IsLetter);
        }
    }
}
