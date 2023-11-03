using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PerformanceTests.Files
{
    public static class FileGenerator
    {
        private static readonly Dictionary<TestFileSize, TestFileInfo[]> TestFilesBySize = new();

        public static void GenerateFiles(params FileGeneratorSetting[] settings)
        {
            foreach (var fileGeneratorSettings in settings)
            {
                if (TestFilesBySize.ContainsKey(fileGeneratorSettings.FileSize) == false)
                    GenerateFiles(fileGeneratorSettings);
            }
        }

        public static IEnumerable<TestFileInfo> GetTestFiles(TestFileSize testFileSize)
        {
            return TestFilesBySize[testFileSize];
        }

        private static void GenerateFiles(FileGeneratorSetting setting)
        {
            var testFiles = new List<TestFileInfo>();

            for (var i = 1; i <= setting.FileCount; i++)
            {
                var fileName = $"{setting.FileSize}_{i}.bin";
                var testFileInfo = GenerateFile(fileName, setting.FileSizeInMb);
                testFiles.Add(testFileInfo);
            }

            TestFilesBySize.Add(setting.FileSize, testFiles.ToArray());
            Console.WriteLine($"{setting.FileSize} file contents were generated");
        }

        private static TestFileInfo GenerateFile(string fileName, int sizeInMb)
        {
            const int bytesInMb = 1024 * 1024;
            var data = new byte[sizeInMb * bytesInMb];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(data);

            return new TestFileInfo(data, fileName);
        }
    }
}