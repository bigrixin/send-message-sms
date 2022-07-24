﻿using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MyAbilityFirst.Infrastructure;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;

namespace MyAbilityFirst.Services.Common
{
	public class UploadService : IUploadService
	{

		#region Fields

		private readonly IWriteEntities _entities;

		#endregion

		#region Ctor

		public UploadService(IWriteEntities entities)
		{
			this._entities = entities;
		}

		#endregion

		#region IUploadService
		public string UploadToAzureStorage(HttpPostedFileBase file, string containerName)
		{
			if (file == null)
				return "False";

			//The container name must be lowercase.
			//		string containerName = "client";
			string pathFileName = getNewFileName(file);
			//mimeType == "image/png", or "application/pdf"
			string mimeType = file.ContentType.ToLower();

			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve a reference to a container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			// Create the container if it doesn't already exist.
			container.CreateIfNotExists();

			// Make container for public
			container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

			// Retrieve reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(pathFileName);
			if (mimeType.IndexOf("image") != -1)
				blockBlob.Properties.ContentType = mimeType;   // "image/jpg";

			// Reset the Stream to the Beginning before upload
			file.InputStream.Seek(0, SeekOrigin.Begin);

			blockBlob.UploadFromStream(file.InputStream);

			//	string sasURL=GetBlobSasUri(blockBlob);   //get sasURL for limited time, later need it.

			string fileURL = blockBlob.Uri.ToString();

			return blockBlob.Uri.ToString();
		}

		public bool DeleteFromAzureStorage(string fileURL, string containerName)
		{
			if (String.IsNullOrEmpty(fileURL))
				return false;
			int position = fileURL.IndexOf(containerName) + containerName.Length + 1;
			string fileName = fileURL.Substring(position);
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			// Retrieve reference to a blob named "myblob.txt".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

			if (blockBlob.Exists())
			{
				blockBlob.Delete();
				return true;
			}
			return false;
		}

		public void ListBlobItemFromAzure(string containerName)
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			// Loop over items within the container and output the length and URI.
			foreach (IListBlobItem item in container.ListBlobs(null, false))
			{
				if (item.GetType() == typeof(CloudBlockBlob))
				{
					CloudBlockBlob blob = (CloudBlockBlob)item;

					Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

				}
				else if (item.GetType() == typeof(CloudPageBlob))
				{
					CloudPageBlob pageBlob = (CloudPageBlob)item;

					Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

				}
				else if (item.GetType() == typeof(CloudBlobDirectory))
				{
					CloudBlobDirectory directory = (CloudBlobDirectory)item;

					Console.WriteLine("Directory: {0}", directory.Uri);
				}
			}
		}

		#endregion

		#region Helper

		private string getNewFileName(HttpPostedFileBase file)
		{
			// 	string mimeType = file.ContentType.ToLower();   //mimeType == "image/png", or "application/pdf"
			MD5 md5 = MD5.Create();
			var hashMD5 = md5.ComputeHash(file.InputStream);
			//For new file name prefix
			string fileNamePrefix = Guid.NewGuid().ToString();
			string fileMD5String = BitConverter.ToString(hashMD5).Replace("-", string.Empty);
			string path = fileMD5String.Substring(0, 3);  //sub directory
			string fileName = file.FileName.ToLower();
			//Use for long file name, cut to first 20 characters
			if (fileName.Length > 20)
			{
				var fileType = fileName.Substring(fileName.LastIndexOf('.'));
				fileName = fileName.Substring(0, 20) + fileType;
			}
			string newFileName = path + "/" + fileNamePrefix + "-" + fileName;
			return newFileName;
		}

		//This is used for container has set Permissions, keep it
		private string GetBlobSasUri(CloudBlockBlob blob)
		{
			//Set the expiry time and permissions for the blob.
			//In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
			//The shared access signature will be valid immediately.
			SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
			sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
			sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
			sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

			//Generate the shared access signature on the blob, setting the constraints directly on the signature.
			string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

			//Return the URI string for the container, including the SAS token.
			return blob.Uri + sasBlobToken;
		}

		#endregion

	}
}