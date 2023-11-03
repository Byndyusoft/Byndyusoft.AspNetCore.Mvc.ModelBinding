using System;
using System.Linq;

namespace PerformanceTests.Files
{
    public class FileGeneratorSetting
    {
        public FileGeneratorSetting(int fileSizeInMb, int fileCount, TestFileSize fileSize)
        {
            if (fileSizeInMb < 1 || fileCount < 1)
                throw new ArgumentException("Parameters must be positive");
            FileSizeInMb = fileSizeInMb;
            FileCount = fileCount;
            FileSize = fileSize;
        }

        public int FileSizeInMb { get; }

        public int FileCount { get; }

        public TestFileSize FileSize { get; }

        public static FileGeneratorSetting[] GetDefault() => new FileGeneratorSetting[]
        {
            new(fileSizeInMb: 1, fileCount: 5, TestFileSize.Small),
            new(fileSizeInMb: 100, fileCount: 2, TestFileSize.Big),
            new(fileSizeInMb: 1000, fileCount: 1, TestFileSize.Large)
        };

        public static FileGeneratorSetting GetDefault(TestFileSize testFileSize) =>
            GetDefault().Single(i => i.FileSize == testFileSize);
    }
}