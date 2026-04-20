namespace srt4dvd.Services
{
    public interface IIOService
    {
        string ReadText(string path);
        byte[] ReadBinary(string path);
        void WriteText(string path, string[] content);
        void WriteText(string path, string content);
        void WriteBinary(string path, byte[] content);

        Stream GetStream(string path);
    }
    public class IOService : IIOService
    {
        public string ReadText(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (File.Exists(path) == false) throw new FileNotFoundException(path);

            try
            {
                return File.ReadAllText(path);
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
            if (File.Exists(path) == false) throw new FileNotFoundException(path);

            try
            {
                File.WriteAllLines(path, content);
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
                File.WriteAllText(path, content);
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
                throw new Exception("Unable to load file, see inner exception.", ex);
            }

        }

        public Stream GetStream(string path)
        {
            var reader = new StreamReader(path);
            return reader.BaseStream;
        }
    }
}
