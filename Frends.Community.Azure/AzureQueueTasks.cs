using Microsoft.Azure.Storage.Queue;
using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Frends.Community.Azure
{
    public static class AzureQueueTasks
    {
        #region Queues

        /// <summary>
        /// Create a new QueueStorage.
        /// Queue name must start with a letter or number, and can contain only letters, numbers, and the dash (-) character. See https://docs.microsoft.com/en-us/rest/api/storageservices/naming-queues-and-metadata for details.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info }</returns>
        public static async Task<QueueOperationResult> CreateQueueAsync(QueueConnectionProperties connection, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                // check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);

                // check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                // create queue if it does not exist
                if (await queue.CreateIfNotExistsAsync())
                    return new QueueOperationResult { Success = true, Info = $"Queue '{connection.QueueName}' created." };
                else
                    return new QueueOperationResult { Success = false, Info = $"Queue named '{connection.QueueName}' already exists." };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueueOperationResult { Success = false, Info = ex.Message };
            }
        }

        /// <summary>
        /// Delete a existing QueueStorage
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info }</returns>
        public static async Task<QueueOperationResult> DeleteQueueAsync(QueueConnectionProperties connection, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);

                // check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                // delete queue if exists
                if (await queue.DeleteIfExistsAsync())
                    return new QueueOperationResult { Success = true, Info = $"Queue '{connection.QueueName}' deleted." };
                else
                    return new QueueOperationResult { Success = false, Info = $"Queue '{connection.QueueName}' not found." };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueueOperationResult { Success = false, Info = ex.Message };
            }
        }

        /// <summary>
        /// Gets an estimate number of messages in a queue
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info, int Count }</returns>
        public static async Task<QueueGetLengthResult> GetQueueLengthAsync(QueueConnectionProperties connection, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);
                // check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                await queue.FetchAttributesAsync();

                cancellationToken.ThrowIfCancellationRequested();

                int? cachedMessageCount = queue.ApproximateMessageCount;

                return new QueueGetLengthResult { Success = true, Count = cachedMessageCount == null ? 0 : (int)cachedMessageCount };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueueGetLengthResult { Success = false, Info = ex.Message };
            }
        }

        #endregion Queues

        #region Messages

        /// <summary>
        /// Inserts a message to Queue.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info}</returns>
        public static async Task<QueueOperationResult> InsertMessageAsync(QueueConnectionProperties connection, QueueMessageProperties message, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);

                // check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                // create message
                var queueMessage = new CloudQueueMessage(message.Content);

                // create queue if it does not exist
                if (message.CreateQueue)
                    await queue.CreateIfNotExistsAsync();

                await queue.AddMessageAsync(queueMessage);

                return new QueueOperationResult { Success = true, Info = $"Message added to queue '{connection.QueueName}'." };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueueOperationResult { Success = false, Info = ex.Message };
            }
        }

        /// <summary>
        /// Peeks at the message in the front of a queue and returns its content.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info, string Content }</returns>
        public static async Task<QueuePeekMessageResult> PeekNextMessageAsync(QueueConnectionProperties connection, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);

                //check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                var peekedMessage = await queue.PeekMessageAsync();

                if (peekedMessage != null)
                    return new QueuePeekMessageResult { Success = true, Content = peekedMessage.AsString };
                else
                    return new QueuePeekMessageResult { Success = false, Info = $"Message not found in queue '{connection.QueueName}'" };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueuePeekMessageResult { Success = false, Info = ex.Message };
            }
        }

        /// <summary>
        /// Deletes next message in Queue.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, string Info}</returns>
        public static async Task<QueueOperationResult> DeleteMessageAsync(QueueConnectionProperties connection, QueueOptions options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queue = Utils.GetQueueReference(connection);

                //check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                // get message
                var queuedMessage = await queue.GetMessageAsync();

                // check that message was found
                if (queuedMessage != null)
                {
                    await queue.DeleteMessageAsync(queuedMessage);
                    return new QueueOperationResult { Success = true, Info = $"Deleted next message in queue '{connection.QueueName}'" };
                }
                else
                    return new QueueOperationResult { Success = false, Info = $"Could not delete message: Message not found in queue '{connection.QueueName}'" };
            }
            catch (Exception ex)
            {
                if (options.ThrowErrorOnFailure)
                    throw ex;
                return new QueueOperationResult { Success = false, Info = ex.Message };
            }
        }

        #endregion Messages
    }
}