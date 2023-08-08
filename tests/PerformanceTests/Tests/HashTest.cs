using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PerformanceTests.Helpers;

namespace PerformanceTests.Tests
{
    public class HashTest
    {
        private const string PostMethodPrefix = "Files/Hash";
        private readonly string _folderPath = BenchmarkTestHelper.GetDefaultFolderPath();
        private HttpClient _httpClient = default!;

        [GlobalSetup]
        public void Setup()
        {
            _httpClient = BenchmarkTestHelper.GetHttpClient();
        }

        [Benchmark]
        public async Task<string[]> HashOld()
        {
            return await BenchmarkTestHelper.CallApiMethod<string[]>(
                _httpClient, 
                PostMethodPrefix, 
                _folderPath, 
                isNew: false);
        }

        [Benchmark]
        public async Task<string[]> HashNew()
        {
            return await BenchmarkTestHelper.CallApiMethod<string[]>(
                _httpClient, 
                PostMethodPrefix,
                folderPath: _folderPath, 
                isNew: true);
        }
    }
}