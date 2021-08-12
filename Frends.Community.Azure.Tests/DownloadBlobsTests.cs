using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Azure.Tests
{
    public class DownloadBlobsTests
    {
        /// <summary>
        /// Connection string for Azure Storage Emulator
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AZUREBLOBSTORAGE_CONNSTRING");

        /// <summary>
        /// Some random file for test purposes
        /// </summary>
        private readonly string _testBlob = "test-blob.txt";

        private CancellationToken _cancellationToken;

        /// <summary>
        /// Container name for tests
        /// </summary>
        private string _containerName;

        private DownloadBlobDestinationFileProperties _destination;

        private string _destinationDirectory;

        private DownloadBlobSourceProperties _source;

        /// <summary>
        /// Some random file for test purposes
        /// </summary>
        private readonly string _testFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TestFiles\TestFile.xml";

        [SetUp]
        public async Task Setup()
        {
            _destinationDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_destinationDirectory);

            // Generate unique container name to avoid conflicts when running multiple tests
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

            // task properties
            _source = new DownloadBlobSourceProperties
            {
                ConnectionString = _connectionString,
                BlobName = _testBlob,
                BlobType = AzureBlobType.Block,
                ContainerName = _containerName
            };
            _destination = new DownloadBlobDestinationFileProperties
            {
                Directory = _destinationDirectory,
                FileExistsOperation = FileExistsAction.Overwrite
            };
            _cancellationToken = new CancellationToken();

            // setup test material for download tasks

            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            var success = await container.CreateIfNotExistsAsync(_cancellationToken);

            if (!success)
                throw new Exception("Could no create blob container");

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(_testBlob);

            await blockBlob.UploadFromFileAsync(_testFilePath, _cancellationToken);
        }

        [TearDown]
        public async Task Cleanup()
        {
            // delete whole container after running tests
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync(_cancellationToken);

            // delete test files and folders
            if (Directory.Exists(_destinationDirectory))
                Directory.Delete(_destinationDirectory, true);
        }

        [Test]
        public void DownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
        {
            Assert.ThrowsAsync<IOException>(async () => await DelegateDownloadBlobAsync_ThrowsExceptionIfDestinationFileExists());
        }

        private async Task DelegateDownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
        {
            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);
            _destination.FileExistsOperation = FileExistsAction.Error;

            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);
        }

        [Test]
        public async Task ReadBlobContentAsync_ReturnsContentString()
        {
            var result = await AzureBlobTasks.ReadBlobContentAsync(_source, _cancellationToken);

            Assert.IsTrue(result.Content.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
        }

        [Test]
        public async Task DownloadBlobAsync_WritesBlobToFile()
        {
            var result = await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);

            Assert.IsTrue(File.Exists(result.FullPath));
            var fileContent = File.ReadAllText(result.FullPath);
            Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
        }

        [Test]
        public async Task DownloadBlobAsync_RenamesFileIfExists()
        {
            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);
            _destination.FileExistsOperation = FileExistsAction.Rename;

            var result = await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);

            Assert.AreEqual("test-blob(1).txt", result.FileName);
        }

        [Test]
        public async Task DownloadBlobAsync_OverwritesFileIfExists()
        {
            // download file with same name couple of time
            _destination.FileExistsOperation = FileExistsAction.Overwrite;
            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);
            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);
            await AzureBlobTasks.DownloadBlobAsync(_source, _destination, _cancellationToken);

            // only one file should exist in destination folder
            Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
        }
    }
}