using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceTests.Consts;
using PerformanceTests.Helpers;

namespace PerformanceTests.Tests
{
    [SimpleJob(RunStrategy.Monitoring, iterationCount: 20)]
    [AllStatisticsColumn]
    public class HashTest
    {
        private BenchmarkTestInstance _benchmarkTestInstance = default!;

        [GlobalSetup]
        public void Setup()
        {
            FileGenerator.GenerateFiles(FileGeneratorSetting.GetDefaultOptions());
            _benchmarkTestInstance = new BenchmarkTestInstance("Files/Hash");
        }

        [Params(FolderNames.Small, FolderNames.Big, FolderNames.Large)] 
        public string Subfolder = default!;

        [Benchmark]
        public async Task<string[]> HashOld()
        {
            return await _benchmarkTestInstance.TestOldWay<string[]>(Subfolder);
        }

        [Benchmark]
        public async Task<string[]> HashNew()
        {
            return await _benchmarkTestInstance.TestNewWay<string[]>(Subfolder);
        }
    }
}