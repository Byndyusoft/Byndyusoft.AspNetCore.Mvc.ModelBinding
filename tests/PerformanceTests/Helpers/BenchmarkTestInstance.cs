using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerformanceTests.Helpers
{
    public class BenchmarkTestInstance
    {
        private readonly HttpClient _httpClient;
        private readonly string _postMethodPrefix;
        private readonly string _folderBasePath = BenchmarkTestHelper.GetDefaultFolderBasePath();

        public BenchmarkTestInstance(string postMethodPrefix)
        {
            _httpClient = BenchmarkTestHelper.GetHttpClient();
            _postMethodPrefix = postMethodPrefix;
        }
        
        public async Task<T> TestOldWay<T>(string subfolder)
        {
            return await BenchmarkTestHelper.CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                GetFolderPath(subfolder),
                isNew: false);
        }

        public async Task<T> TestNewWay<T>(string subfolder)
        {
            return await BenchmarkTestHelper.CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                GetFolderPath(subfolder),
                isNew: true);
        }

        private string GetFolderPath(string subfolder)
        {
            return Path.Combine(_folderBasePath, subfolder);
        }
    }
}