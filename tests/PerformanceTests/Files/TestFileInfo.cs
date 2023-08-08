namespace PerformanceTests.Files
{
    public class TestFileInfo
    {
        public TestFileInfo(byte[] content, string fileName)
        {
            Content = content;
            FileName = fileName;
        }

        public byte[] Content { get; }

        public string FileName { get; }
    }
}