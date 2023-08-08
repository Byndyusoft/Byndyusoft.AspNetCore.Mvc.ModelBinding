using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceTests.Files;
using PerformanceTests.Helpers;

namespace PerformanceTests.Tests
{
    [SimpleJob(RunStrategy.Throughput)]
    [AllStatisticsColumn]
    public class UploadToStorageTest
    {
        private BenchmarkTestInstance _benchmarkTestInstance = default!;

        [GlobalSetup]
        public void Setup()
        {
            FileGenerator.GenerateFiles(FileGeneratorSetting.GetDefault(TestFileSize));
            _benchmarkTestInstance = new BenchmarkTestInstance("Files/Upload");
        }

        [Params(TestFileSize.Small, TestFileSize.Big, TestFileSize.Large)]
        public TestFileSize TestFileSize = TestFileSize.None;

        [Benchmark]
        public async Task<string[]> HashOld()
        {
            return await _benchmarkTestInstance.TestOldWay<string[]>(TestFileSize);
        }

        [Benchmark]
        public async Task<string[]> HashNew()
        {
            return await _benchmarkTestInstance.TestNewWay<string[]>(TestFileSize);
        }
    }
}