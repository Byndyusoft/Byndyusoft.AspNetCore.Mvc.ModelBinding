using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Byndyusoft.TestApi.Dtos;
using PerformanceTests.Files;
using PerformanceTests.Helpers;

namespace PerformanceTests.Tests
{
    [SimpleJob(RunStrategy.Throughput)]
    [AllStatisticsColumn]
    public class SaveToDiskTest
    {
        private BenchmarkTestInstance _benchmarkTestInstance = default!;

        [GlobalSetup]
        public void Setup()
        {
            FileGenerator.GenerateFiles(FileGeneratorSetting.GetDefault(TestFileSize));
            _benchmarkTestInstance = new BenchmarkTestInstance("Files/Save");
        }

        [Params(TestFileSize.Small, TestFileSize.Big, TestFileSize.Large)]
        public TestFileSize TestFileSize = TestFileSize.None;

        [Benchmark]
        public async Task<SaveResultDto> SaveOld()
        {
            return await _benchmarkTestInstance.TestOldWay<SaveResultDto>(TestFileSize);
        }

        [Benchmark]
        public async Task<SaveResultDto> SaveNew()
        {
            return await _benchmarkTestInstance.TestNewWay<SaveResultDto>(TestFileSize);
        }
    }
}