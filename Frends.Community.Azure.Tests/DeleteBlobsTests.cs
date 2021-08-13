using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Azure.Tests
{
    [TestFixture]
    [Ignore("Unit tests requires Azure Storage Emulator running")]
    public class DeleteBlobsTests
    {
        /// <summary>
        /// Connection string for Azure Storage Emulator
        /// </summary>
        private readonly string _connectionString = "UseDevelopmentStorage=true;";

        /// <summary>
        /// Container name for tests
        /// </summary>
        private readonly string _containerName = "test-container";

        [TearDown]
        public async Task Cleanup()
        {
            // delete whole container after running tests
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync();
        }

        [Test]
        public async Task DeleteBlobAsync_ShouldReturnTrueWithNonexistingBlob()
        {
            var input = new DeleteBlobProperties
            {
                BlobName = Guid.NewGuid().ToString()
            };
            var connection = new DeleteBlobsBlobConnectionProperties
            {
                ConnectionString = _connectionString,
                ContainerName = _containerName
            };
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.CreateIfNotExistsAsync();

            var result = await AzureBlobTasks.DeleteBlobAsync(input, connection, new CancellationToken());

            Assert.IsTrue(result.Success, "DeleteBlob should've returned true when trying to delete non existing blob");
        }

        [Test]
        public async Task DeleteBlobAsync_ShouldReturnTrueWithNonexistingContainer()
        {
            var input = new DeleteBlobProperties
            {
                BlobName = Guid.NewGuid().ToString()
            };
            var options = new DeleteBlobsBlobConnectionProperties
            {
                ConnectionString = _connectionString,
                ContainerName = Guid.NewGuid().ToString()
            };

            var result = await AzureBlobTasks.DeleteBlobAsync(input, options, new CancellationToken());

            Assert.IsTrue(result.Success,
                "DeleteBlob should've returned true when trying to delete blob in non existing container");
        }

        [Test]
        public async Task DeleteContainerAsync_ShouldReturnTrueWithNonexistingContainer()
        {
            var inputProperties = new DeleteBlobsContainerProperties { ContainerName = Guid.NewGuid().ToString() };
            var connection = new DeleteBlobsContainerConnectionProperties { ConnectionString = _connectionString };

            var result = await AzureBlobTasks.DeleteContainerAsync(inputProperties, connection, new CancellationToken());

            Assert.IsTrue(result.Success,
                "DeleteContainer should've returned true when trying to delete non existing container");
        }
    }
}