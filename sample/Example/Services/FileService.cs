using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Example.Settings;
using Microsoft.Extensions.Options;

namespace Byndyusoft.Example.Services
{
    public class FileService
    {
        private readonly SaveFileSettings _saveFileSettings;

        public FileService(
            IOptions<SaveFileSettings> saveFileSettings)
        {
            _saveFileSettings = saveFileSettings.Value;
        }

        public async Task SaveFileAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_saveFileSettings.FolderName, fileName);
            await using var fileStream = File.OpenWrite(filePath);
            await stream.CopyToAsync(fileStream, cancellationToken);

            await fileStream.FlushAsync(cancellationToken);
        }
    }
}