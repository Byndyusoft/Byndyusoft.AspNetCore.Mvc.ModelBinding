using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Byndyusoft.IntegrationTests;

namespace PerformanceTests.Helpers
{
    public class BenchmarkTestHelper
    {
        public static HttpClient GetHttpClient()
        {
            var apiFixture = new ApiFixture();
            return apiFixture.CreateClient();
        }

        public static string GetDefaultFolderBasePath() => "D:\\PerformanceTestFiles";

        public static async Task<T> CallApiMethod<T>(
            HttpClient httpClient, 
            string postMethodPrefix, 
            string folderPath,
            bool isNew)
        {
            using var multipartFormDataContent = GetMultipartFormDataContent(folderPath);
            var postMethodSuffix = isNew ? "New" : "Old";
            var postMethodName = $"{postMethodPrefix}{postMethodSuffix}";
            using var httpResponseMessage = await httpClient.PostAsync(postMethodName, multipartFormDataContent);
            var result = await httpResponseMessage.Content.ReadFromJsonAsync<T>();
            return result!;
        }

        private static MultipartFormDataContent GetMultipartFormDataContent(string folderPath)
        {
            var multipartFormDataContent = new MultipartFormDataContent
            {
                { new StringContent("Ivan"), "Name" },
                { new StringContent("35"), "Age" }
            };

            foreach (var (fileName, content) in CreateStreamContents(folderPath))
                multipartFormDataContent.Add(content, "Files", fileName);
            return multipartFormDataContent;
        }

        private static IEnumerable<(string FileName, StreamContent Content)> CreateStreamContents(string folderPath)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
                yield return (fileInfo.Name, TestHelper.CreateStreamContentFromFilePath(fileInfo.FullName));
        }
    }
}