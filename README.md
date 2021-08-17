# Frends.Community.Azure

frends Community task library for Azure related operations

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.Azure/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.Azure/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.Azure) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Frends.Community.Azure](#frendscommunityazure)
- [Installing](#installing)
- [Tasks](#tasks)
  - [AzureBlobTasks](#azureblobtasks)
    - [UploadFileAsync](#uploadfileasync)
      - [Properties](#properties)
      - [Returns](#returns)
    - [ListBlobs](#listblobs)
      - [Properties](#properties-1)
      - [Returns](#returns-1)
    - [DownloadBlobAsync](#downloadblobasync)
      - [Properties](#properties-2)
      - [Returns](#returns-2)
    - [ReadBlobContentAsync](#readblobcontentasync)
      - [Properties](#properties-3)
      - [Returns](#returns-3)
    - [DeleteBlobAsync](#deleteblobasync)
      - [Properties](#properties-4)
      - [Returns](#returns-4)
    - [DeleteContainerAsync](#deletecontainerasync)
      - [Properties](#properties-5)
      - [Returns](#returns-5)
  - [AzureQueueTasks](#azurequeuetasks)
    - [CreateQueueAsync](#createqueueasync)
      - [Properties](#properties-6)
      - [Returns](#returns-6)
    - [DeleteQueueAsync](#deletequeueasync)
      - [Properties](#properties-7)
      - [Returns](#returns-7)
    - [GetQueueLengthAsync](#getqueuelengthasync)
      - [Properties](#properties-8)
      - [Returns](#returns-8)
    - [InsertMessageAsync](#insertmessageasync)
      - [Properties](#properties-9)
      - [Returns](#returns-9)
    - [DeleteMessageAsync](#deletemessageasync)
      - [Properties](#properties-10)
      - [Returns](#returns-10)
    - [PeekNextMessageAsync](#peeknextmessageasync)
      - [Properties](#properties-11)
      - [Returns](#returns-11)
  - [AzureOAuthTasks](#azureoauthtasks)
    - [GetAccessToken](#getaccesstoken)
      - [Properties](#properties-12)
      - [Returns](#returns-12)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.Azure

# Tasks

## AzureBlobTasks

Task operations that use Azure DataMovement library for managing blobs
https://github.com/Azure/azure-storage-net-data-movement

### UploadFileAsync

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Source File | string | Full path to file that is uploaded. | 'c:\temp\uploadMe.xml' |
| Contents Only | bool | Reads file content as string and treats content as selected Encoding | true |
| Compress | bool | Applies gzip compression to file or file content | true |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container where the data will be uploaded. If the container doesn't exist, then it will be created. See [Naming and Referencing Containers](https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata) for naming conventions. | 'my-container' |
| Create container if it does not exist | bool | Tries to create the container if it does not exist. | false |
| Blob Type | enum: Append, Block or Page  | Azure blob type to upload. | Block |
| Rename To | string | If value is set, uploaded file will be renamed to this. | 'newFileName.xml' |
| Overwrite | bool | Should upload operation overwrite existing file with same name. | true |
| ParallelOperations | int | The number of the concurrent operations. | 64 |
| Content-Type | string | Forces any content-type to file. If empty, tries to guess based on extension and MIME-type | text/xml |
| Content-Encoding | string | File content is treated as this. Does not affect file encoding when Contents Only is true. If compression is enabled, Content-Type is set as 'gzip' | utf8 |

#### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| SourceFile | string | Full path of file uploaded | |
| Uri | string | Uri to uploaded blob | |

### ListBlobs
List blobs in container

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where the data will be downloaded. | 'my-container' |
| Flat blob listing | bool | Specifies whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory. | true |
| Prefix | string | Blob prefix used while searching container | |

#### Returns

Result is a list of object with following properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Name | string | Blob Name. With Directories this is empty. | |
| Uri | string | Blob Uri | |
| BlobType | string | Type of the blob. Either 'Block','Page' or 'Directory' | 'Block' |
| ETag | string | Value that is updated everytime blob is updated | 

### DownloadBlobAsync
Downloads blob to a file

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where the data will be downloaded. | 'my-container' |
| Blob Name | string | Name of the blob to be downloaded. | 'donwloadMe.xml' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to download. | Block |
| Directory | string | Download destination directory. | 'c:\downloads' |
| FileExistsOperation | enum: Error, Rename, Overwrite | Action to take if destination file exists. Error: throws exception, Overwrite: writes over existing file, Rename: Renames file by adding '(1)' at the end (example: myFile.txt --> myFile(1).txt) | Error |

#### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| FileName | string | Downloaded file name. | |
| Directory | string | Download directory. | |
| FullPath | string | Full path to downloaded file. | |

### ReadBlobContentAsync
Reads blob content to string.

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where blob data is located. | 'my-container' |
| Blob Name | string | Name of the blob which content is read. | 'donwloadMe.xml' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to read. | Block |
| Encoding | string | Encoding name in which blob content is read. | 'UTF-8' |

#### Returns 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Content | string | Blob content. | |

### DeleteBlobAsync
Deletes a blob from target container. Operation result is seen as succesful even if the blob or container doesn't exist.

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the container where delete blob exists. | 'my-container' |
| Blob Name | string | Name of the blob to delete. | 'deleteMe.xml' |
| Verify ETag when deleting | string | Delete blob only if the ETag matches. Leave empty if verification is not needed. Used for concurrency. | 0x9FE13BAA3234312 |
| Blob Type | enum: Append, Block or Page | Azure blob type to read. | Block |
| Snapshot delete option | enum:  None, IncludeSnapshots or DeleteSnapshotsOnly | Defines what should be done with blob snapshots | |

#### Returns 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Success | bool | Indicates whether the operation was succesful or not. | true |

### DeleteContainerAsync
Deletes a whole container from blob storage.

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the container to delete. | 'my-container' |

#### Returns 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Success | bool | Indicates whether the operation was succesful or not. | true |

## AzureQueueTasks

Task operations that uses Azure Queue library for working with Microsoft Azure Storage Queue service for storing and retrieving messages

### CreateQueueAsync

Creates a new Queue in QueueStorage

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the created Queue. See [Naming Queues and Metada](https://docs.microsoft.com/en-us/rest/api/storageservices/naming-queues-and-metadata) for naming conventions. | 'my-new-queue' |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | Task result message. If Throw Exception on failure is false, contains error message. | 'Queue 'my-queue' created.' |



### DeleteQueueAsync

Deletes a queue in QueueStorage

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the Queue you wish to delete. | 'delete-this-queue' |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | Task result message. If Throw Exception on failure is false, contains error message. | 'Queue 'my-queue' deleted.' |

### GetQueueLengthAsync

Gets an estimate number of messages in a queue

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the Queue which message count is calculated. | 'count-this-queue' |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | Task result message. If Throw Exception on failure is false, contains error message. |  |
| Count | int | The estimated number of messages in queue. | 4 |

### InsertMessageAsync

Inserts a new message to queue

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the Queue where message is added. | 'add-message-queue' |
| Content | string | Message content. | 'Hello QueueStorage!' |
| Create Queue | bool | Indicates if Queue should be created in case it does not exist. | true |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | Task result message. If Throw Exception on failure is false, contains error message. | 'Message added to queue 'my-queue'.' |

### DeleteMessageAsync

Removes next message from queue

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the Queue from where message is deleted. | 'remove-message-queue' |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | Task result message. If Throw Exception on failure is false, contains error message. | 'Deleted next message in queue 'my-queue'.' |

### PeekNextMessageAsync

Peeks at the message in front of the queue and returns its content

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Storage Connection String | string | Queue Storage Connection string. See [Configure Azure Storage connection strings](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) | UseDevelopmentStorage=true; |
| Queue Name | string | Name of the Queue from where message is peeked. | 'peek-message-queue' |
| Throw Error On Failure | bool | If true exception is thrown in case of failure, otherwise returns Object { Success = false} | true |

#### Returns
| Property | Type | Description | Example |
| ---------------------| ---------------------| ----------------------- | -------- |
| Success | boolean | Task execution result. | true |
| Info | string | If Task fails, contains error information |  |
| Content | string | Content of the message in front of the queue | 'some content.' |

## AzureOAuthTasks

Task operations that use Azure ADAL.NET library for acquiring tokens from Azure AD and ADFS
https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki

### GetAccessToken
Authenticates and gets JWT access token.

#### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| AuthContextURL | string | Argument for new AuthenticationContext(*AuthContextURL*) | 'https://login.windows.net/{{TenantId}}' |
| Resource | string | Resource name for authContext.AcquireTokenAsync(*Resource*, credential) | 'https://management.azure.com/' |
| ClientId | string | Client ID from Azure | 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX' |
| ClientSecret | string | Client Secret from Azure | 'AzureClientSecretPassword' |

#### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Access token | string | JWT Access token | |

# Building

Clone a copy of the repository

`git clone https://github.com/CommunityHiQ/Frends.Community.Azure.git`

Rebuild the project

`dotnet build`

Run tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repository on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version | Changes |
| ------- | ------- |
| 1.0.0   | First version. Frends Azure tasks merged to this solution |
