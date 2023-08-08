using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Byndyusoft.IntegrationTests;
using PerformanceTests.Files;

namespace PerformanceTests.Helpers
{
    public class BenchmarkTestInstance
    {
        private readonly HttpClient _httpClient;
        private readonly string _postMethodPrefix;

        public BenchmarkTestInstance(string postMethodPrefix)
        {
            _httpClient = GetHttpClient();
            _postMethodPrefix = postMethodPrefix;
        }
        
        public async Task<T> TestOldWay<T>(TestFileSize testFileSize)
        {
            return await CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                testFileSize,
                isNew: false);
        }

        public async Task<T> TestNewWay<T>(TestFileSize testFileSize)
        {
            return await CallApiMethod<T>(
                _httpClient,
                _postMethodPrefix,
                testFileSize,
                isNew: true);
        }

        public static HttpClient GetHttpClient()
        {
            var apiFixture = new ApiFixture();
            return apiFixture.CreateClient();
        }

        public static async Task<T> CallApiMethod<T>(
            HttpClient httpClient,
            string postMethodPrefix,
            TestFileSize testFileSize,
            bool isNew)
        {
            using var multipartFormDataContent = GetMultipartFormDataContent(testFileSize);
            var postMethodSuffix = isNew ? "New" : "Old";
            var postMethodName = $"{postMethodPrefix}{postMethodSuffix}";
            using var httpResponseMessage = await httpClient.PostAsync(postMethodName, multipartFormDataContent);
            var result = await httpResponseMessage.Content.ReadFromJsonAsync<T>();
            return result!;
        }

        private static MultipartFormDataContent GetMultipartFormDataContent(TestFileSize testFileSize)
        {
            var multipartFormDataContent = new MultipartFormDataContent
            {
                { new StringContent("Ivan"), "Name" },
                { new StringContent("35"), "Age" }
            };

            foreach (var testFileInfo in FileGenerator.GetTestFiles(testFileSize))
            {
                var streamContent = CreateStreamContent(testFileInfo.Content);
                multipartFormDataContent.Add(streamContent, "Files", testFileInfo.FileName);
            }

            return multipartFormDataContent;
        }

        public static StreamContent CreateStreamContent(byte[] fileContent)
        {
            var memoryStream = new MemoryStream(fileContent);
            var streamContent = new StreamContent(memoryStream);

            streamContent.Headers.ContentLength = fileContent.Length;

            return streamContent;
        }
    }
}