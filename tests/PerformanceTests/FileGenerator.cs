using System;
using System.IO;
using System.Security.Cryptography;
using PerformanceTests.Consts;

namespace PerformanceTests
{
    public static class FileGenerator
    {
        const int BlockSize = 1024 * 8;
        const int BlocksPerMb = 1024 * 1024 / BlockSize;
        private static bool _generated = false;

        public static void GenerateFiles(params FileGeneratorSetting[] settings)
        {
            if (_generated)
            {
                Console.WriteLine("Files were already generated");
                return;
            }

            Console.WriteLine("Generating new files");

            foreach (var fileGeneratorSettings in settings) 
                GenerateFiles(fileGeneratorSettings);
            _generated = true;

            Console.WriteLine("Files were generated");
        }

        private static void GenerateFiles(FileGeneratorSetting setting)
        {
            var subfolderPath = Path.Combine(FolderNames.BaseFolderName, setting.Subfolder);
            Directory.CreateDirectory(subfolderPath);

            for (var i = 1; i <= setting.FileCount; i++)
            {
                var fileName = $"{setting.Subfolder}_{i}.bin";
                var filePath = Path.Combine(subfolderPath, fileName);
                GenerateFile(filePath, setting.FileSizeInMb);
            }
        }

        private static void GenerateFile(string filePath, int sizeInMb)
        {
            byte[] data = new byte[BlockSize];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            using FileStream stream = File.OpenWrite(filePath);

            for (int i = 0; i < sizeInMb * BlocksPerMb; i++)
            {
                randomNumberGenerator.GetBytes(data);
                stream.Write(data, 0, data.Length);
            }
        }
    }

    public class FileGeneratorSetting
    {
        public static FileGeneratorSetting[] GetDefaultOptions() => new FileGeneratorSetting[]
        {
            new(fileSizeInMb: 1, fileCount: 5, FolderNames.Small),
            new(fileSizeInMb: 100, fileCount: 2, FolderNames.Big),
            new(fileSizeInMb: 1000, fileCount: 1, FolderNames.Large)
        };

        public FileGeneratorSetting(int fileSizeInMb, int fileCount, string subfolder)
        {
            if (fileSizeInMb < 1 || fileCount < 1)
                throw new ArgumentException("Parameters must be positive");
            FileSizeInMb = fileSizeInMb;
            FileCount = fileCount;
            Subfolder = subfolder;
        }

        public int FileSizeInMb { get; }

        public int FileCount { get; }

        public string Subfolder { get; }
    }
}