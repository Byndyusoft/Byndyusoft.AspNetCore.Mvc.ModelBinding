using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Byndyusoft.TestApi.Dtos;
using FluentAssertions;
using Xunit;

namespace Byndyusoft.IntegrationTests.Tests
{
    public class FilesControllerTests : IDisposable
    {
        private const string FirstFileName = "FirstFile";
        private const string SecondFileName = "SecondFile";
        private readonly HttpClient _httpClient;
        private readonly ApiFixture _apiFixture;

        public FilesControllerTests()
        {
            _apiFixture = new ApiFixture();
            _httpClient = _apiFixture.CreateClient();
        }

        [Fact]
        public async Task BindToModel_FirstValuesThenFiles_ModelIsBoundCorrectly_AndFileContentsAreCorrect()
        {
            // Arrange
            using var multipartFormDataContent = new MultipartFormDataContent
            {
                { new StringContent("Ivan"), "Name" },
                { new StringContent("35"), "Age" }
            };

            using var firstStreamContent = TestHelper.CreateStreamContent(FileNames.FirstFile);
            multipartFormDataContent.Add(firstStreamContent, FirstFileName, FileNames.FirstFile);

            using var secondStreamContent = TestHelper.CreateStreamContent(FileNames.SecondFile);
            multipartFormDataContent.Add(secondStreamContent, SecondFileName, FileNames.SecondFile);

            // Act
            using var httpResponseMessage = await _httpClient.PostAsync("Files/SaveNew", multipartFormDataContent);

            // Assert
            httpResponseMessage.EnsureSuccessStatusCode();
            var saveResultDto = await httpResponseMessage.Content.ReadFromJsonAsync<SaveResultDto>();

            var firstFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.FirstFile);
            var secondFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.SecondFile);

            var expectedResultDto = new SaveResultDto
            {
                Name = "Ivan",
                Age = 35,
                Files = new[]
                {
                    new FileResultDto
                    {
                        Name = FirstFileName,
                        FileName = FileNames.FirstFile,
                        ContentLength = firstFileBytes.Length
                    },
                    new FileResultDto
                    {
                        Name = SecondFileName,
                        FileName = FileNames.SecondFile,
                        ContentLength = secondFileBytes.Length
                    }
                }
            };
            saveResultDto.Should().BeEquivalentTo(expectedResultDto,
                o => o.Excluding(mi =>
                    mi.DeclaringType == typeof(FileResultDto) && mi.Name == nameof(FileResultDto.FilePath)));

            var firstFilePath = saveResultDto!.Files.Single(i => i.Name == FirstFileName).FilePath;
            await AssertFileContentsAsync(firstFilePath, firstFileBytes);

            var secondFilePath = saveResultDto.Files.Single(i => i.Name == SecondFileName).FilePath;
            await AssertFileContentsAsync(secondFilePath, secondFileBytes);
        }

        [Fact]
        public async Task BindToModel_ThereAreValuesAfterFiles_ThrowsException()
        {
            // Arrange
            using var multipartFormDataContent = new MultipartFormDataContent
            {
                { new StringContent("Ivan"), "Name" }
            };

            using var firstStreamContent = TestHelper.CreateStreamContent(FileNames.FirstFile);
            multipartFormDataContent.Add(firstStreamContent, FirstFileName, FileNames.FirstFile);

            using var secondStreamContent = TestHelper.CreateStreamContent(FileNames.SecondFile);
            multipartFormDataContent.Add(secondStreamContent, SecondFileName, FileNames.SecondFile);

            multipartFormDataContent.Add(new StringContent("35"), "Age");

            // Act
            // ReSharper disable once AccessToDisposedClosure
            Func<Task<HttpResponseMessage>> action = () => _httpClient.PostAsync("Files/SaveNew", multipartFormDataContent);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task BindToParameter_OnlyFiles_ParameterIsBoundCorrectly_AndFileContentsAreCorrect()
        {
            // Arrange
            using var multipartFormDataContent = new MultipartFormDataContent();

            using var firstStreamContent = TestHelper.CreateStreamContent(FileNames.FirstFile);
            multipartFormDataContent.Add(firstStreamContent, FirstFileName, FileNames.FirstFile);

            using var secondStreamContent = TestHelper.CreateStreamContent(FileNames.SecondFile);
            multipartFormDataContent.Add(secondStreamContent, SecondFileName, FileNames.SecondFile);

            // Act
            using var httpResponseMessage = await _httpClient.PostAsync("Files/SaveNewByParameter", multipartFormDataContent);

            // Assert
            httpResponseMessage.EnsureSuccessStatusCode();
            var fileResultDtos = await httpResponseMessage.Content.ReadFromJsonAsync<FileResultDto[]>();

            var firstFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.FirstFile);
            var secondFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.SecondFile);

            var expectedResultDto = new FileResultDto[]
            {
                new()
                {
                    Name = FirstFileName,
                    FileName = FileNames.FirstFile,
                    ContentLength = firstFileBytes.Length
                },
                new()
                {
                    Name = SecondFileName,
                    FileName = FileNames.SecondFile,
                    ContentLength = secondFileBytes.Length
                }
            };

            fileResultDtos.Should().BeEquivalentTo(expectedResultDto,
                o => o.Excluding(mi =>
                    mi.DeclaringType == typeof(FileResultDto) && mi.Name == nameof(FileResultDto.FilePath)));

            var firstFilePath = fileResultDtos!.Single(i => i.Name == FirstFileName).FilePath;
            await AssertFileContentsAsync(firstFilePath, firstFileBytes);

            var secondFilePath = fileResultDtos!.Single(i => i.Name == SecondFileName).FilePath;
            await AssertFileContentsAsync(secondFilePath, secondFileBytes);
        }

        [Fact]
        public async Task BindToParameter_AndStreamsAreUsedIncorrectly_OnlyFiles_ParameterIsBoundCorrectly_AndFilesAreEmpty()
        {
            // Arrange
            using var multipartFormDataContent = new MultipartFormDataContent();

            using var firstStreamContent = TestHelper.CreateStreamContent(FileNames.FirstFile);
            multipartFormDataContent.Add(firstStreamContent, FirstFileName, FileNames.FirstFile);

            using var secondStreamContent = TestHelper.CreateStreamContent(FileNames.SecondFile);
            multipartFormDataContent.Add(secondStreamContent, SecondFileName, FileNames.SecondFile);

            // Act
            using var httpResponseMessage = await _httpClient.PostAsync("Files/SaveNewIncorrectly", multipartFormDataContent);

            // Assert
            httpResponseMessage.EnsureSuccessStatusCode();
            var fileResultDtos = await httpResponseMessage.Content.ReadFromJsonAsync<FileResultDto[]>();

            var firstFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.FirstFile);
            var secondFileBytes = await TestHelper.ReadFileBytesAsync(FileNames.SecondFile);

            var expectedResultDto = new FileResultDto[]
            {
                new()
                {
                    Name = FirstFileName,
                    FileName = FileNames.FirstFile,
                    ContentLength = firstFileBytes.Length
                },
                new()
                {
                    Name = SecondFileName,
                    FileName = FileNames.SecondFile,
                    ContentLength = secondFileBytes.Length
                }
            };

            fileResultDtos.Should().BeEquivalentTo(expectedResultDto,
                o => o.Excluding(mi =>
                    mi.DeclaringType == typeof(FileResultDto) && mi.Name == nameof(FileResultDto.FilePath)));

            var emptyFileContents = Array.Empty<byte>();

            var firstFilePath = fileResultDtos!.Single(i => i.Name == FirstFileName).FilePath;
            await AssertFileContentsAsync(firstFilePath, emptyFileContents);

            var secondFilePath = fileResultDtos!.Single(i => i.Name == SecondFileName).FilePath;
            await AssertFileContentsAsync(secondFilePath, emptyFileContents);
        }

        private async Task AssertFileContentsAsync(string filePath, byte[] expectedFileBytes)
        {
            var actualFileBytes = await File.ReadAllBytesAsync(filePath);
            actualFileBytes.Should().BeEquivalentTo(expectedFileBytes, o => o.WithStrictOrdering());
        }

        public void Dispose()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(_apiFixture.SaveFileSettings.FolderName);
                if (directoryInfo.Exists is false)
                    return;

                foreach (var fileInfo in directoryInfo.EnumerateFiles()) 
                    fileInfo.Delete();
            }
            catch
            {
                // ignored
            }
        }
    }
}