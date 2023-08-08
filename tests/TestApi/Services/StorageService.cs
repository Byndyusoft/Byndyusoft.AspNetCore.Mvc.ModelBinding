using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Minio;

namespace Byndyusoft.TestApi.Services
{
    public class StorageService
    {
        private const string Endpoint = "localhost:9000";
        private const string AccessKey = "minioadmin";
        private const string SecretKey = "minioadmin";
        private const string BucketName = "model-binding";

        public async Task<string> UploadStreamAsync(Stream stream, string fileName, long fileSize,
            CancellationToken cancellationToken)
        {
            using var minioClient = GetMinioClient();
            await CreateBucketAsync(minioClient, cancellationToken);
            return await UploadStreamAsync(minioClient, stream, fileName, fileSize, cancellationToken);
        }

        private MinioClient GetMinioClient()
        {
            return new MinioClient()
                .WithEndpoint(Endpoint)
                .WithCredentials(AccessKey, SecretKey)
                .WithSSL(false)
                .Build();
        }

        private async Task CreateBucketAsync(MinioClient minioClient, CancellationToken cancellationToken)
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(BucketName);
            if (await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken))
                return;

            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(BucketName);
            await minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        private async Task<string> UploadStreamAsync(MinioClient minioClient, Stream stream, string fileName,
            long fileSize, CancellationToken cancellationToken)
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(fileName)
                .WithObjectSize(fileSize)
                .WithStreamData(stream);
            var putObjectResponse = await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            return putObjectResponse.ObjectName;
        }
    }
}