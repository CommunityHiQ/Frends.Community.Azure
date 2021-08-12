using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using MimeMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Frends.Community.Azure
{
    public static class AzureBlobTasks
    {
        #region ListBlobs

        /// <summary>
        /// List blobs in container. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Object { List&lt;Object&gt; { string Name, string Uri, string BlobType }}</returns>
        public static ListBlobsOutput ListBlobs(ListBlobsSourceProperties source)
        {
            var container = Utils.GetBlobContainer(source.ConnectionString, source.ContainerName);

            var blobs = new List<BlobData>();

            foreach (var item in container.ListBlobs(string.IsNullOrWhiteSpace(source.Prefix) ? null : source.Prefix,
                source.FlatBlobListing))
            {
                var blobType = item.GetType();

                if (blobType == typeof(CloudBlockBlob))
                {
                    var blockBlob = (CloudBlockBlob)item;
                    blobs.Add(new BlobData
                    {
                        BlobType = "Block",
                        Uri = blockBlob.Uri.ToString(),
                        Name = blockBlob.Name,
                        ETag = blockBlob.Properties.ETag
                    });
                }
                else if (blobType == typeof(CloudPageBlob))
                {
                    var pageBlob = (CloudPageBlob)item;
                    blobs.Add(new BlobData
                    {
                        BlobType = "Page",
                        Uri = pageBlob.Uri.ToString(),
                        Name = pageBlob.Name,
                        ETag = pageBlob.Properties.ETag
                    });
                }
                else if (blobType == typeof(CloudBlobDirectory))
                {
                    var directory = (CloudBlobDirectory)item;
                    blobs.Add(new BlobData
                    {
                        BlobType = "Directory",
                        Uri = directory.Uri.ToString(),
                        Name = directory.Prefix
                    });
                }
            }

            return new ListBlobsOutput { Blobs = blobs };
        }

        #endregion ListBlobs

        #region DownloadBlobs

        /// <summary>
        /// Downloads Blob to a file. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { string FileName, string Directory, string FullPath}</returns>
        public static async Task<DownloadBlobOutput> DownloadBlobAsync(DownloadBlobSourceProperties source,
            DownloadBlobDestinationFileProperties destination, CancellationToken cancellationToken)
        {
            var result = await DownloadBlob(source, destination, SourceBlobOperation.Download, cancellationToken);
            return new DownloadBlobOutput
            {
                Directory = result.Directory,
                FileName = result.FileName,
                FullPath = result.FullPath
            };
        }

        /// <summary>
        /// Reads blob content and returns it. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { string Content }</returns>
        public static async Task<DownloadBlobReadContentOutput> ReadBlobContentAsync(DownloadBlobSourceProperties source,
            CancellationToken cancellationToken)
        {
            var result = await DownloadBlob(source, null, SourceBlobOperation.Read, cancellationToken);
            return new DownloadBlobReadContentOutput
            {
                Content = result.Content
            };
        }

        private static async Task<DownloadBlobOutputBase> DownloadBlob(DownloadBlobSourceProperties sourceProperties,
            DownloadBlobDestinationFileProperties destinationProperties, SourceBlobOperation operation,
            CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();
            var container = Utils.GetBlobContainer(sourceProperties.ConnectionString, sourceProperties.ContainerName);
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();
            // get reference to blob
            var blobReference = Utils.GetCloudBlob(container, sourceProperties.BlobName, sourceProperties.BlobType);
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            var encoding = string.IsNullOrEmpty(sourceProperties.Encoding) ? blobReference.GetEncoding() : Encoding.GetEncoding(sourceProperties.Encoding);

            var content = await blobReference.ReadContents(encoding, cancellationToken);
            switch (operation)
            {
                case SourceBlobOperation.Read:
                    return new DownloadBlobOutputBase { Content = content };

                case SourceBlobOperation.Download:
                    return WriteToFile(content, sourceProperties.BlobName, encoding, destinationProperties);

                default:
                    throw new Exception("Unknown operations. Allowed operations are Read and Download");
            }
        }

        private static DownloadBlobOutputBase WriteToFile(string content, string fileName, Encoding encoding,
            DownloadBlobDestinationFileProperties destinationProperties)
        {
            var destinationFileName = fileName;
            if (File.Exists(Path.Combine(destinationProperties.Directory, destinationFileName)))
                switch (destinationProperties.FileExistsOperation)
                {
                    case FileExistsAction.Error:
                        throw new IOException($"Destination file '{destinationFileName}' already exists.");
                    case FileExistsAction.Rename:
                        destinationFileName =
                            Utils.GetRenamedFileName(destinationFileName, destinationProperties.Directory);
                        break;
                }

            // Write blob content to file
            var destinationFileFullPath = Path.Combine(destinationProperties.Directory, destinationFileName);
            File.WriteAllText(destinationFileFullPath, content, encoding);

            return new DownloadBlobOutputBase
            {
                FullPath = destinationFileFullPath,
                Directory = Path.GetDirectoryName(destinationFileFullPath),
                FileName = Path.GetFileName(destinationFileFullPath)
            };
        }

        #endregion DownloadBlobs

        #region DeleteBlobs

        /// <summary>
        /// Deletes a single blob from Azure blob storage. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// </summary>
        /// <param name="target">Blob to delete</param>
        /// <param name="connectionProperties"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object { bool Success }</returns>
        public static async Task<DeleteBlobsOutput> DeleteBlobAsync(DeleteBlobProperties target,
            DeleteBlobsBlobConnectionProperties connectionProperties, CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            // get container
            var container = Utils.GetBlobContainer(connectionProperties.ConnectionString,
                connectionProperties.ContainerName);

            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            // get the destination blob, rename if necessary
            var blob = Utils.GetCloudBlob(container, target.BlobName, target.BlobType);

            if (!await blob.ExistsAsync(cancellationToken)) return new DeleteBlobsOutput { Success = true };

            try
            {
                var accessCondition = string.IsNullOrWhiteSpace(target.VerifyETagWhenDeleting)
                    ? AccessCondition.GenerateIfMatchCondition(target.VerifyETagWhenDeleting)
                    : AccessCondition.GenerateEmptyCondition();

                var result = await blob.DeleteIfExistsAsync(
                    target.SnapshotDeleteOption.ConvertEnum<DeleteSnapshotsOption>(), accessCondition,
                    new BlobRequestOptions(), new OperationContext(), cancellationToken);

                return new DeleteBlobsOutput { Success = result };
            }
            catch (Exception e)
            {
                throw new Exception("DeleteBlobAsync: Error occured while trying to delete blob", e);
            }
        }

        /// <summary>
        /// Deletes a whole container from Azure blob storage. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// </summary>
        /// <param name="target"></param>
        /// <param name="connectionProperties"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object { bool Success }</returns>
        public static async Task<DeleteBlobsOutput> DeleteContainerAsync(DeleteBlobsContainerProperties target,
            DeleteBlobsContainerConnectionProperties connectionProperties, CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            // get container
            var container = Utils.GetBlobContainer(connectionProperties.ConnectionString, target.ContainerName);

            if (!await container.ExistsAsync(cancellationToken)) return new DeleteBlobsOutput { Success = true };

            // delete container
            try
            {
                var result = await container.DeleteIfExistsAsync(cancellationToken);
                return new DeleteBlobsOutput { Success = result };
            }
            catch (Exception e)
            {
                throw new Exception("DeleteContainerAsync: Error occured while trying to delete blob container", e);
            }
        }

        #endregion DeleteBlobs

        #region UploadBlobs

        /// <summary>
        /// Uploads a single file to Azure blob storage. See https://github.com/CommunityHiQ/Frends.Community.Azure
        /// Will create given container on connection if necessary.
        /// </summary>
        /// <returns>Object { string Uri, string SourceFile }</returns>
        public static async Task<UploadBlobsOutput> UploadFileAsync(UploadBlobsInput input,
            UploadBlobsDestinationProperties destinationProperties, CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            // check that source file exists
            var fi = new FileInfo(input.SourceFile);
            if (!fi.Exists)
                throw new ArgumentException($"Source file {input.SourceFile} does not exist", nameof(input.SourceFile));

            // get container
            var container = Utils.GetBlobContainer(destinationProperties.ConnectionString,
                destinationProperties.ContainerName);

            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (destinationProperties.CreateContainerIfItDoesNotExist)
                    await container.CreateIfNotExistsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Checking if container exists or creating new container caused an exception.", ex);
            }

            // get the destination blob, rename if necessary
            var destinationBlob = Utils.GetCloudBlob(container,
                string.IsNullOrWhiteSpace(destinationProperties.RenameTo) ? fi.Name : destinationProperties.RenameTo,
                destinationProperties.BlobType);

            var contentType = string.IsNullOrWhiteSpace(destinationProperties.ContentType)
                ? MimeUtility.GetMimeMapping(fi.Name)
                : destinationProperties.ContentType;

            var encoding = destinationProperties.FileEncoding.ConvertToEncoding();

            // delete blob if user requested overwrite
            if (destinationProperties.Overwrite) await destinationBlob.DeleteIfExistsAsync(cancellationToken);

            // setup the number of the concurrent operations
            TransferManager.Configurations.ParallelOperations = destinationProperties.ParallelOperations;

            // Use UploadOptions to set ContentType of destination CloudBlob
            var uploadOptions = new UploadOptions();

            var progressHandler = new Progress<TransferStatus>(progress =>
            {
                Console.WriteLine("Bytes uploaded: {0}", progress.BytesTransferred);
            });

            // Setup the transfer context and track the upload progress
            var transferContext = new SingleTransferContext
            {
                SetAttributesCallbackAsync = (src, destination) =>
                {
                    if (!(destination is CloudBlob)) throw new Exception("We did not get CloudBlob reference. ");
                    var cloudBlob = (CloudBlob)destination;
                    cloudBlob.Properties.ContentType = contentType;
                    cloudBlob.Properties.ContentEncoding = input.Compress ? "gzip" : encoding.WebName;
                    return Task.CompletedTask;
                },

                ProgressHandler = progressHandler
            };

            // begin and await for upload to complete
            try
            {
                using (var stream = Utils.GetStream(input.Compress, input.ContentsOnly, encoding, fi))
                {
                    await TransferManager.UploadAsync(
                        stream,
                        destinationBlob,
                        uploadOptions,
                        transferContext,
                        cancellationToken);
                }
            }
            catch (Exception e)
            {
                throw new Exception("UploadFileAsync: Error occured while uploading file to blob storage", e);
            }

            // return uri to uploaded blob and source file path
            return new UploadBlobsOutput { SourceFile = input.SourceFile, Uri = destinationBlob.Uri.ToString() };
        }

        #endregion UploadBlobs
    }
}