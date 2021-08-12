using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Frends.Community.Azure.Tests
{
    public class ListBlobsTests
    {
        /// <summary>
        /// Connection string for Azure Storage Emulator
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AZUREBLOBSTORAGE_CONNSTRING", EnvironmentVariableTarget.User);

        /// <summary>
        /// Test blob name
        /// </summary>
        private readonly string _testBlob = "test-blob.txt";

        /// <summary>
        /// Container name for tests
        /// </summary>
        private string _containerName;

        private ListBlobsSourceProperties _sourceProperties;

        /// <summary>
        /// Some random file for test purposes
        /// </summary>
        private readonly string _testFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TestFiles\TestFile.xml";

        [SetUp]
        public async Task Setup()
        {
            // Generate unique container name to avoid conflicts when running multiple tests
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

            _sourceProperties = new ListBlobsSourceProperties
            {
                ConnectionString = _connectionString,
                ContainerName = _containerName,
                FlatBlobListing = false
            };

            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.CreateIfNotExistsAsync();

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(_testBlob);
            await blockBlob.UploadFromFileAsync(_testFilePath);
            var blobWithDir = container.GetBlockBlobReference("directory/test-blob2.txt");
            await blobWithDir.UploadFromFileAsync(_testFilePath);
        }

        [TearDown]
        public async Task Cleanup()
        {
            // delete whole container after running tests
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync();
        }

        [Test]
        public void ListBlobs_ReturnBlockAndDirectory()
        {
            var result = AzureBlobTasks.ListBlobs(_sourceProperties);

            Assert.AreEqual(2, result.Blobs.Count);
            Assert.AreEqual("Block", result.Blobs[1].BlobType);
            Assert.AreEqual(_testBlob, result.Blobs[1].Name);

            Assert.AreEqual("Directory", result.Blobs[0].BlobType);
        }

        [Test]
        public void ListBlobsWithPrefix()
        {
            _sourceProperties.FlatBlobListing = true;
            var result = AzureBlobTasks.ListBlobs(_sourceProperties);

            Assert.AreEqual(2, result.Blobs.Count);
        }
    }
}