using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace Frends.Community.Azure.Tests
{
    public static class QueueTaskTestHelpers
    {
        public static void DeleteQueue(CloudQueueClient client, string queueName)
        {
            CloudQueue queue = client.GetQueueReference(queueName);
            queue.DeleteIfExists();
        }

        public static void AddMessagesToQueue(CloudQueueClient client, string queueName, int count)
        {
            CloudQueue queue = client.GetQueueReference(queueName);
            for (var i = 0; i < count; i++)
            {
                var queueMessage = new CloudQueueMessage($"Test message number {i}");
                queue.AddMessage(queueMessage);
            }
        }

        /// <summary>
        /// Helper method for delete queue and insert message Tests
        /// </summary>
        public static void CreateQueue(CloudQueueClient client, string name)
        {
            CloudQueue queue = client.GetQueueReference(name);
            queue.CreateIfNotExists();
        }

        public static bool QueueExists(CloudQueueClient client, string name)
        {
            var queue = client.GetQueueReference(name);
            return queue.Exists();
        }

        public static CloudQueueClient GetQueueClient(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudQueueClient();
        }
    }
}