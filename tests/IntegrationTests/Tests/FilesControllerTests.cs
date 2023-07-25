using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Byndyusoft.Example.Dtos;
using FluentAssertions;
using Xunit;

namespace Byndyusoft.IntegrationTests.Tests
{
    public class FilesControllerTests
    {
        private readonly HttpClient _httpClient;

        public FilesControllerTests()
        {
            var apiFixture = new ApiFixture();
            _httpClient = apiFixture.CreateClient();
        }

        [Fact]
        public async Task Test()
        {
            // Arrange
            using var multipartFormDataContent = new MultipartFormDataContent();

            multipartFormDataContent.Add(new StringContent("Ivan"), "Name");
            multipartFormDataContent.Add(new StringContent("35"), "Age");

            using var firstStreamContent = Helper.CreateStreamContent(FileNames.FirstFile);
            multipartFormDataContent.Add(firstStreamContent, "FirstFile", FileNames.FirstFile);

            using var secondStreamContent = Helper.CreateStreamContent(FileNames.SecondFile);
            multipartFormDataContent.Add(secondStreamContent, "SecondFile", FileNames.SecondFile);

            // Act
            using var httpResponseMessage = await _httpClient.PostAsync("Files/SaveNew", multipartFormDataContent);

            // Assert
            httpResponseMessage.EnsureSuccessStatusCode();
            var saveResultDto = await httpResponseMessage.Content.ReadFromJsonAsync<SaveResultDto>();

            var firstFileBytes = await Helper.ReadFileBytesAsync(FileNames.FirstFile);
            var secondFileBytes = await Helper.ReadFileBytesAsync(FileNames.SecondFile);

            var expectedResultDto = new SaveResultDto
            {
                Name = "Ivan",
                Age = 35,
                Files = new[]
                {
                    new FileResultDto
                    {
                        Name = "FirstFile",
                        FileName = FileNames.FirstFile,
                        ContentLength = firstFileBytes.Length
                    },
                    new FileResultDto
                    {
                        Name = "SecondFile",
                        FileName = FileNames.SecondFile,
                        ContentLength = secondFileBytes.Length
                    }
                }
            };
            saveResultDto.Should().BeEquivalentTo(expectedResultDto,
                o => o.Excluding(mi =>
                    mi.DeclaringType == typeof(FileResultDto) && mi.Name == nameof(FileResultDto.FilePath)));
        }
    }

    public class FileNames
    {
        public const string FirstFile = "FirstPicture.jpg";

        public const string SecondFile = "SecondPicture.jpg";
    }

    public class Helper
    {
        public static StreamContent CreateStreamContent(string fileName)
        {
            var filePath = GetFilePath(fileName);
            var fileInfo = new FileInfo(filePath);
            var fileStream = fileInfo.OpenRead();

            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentLength = fileInfo.Length;

            return streamContent;
        }

        public static async Task<byte[]> ReadFileBytesAsync(string fileName)
        {
            return await File.ReadAllBytesAsync(GetFilePath(fileName));
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine("TestFiles", fileName);
        }
    }
}