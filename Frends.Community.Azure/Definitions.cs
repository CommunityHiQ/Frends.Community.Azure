#pragma warning disable 1591

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Azure
{
    #region BlobTasks

    #region Enums

    public enum AzureBlobType
    {
        Append,
        Block,
        Page
    }

    public enum FileExistsAction
    {
        Error,
        Rename,
        Overwrite
    }

    public enum SourceBlobOperation
    {
        Download,
        Read
    }

    public enum SnapshotDeleteOption
    {
        None,
        IncludeSnapshots,
        DeleteSnapshotsOnly
    }

    #endregion Enums

    #region ListBLobs

    public class ListBlobsSourceProperties
    {
        /// <summary>
        /// Connection string for Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///  Name of the Azure blob storage container where the file is downloaded from
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Specifies whether to list blobs in a flat listing or whether to list blobs hierarchically by virtual directory
        /// </summary>
        [DefaultValue("true")]
        public bool FlatBlobListing { get; set; }

        /// <summary>
        /// Blob prefix used while searching container
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Prefix { get; set; }
    }

    public class ListBlobsOutput
    {
        public List<BlobData> Blobs { get; set; }
    }

    public class BlobData
    {
        public string BlobType { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public string ETag { get; set; }
    }

    #endregion ListBLobs

    #region DownloadBlobs

    public class DownloadBlobSourceProperties
    {
        /// <summary>
        /// Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the azure blob storage container where the file is downloaded from.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Name of the blob to download
        /// </summary>
        [DefaultValue("example.xml")]
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        /// <summary>
        /// Type of blob to download: Append, Block or Page.
        /// </summary>
        [DisplayName("Blob Type")]
        [DefaultValue(AzureBlobType.Block)]
        public AzureBlobType BlobType { get; set; }

        /// <summary>
        /// Set encoding manually. Empty value tries to get encoding set in Azure.
        /// </summary>
        [DefaultValue("UTF-8")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Encoding { get; set; }
    }

    public class DownloadBlobDestinationFileProperties
    {
        /// <summary>
        /// Download destination directory.
        /// </summary>
        [DefaultValue(@"c:\temp")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Directory { get; set; }

        /// <summary>
        /// Error: Throws exception if destination file exists.
        /// Rename: Adds '(1)' at the end of file name. Incerements the number if (1) already exists.
        /// Overwrite: Overwrites existing file.
        /// </summary>
        [DefaultValue(FileExistsAction.Error)]
        public FileExistsAction FileExistsOperation { get; set; }
    }

    public class DownloadBlobReadContentOutput
    {
        public string Content { get; set; }
    }

    public class DownloadBlobOutput
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string FullPath { get; set; }
    }

    public class DownloadBlobOutputBase
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string FullPath { get; set; }
        public string Content { get; set; }
    }

    #endregion DownloadBlobs

    #region DeleteBlobs

    public class DeleteBlobsOutput
    {
        public bool Success { get; set; }
    }

    public class DeleteBlobsContainerProperties
    {
        /// <summary>
        /// Name of the container to delete
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }
    }

    public class DeleteBlobsContainerConnectionProperties
    {
        /// <summary>
        /// Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }
    }

    public class DeleteBlobsBlobConnectionProperties
    {
        /// <summary>
        /// Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the container where delete blob exists.
        /// </summary>
        [DisplayName("Blob Container Name")]
        [DefaultValue("test-container")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }
    }

    public class DeleteBlobProperties
    {
        /// <summary>
        /// Name of the blob to delete
        /// </summary>
        [DisplayName("Blob name")]
        [DefaultValue("TestFile.xml")]
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        /// <summary>
        /// Delete blob only if the ETag matches. Leave empty if verification is not needed.
        /// </summary>
        [DisplayName("Verify ETag When Deleting")]
        [DefaultValue("0x9FE13BAA323E5A4")]
        [DisplayFormat(DataFormatString = "Text")]
        public string VerifyETagWhenDeleting { get; set; }

        /// <summary>
        /// Type of blob to delete: Append, Block or Page
        /// </summary>
        [DisplayName("Blob Type")]
        [DefaultValue(AzureBlobType.Block)]
        public AzureBlobType BlobType { get; set; }

        /// <summary>
        /// What should be done with blob snapshots?
        /// </summary>
        [DisplayName("Snapshot Delete Option")]
        [DefaultValue(SnapshotDeleteOption.IncludeSnapshots)]
        public SnapshotDeleteOption SnapshotDeleteOption { get; set; }
    }

    #endregion DeleteBlobs

    #region UploadBlobs

    public class UploadBlobsOutput
    {
        public string SourceFile { get; set; }
        public string Uri { get; set; }
    }

    public class UploadBlobsDestinationProperties
    {
        /// <summary>
        /// Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the azure blob storage container where the data will be uploaded.
        /// Naming: lowercase
        /// Valid chars: alphanumeric and dash, but cannot start or end with dash
        /// </summary>
        [DefaultValue("test-container")]
        [DisplayName("Container Name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Determines if the container should be created if it does not exist
        /// </summary>
        [DisplayName("Create container if it does not exist")]
        public bool CreateContainerIfItDoesNotExist { get; set; }

        /// <summary>
        /// Azure blob type to upload: Append, Block or Page
        /// </summary>
        [DefaultValue(AzureBlobType.Block)]
        [DisplayName("Blob Type")]
        public AzureBlobType BlobType { get; set; }

        /// <summary>
        /// Source file can be renamed to this name in azure blob storage
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Rename source file")]
        [DisplayFormat(DataFormatString = "Text")]
        public string RenameTo { get; set; }

        /// <summary>
        /// Set desired content-type. If empty, task tries to guess from mime-type.
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Force Content-Type")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContentType { get; set; }

        /// <summary>
        /// Set desired content-encoding. Defaults to UTF8 BOM.
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Force Content-Encoding")]
        [DisplayFormat(DataFormatString = "Text")]
        public string FileEncoding { get; set; }

        /// <summary>
        /// Should upload operation overwrite existing file with same name?
        /// </summary>
        [DefaultValue(true)]
        [DisplayName("Overwrite existing file")]
        public bool Overwrite { get; set; }

        /// <summary>
        /// How many work items to process concurrently.
        /// </summary>
        [DefaultValue(64)]
        [DisplayName("Parallel Operation")]
        public int ParallelOperations { get; set; }
    }

    public class UploadBlobsInput
    {
        [DefaultValue(@"c:\temp\TestFile.xml")]
        [DisplayName("Source File")]
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceFile { get; set; }

        /// <summary>
        /// Uses stream to read file content.
        /// </summary>
        [DefaultValue(false)]
        [DisplayName("Stream content only")]
        public bool ContentsOnly { get; set; }

        /// <summary>
        /// Works only when transferring stream content.
        /// </summary>
        [DefaultValue(false)]
        [DisplayName("Gzip compression")]
        public bool Compress { get; set; }
    }

    #endregion UploadBlobs

    #endregion BlobTasks

    #region QueueTasks

    /// <summary>
    /// Properties for Azure Storage Connection
    /// </summary>
    public class QueueConnectionProperties
    {
        /// <summary>
        /// Connection String to Azure Storage
        /// </summary>
        [DefaultValue("\"UseDevelopmentStorage=true;\"")]
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Queue name must start with a letter or number, and can contain only letters, numbers, and the dash (-) character.
        /// </summary>
        public string QueueName { get; set; }
    }

    public class QueueMessageProperties
    {
        /// <summary>
        /// Message content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// True: creates queue if it does not exist
        /// False: throws error if queue does not exist
        /// </summary>
        [DefaultValue(true)]
        public bool CreateQueue { get; set; }
    }

    public class QueueOptions
    {
        /// <summary>
        /// Choose if Exception should be thrown if error occurs, otherwise returns Object { Success = false, Message = 'Error information' }
        /// </summary>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; }
    }

    public class QueueOperationResult
    {
        public bool Success { get; set; }
        public string Info { get; set; }
    }

    public class QueueGetLengthResult
    {
        public bool Success { get; set; }
        public string Info { get; set; }
        public int Count { get; set; }
    }

    public class QueuePeekMessageResult
    {
        public bool Success { get; set; }
        public string Info { get; set; }
        public string Content { get; set; }
    }

    #endregion QueueTasks
}