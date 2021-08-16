using Microsoft.Azure.Storage.Blob;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Azure.Tests
{
    [TestFixture]
    [Ignore("Unit tests requires Azure Storage Emulator running")]
    public class UploadBlobsTests
    {
        /// <summary>
        /// Connection string for Azure Storage Emulator
        /// </summary>
        private readonly string _connectionString = "UseDevelopmentStorage=true";

        /// <summary>
        /// Container name for tests
        /// </summary>
        private string _containerName;

        /// <summary>
        /// Some random file for test purposes
        /// </summary>
        private readonly string _testFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TestFiles\TestFile.xml";

        [SetUp]
        public void TestSetup()
        {
            // Generate unique container name to avoid conflicts when running multiple tests
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        }

        [TearDown]
        public async Task Cleanup()
        {
            // delete whole container after running tests
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync();
        }

        [Test]
        public void UploadFileAsync_ShouldThrowArgumentExceptionIfFileWasNotFound()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await DelegatedUploadFileAsync_ShouldThrowArgumentExceptionIfFileWasNotFound());
        }

        private async Task DelegatedUploadFileAsync_ShouldThrowArgumentExceptionIfFileWasNotFound()
        {
            await AzureBlobTasks.UploadFileAsync(
                new UploadBlobsInput { SourceFile = "NonExistingFile" },
                new UploadBlobsDestinationProperties(),
                new CancellationToken());
        }

        [Test]
        public async Task UploadFileAsync_ShouldUploadFileAsBlockBlob()
        {
            var input = new UploadBlobsInput
            {
                SourceFile = _testFilePath
            };
            var options = new UploadBlobsDestinationProperties
            {
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = true,
                CreateContainerIfItDoesNotExist = true
            };
            var container = Utils.GetBlobContainer(_connectionString, _containerName);

            var result = await AzureBlobTasks.UploadFileAsync(input, options, new CancellationToken());
            var blobResult = Utils.GetCloudBlob(container, "TestFile.xml", AzureBlobType.Block);

            StringAssert.EndsWith($"{_containerName}/TestFile.xml", result.Uri);
            Assert.IsTrue(blobResult.Exists(), "Uploaded TestFile.xml blob should exist");
        }

        [Test]
        public async Task UploadFileAsync_ShouldRenameFileToBlob()
        {
            var input = new UploadBlobsInput
            {
                SourceFile = _testFilePath
            };
            var options = new UploadBlobsDestinationProperties
            {
                RenameTo = "RenamedFile.xml",
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = true,
                CreateContainerIfItDoesNotExist = true
            };

            var result = await AzureBlobTasks.UploadFileAsync(input, options, new CancellationToken());

            StringAssert.EndsWith($"{_containerName}/RenamedFile.xml", result.Uri);
        }

        [Test]
        public async Task UploadFileAsync_ShouldUploadCompressedFile()
        {
            var input = new UploadBlobsInput
            {
                SourceFile = _testFilePath,
                Compress = true,
                ContentsOnly = true
            };

            var guid = Guid.NewGuid().ToString();
            var renameTo = guid + ".gz";

            var options = new UploadBlobsDestinationProperties
            {
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = false,
                CreateContainerIfItDoesNotExist = true,
                ContentType = "text/xml",
                FileEncoding = "utf8",
                RenameTo = renameTo
            };
            var container = Utils.GetBlobContainer(_connectionString, _containerName);

            await AzureBlobTasks.UploadFileAsync(input, options, new CancellationToken());
            var blobResult = Utils.GetCloudBlob(container, renameTo, AzureBlobType.Block);

            Assert.IsTrue(blobResult.Exists(), "Uploaded TestFile.xml blob should exist");
        }

        [Test]
        public async Task UploadFileAsync_ContentTypeIsForcedProperly()
        {
            var input = new UploadBlobsInput
            {
                SourceFile = _testFilePath,
                Compress = false,
                ContentsOnly = false
            };

            var guid = Guid.NewGuid().ToString();
            var renameTo = guid + ".gz";

            var options = new UploadBlobsDestinationProperties
            {
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = false,
                CreateContainerIfItDoesNotExist = true,
                ContentType = "foo/bar",
                FileEncoding = "utf8",
                RenameTo = renameTo
            };
            var container = Utils.GetBlobContainer(_connectionString, _containerName);

            await AzureBlobTasks.UploadFileAsync(input, options, new CancellationToken());
            var blobResult = (CloudBlockBlob)Utils.GetCloudBlob(container, renameTo, AzureBlobType.Block);
            await blobResult.FetchAttributesAsync();

            Assert.IsTrue(blobResult.Properties.ContentType == "foo/bar");
        }

        [Test]
        public async Task UploadFileAsync_ContentEncodingIsGzipWhenCompressed()
        {
            var input = new UploadBlobsInput
            {
                SourceFile = _testFilePath,
                Compress = true,
                ContentsOnly = true
            };

            var guid = Guid.NewGuid().ToString();
            var renameTo = guid + ".gz";

            var options = new UploadBlobsDestinationProperties
            {
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = false,
                CreateContainerIfItDoesNotExist = true,
                ContentType = "foo/bar",
                FileEncoding = "utf8",
                RenameTo = renameTo
            };
            var container = Utils.GetBlobContainer(_connectionString, _containerName);

            await AzureBlobTasks.UploadFileAsync(input, options, new CancellationToken());
            var blobResult = (CloudBlockBlob)Utils.GetCloudBlob(container, renameTo, AzureBlobType.Block);
            await blobResult.FetchAttributesAsync();

            Assert.IsTrue(blobResult.Properties.ContentEncoding == "gzip");
        }
    }
}