/*
 * MvcFileUploader utility
 * https://github.com/marufbd/MvcFileUploader
 *
 * Copyright 2015, Maruf Rahman
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MvcFileUploader.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Web;
using System.Threading.Tasks;

namespace MvcFileUploader
{
    public class FileSaver
    {
        private const string DOCUMENT_LIBRARY = "documents";
        private const string IMAGE_LIBRARY = "images";
        CloudStorageAccount storageAccount;
        CloudBlobClient blobStorage;

        public FileSaver(string storageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobStorage = storageAccount.CreateCloudBlobClient();
        }

        public async Task<ViewDataUploadFileResult> StoreFile(Action<MvcFileSave> action, bool isImages)
        {
            var mvcFile = new MvcFileSave();
            mvcFile.FileTimeStamp = DateTime.Now.ToUniversalTime();
            action(mvcFile);

            ViewDataUploadFileResult status;

            //var dirInfo = new DirectoryInfo(mvcFile.StorageDirectory);
            var file = mvcFile.File;
            var fileNameWithoutPath = Path.GetFileName(mvcFile.File.FileName);
            var fileExtension = Path.GetExtension(fileNameWithoutPath);
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(mvcFile.File.FileName));
            var genName = fileName + "-" + mvcFile.FileTimeStamp.ToFileTime();
            var genFileName = String.IsNullOrEmpty(mvcFile.FileName) ? genName + fileExtension : mvcFile.FileName;// get filename if set
            //var fullPath = Path.Combine(mvcFile.StorageDirectory, genFileName);

            try
            {
                CloudBlockBlob blob;
                if (isImages)
                    blob = await UploadImage(mvcFile.File);
                else
                    blob = await UploadDocument(mvcFile.File);

                var viewDataUploadFileResult = new ViewDataUploadFileResult()
                {
                    name = fileNameWithoutPath,
                    SavedFileName = genFileName,
                    size = blob.Properties.Length,
                    type = file.ContentType,
                    url = blob.Uri.AbsoluteUri,//mvcFile.UrlPrefix + "/" + genFileName,
                    //delete_url = Url.Action("DeleteFile", new { fileUrl = "/"+storageRoot+"/" + genFileName }),
                    //thumbnail_url = thumbUrl + "?width=100",
                    deleteType = "POST",
                    Title = fileName,

                    ////for controller use
                    //FullPath = dirInfo.FullName + "/" + genFileName
                };

                //add delete url                           
                mvcFile.AddFileUriParamToDeleteUrl("fileUrl", viewDataUploadFileResult.url);
                viewDataUploadFileResult.deleteUrl = mvcFile.DeleteUrl;

                status = viewDataUploadFileResult;
            }
            catch (Exception exc)
            {
                if (mvcFile.ThrowExceptions)
                    throw;

                status = new ViewDataUploadFileResult()
                             {
                                 error = exc.Message,
                                 name = file.FileName,
                                 size = file.ContentLength,
                                 type = file.ContentType
                             };
            }

            return status;
        }


        public async Task<CloudBlockBlob> UploadDocument(HttpPostedFileBase documentFile)
        {
            string blobName = documentFile.FileName;// Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
            // GET a blob reference. 
            if (!string.IsNullOrEmpty(blobName))
            {
                CloudBlockBlob documentBlob = DocumentContainer.GetBlockBlobReference(blobName);
                // Uploading a local file and Create the blob.
                using (var fs = documentFile.InputStream)
                {
                    await documentBlob.UploadFromStreamAsync(fs);
                }
                return documentBlob;
            }
            return null;
        }

        public async Task<CloudBlockBlob> UploadImage(HttpPostedFileBase imageFile)
        {
            string blobName = imageFile.FileName;// Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
            if (!string.IsNullOrEmpty(blobName))
            {
                // GET a blob reference. 
                CloudBlockBlob imageBlob = ImageContainer.GetBlockBlobReference(blobName);
                // Uploading a local file and Create the blob.
                using (var fs = imageFile.InputStream)
                {
                    await imageBlob.UploadFromStreamAsync(fs);
                }
                return imageBlob;
            }
            return null;
        }


        public CloudBlobContainer DocumentContainer
        {
            get
            {
                var _documentContainer = blobStorage.GetContainerReference(DOCUMENT_LIBRARY);
                if (_documentContainer.CreateIfNotExists())
                {
                    // configure container for public access
                    var permissions = _documentContainer.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    _documentContainer.SetPermissions(permissions);
                }

                return _documentContainer;
            }
        }

        public CloudBlobContainer ImageContainer
        {
            get
            {
                var _imageContainer = blobStorage.GetContainerReference(IMAGE_LIBRARY);
                if (_imageContainer.CreateIfNotExists())
                {
                    // configure container for public access
                    var permissions = _imageContainer.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    _imageContainer.SetPermissions(permissions);
                }

                return _imageContainer;
            }
        }
    }

    public class FileSaverToServer
    {
        public FileSaverToServer()
        {
          
        }

        public async Task<ViewDataUploadFileResult> StoreFile(Action<MvcFileSave> action, bool isImages)
        {
            var mvcFile = new MvcFileSave();
            mvcFile.FileTimeStamp = DateTime.Now.ToUniversalTime();
            action(mvcFile);

            ViewDataUploadFileResult status;

            //var dirInfo = new DirectoryInfo(mvcFile.StorageDirectory);
            var file = mvcFile.File;
            var fileNameWithoutPath = Path.GetFileName(mvcFile.File.FileName);
            var fileExtension = Path.GetExtension(fileNameWithoutPath);
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(mvcFile.File.FileName));
            var genName = fileName + "-" + mvcFile.FileTimeStamp.ToFileTime();
            var genFileName = String.IsNullOrEmpty(mvcFile.FileName) ? genName + fileExtension : mvcFile.FileName;// get filename if set
            //var fullPath = Path.Combine(mvcFile.StorageDirectory, genFileName);

            try
            {
                string fileNameRet;
                if (isImages)
                    fileNameRet = await UploadImage(mvcFile.File, mvcFile.StorageDirectory);
                else
                    fileNameRet = await UploadDocument(mvcFile.File);

                var viewDataUploadFileResult = new ViewDataUploadFileResult()
                {
                    name = fileNameWithoutPath,
                    SavedFileName = genFileName,
                   
                    type = file.ContentType,
                   //mvcFile.UrlPrefix + "/" + genFileName,
                    //delete_url = Url.Action("DeleteFile", new { fileUrl = "/"+storageRoot+"/" + genFileName }),
                    //thumbnail_url = thumbUrl + "?width=100",
                    deleteType = "POST",
                    Title = fileName,

                    ////for controller use
                    //FullPath = dirInfo.FullName + "/" + genFileName
                };

                //add delete url                           
                mvcFile.AddFileUriParamToDeleteUrl("fileUrl", viewDataUploadFileResult.url);
                viewDataUploadFileResult.deleteUrl = mvcFile.DeleteUrl;

                status = viewDataUploadFileResult;
            }
            catch (Exception exc)
            {
                if (mvcFile.ThrowExceptions)
                    throw;

                status = new ViewDataUploadFileResult()
                {
                    error = exc.Message,
                    name = file.FileName,
                    size = file.ContentLength,
                    type = file.ContentType
                };
            }

            return status;
        }


        public async Task<string> UploadDocument(HttpPostedFileBase documentFile)
        {
           
            return null;
        }

        public async Task<string> UploadImage(HttpPostedFileBase imageFile, string FolderPath)
        {
            if (imageFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var path = Path.Combine(FolderPath, fileName);
                imageFile.SaveAs(path);
                return imageFile.FileName;
            }

            return null;
        }
      
    }
}
