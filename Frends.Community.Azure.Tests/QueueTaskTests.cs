using Microsoft.Azure.Storage.Queue;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Frends.Community.Azure.Tests
{
    [TestFixture]
    [Ignore("Unit tests requires Azure Storage Emulator running")]
    public class QueueTaskTests
    {
        private QueueConnectionProperties _queueConnectionProperties;
        private QueueOptions _options;
        private const string CONNECTIONSTRING = "UseDevelopmentStorage=true";
        private readonly string _createQueueName = "hiq-test-queue";
        private CloudQueueClient _testClient;

        [SetUp]
        public void InitializeTests()
        {
            _queueConnectionProperties = new QueueConnectionProperties { StorageConnectionString = CONNECTIONSTRING, QueueName = _createQueueName };
            _options = new QueueOptions { ThrowErrorOnFailure = true };
            _testClient = QueueTaskTestHelpers.GetQueueClient(CONNECTIONSTRING);
        }

        [TearDown]
        public void TestTearDown()
        {
            //remove test queues
            QueueTaskTestHelpers.DeleteQueue(_testClient, _createQueueName);
        }

        [Test]
        public void CreateQueueTest()
        {
            //_queueConnectionProperties.QueueName = _createQueueName;
            var result = AzureQueueTasks.CreateQueueAsync(_queueConnectionProperties, _options, new CancellationToken());

            Assert.IsTrue(result.Result.Success);
            Assert.IsTrue(QueueTaskTestHelpers.QueueExists(_testClient, _createQueueName));
        }

        [Test]
        public void CreateQueue_ReturnsFalseIfQueueExists()
        {
            var resultSuccess = AzureQueueTasks.CreateQueueAsync(_queueConnectionProperties, _options, new CancellationToken());
            var resultFails = AzureQueueTasks.CreateQueueAsync(_queueConnectionProperties, _options, new CancellationToken());

            Assert.IsTrue(resultSuccess.Result.Success);
            Assert.IsFalse(resultFails.Result.Success);
            Assert.IsTrue(QueueTaskTestHelpers.QueueExists(_testClient, _createQueueName));
        }

        [Test]
        public void DeleteQueue()
        {
            // create queue and check that it exists before delete test
            QueueTaskTestHelpers.CreateQueue(_testClient, _createQueueName);
            Assert.IsTrue(QueueTaskTestHelpers.QueueExists(_testClient, _createQueueName));

            var result = AzureQueueTasks.DeleteQueueAsync(_queueConnectionProperties, _options, new CancellationToken());

            Assert.IsTrue(result.Result.Success);
            Assert.IsFalse(QueueTaskTestHelpers.QueueExists(_testClient, _createQueueName));
        }

        [Test]
        public void DeleteQueue_ReturnsFalseIfQueueDoesNotExist()
        {
            _queueConnectionProperties.QueueName = "foobar";
            var result = AzureQueueTasks.DeleteQueueAsync(_queueConnectionProperties, _options, new CancellationToken());

            Assert.IsFalse(result.Result.Success);
        }

        [Test]
        public void GetQueueLength()
        {
            QueueTaskTestHelpers.CreateQueue(_testClient, _createQueueName);
            QueueTaskTestHelpers.AddMessagesToQueue(_testClient, _createQueueName, 5);
            //_queueConnectionProperties.QueueName = _messageQueueName;

            var result = AzureQueueTasks.GetQueueLengthAsync(_queueConnectionProperties, _options, new CancellationToken());

            Assert.IsTrue(result.Result.Success);
            Assert.AreEqual(5, result.Result.Count);
        }
    }
}