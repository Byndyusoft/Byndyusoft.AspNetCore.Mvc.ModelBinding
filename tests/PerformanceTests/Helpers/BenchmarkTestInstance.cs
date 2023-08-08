using System.Net.Http;
using System.Threading.Tasks;
using PerformanceTests.Files;

namespace PerformanceTests.Helpers
{
    public class BenchmarkTestInstance
    {
        private readonly HttpClient _httpClient;
        private readonly string _postMethodPrefix;

        public BenchmarkTestInstance(string postMethodPrefix)
        {
            _httpClient = BenchmarkTestHelper.GetHttpClient();
            _postMethodPrefix = postMethodPrefix;
        }
        
        public async Task<T> TestOldWay<T>(TestFileSize testFileSize)
        {
            return await BenchmarkTestHelper.CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                testFileSize,
                isNew: false);
        }

        public async Task<T> TestNewWay<T>(TestFileSize testFileSize)
        {
            return await BenchmarkTestHelper.CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                testFileSize,
                isNew: true);
        }
    }
}