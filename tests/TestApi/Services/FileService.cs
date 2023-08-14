using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.TestApi.Settings;
using Microsoft.Extensions.Options;

namespace Byndyusoft.TestApi.Services
{
    public class FileService
    {
        private readonly StorageService _storageService;
        private readonly SaveFileSettings _saveFileSettings;

        public FileService(
            IOptions<SaveFileSettings> saveFileSettings,
            StorageService storageService)
        {
            _storageService = storageService;
            _saveFileSettings = saveFileSettings.Value;
        }

        public async Task<string> UploadToStorageAsync(Stream stream, string fileName, long fileSize, CancellationToken cancellationToken)
        {
            return await _storageService.UploadStreamAsync(stream, fileName, fileSize, cancellationToken);
        }

        public async Task<string> SaveFileAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_saveFileSettings.FolderName);

            var filePath = Path.Combine(_saveFileSettings.FolderName, fileName);
            await using var fileStream = File.OpenWrite(filePath);
            await stream.CopyToAsync(fileStream, cancellationToken);

            await fileStream.FlushAsync(cancellationToken);

            return filePath;
        }

        public async Task<string> CalculateHashAsync(Stream stream, CancellationToken cancellationToken)
        {
            var md5 = MD5.Create();
            var hash = await md5.ComputeHashAsync(stream, cancellationToken);
            var base64String = Convert.ToBase64String(hash);

            return base64String;
        }
    }
}