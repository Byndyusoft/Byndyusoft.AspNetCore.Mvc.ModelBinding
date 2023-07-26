using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Byndyusoft.IntegrationTests
{
    public class TestHelper
    {
        public static StreamContent CreateStreamContent(string fileName)
        {
            var filePath = GetFilePath(fileName);
            var fileInfo = new FileInfo(filePath);
            var fileStream = fileInfo.OpenRead();

            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentLength = fileInfo.Length;

            return streamContent;
        }

        public static async Task<byte[]> ReadFileBytesAsync(string fileName)
        {
            return await File.ReadAllBytesAsync(GetFilePath(fileName));
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine("TestFiles", fileName);
        }
    }
}