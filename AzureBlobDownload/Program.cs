using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Data;
//using Microsoft.WindowsAzure.Storage;

LogDownloader logDownloader = new LogDownloader();
logDownloader.Main();

class LogDownloader
{    
    private const string StorageAccountName = "{account}";
    private const string StorageAccountKey = "{storageKey}";
    private const string RoofOfFolderToDownload = "rootfolder/2022/12"; //Blob Location <= Example
    private const string ContentOfFolderToDownload = RoofOfFolderToDownload + "/15/"; //Day <= Example
    private const string LogFileToDownload = "ServiceLog.log";

    public void Main()
    {     
        var storageAccount = new CloudStorageAccount(new StorageCredentials(StorageAccountName, StorageAccountKey), true);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference("enterprise");
        var blobs = container.GetDirectoryReference(RoofOfFolderToDownload).ListBlobs();

        DownloadBlobs(blobs);
        Console.WriteLine("Completed");
    }

    private static void DownloadBlobs(IEnumerable<IListBlobItem> blobs)
    {
        foreach (var blob in blobs)
        {
            try
            {
                if (blob is CloudBlockBlob blockBlob)
                {
                    if (blockBlob.Name.Contains(LogFileToDownload))
                    {
                        Console.WriteLine($"Downloading File: {blockBlob.Name}");
                        blockBlob.DownloadToFile(blockBlob.Name, FileMode.Create);
                        Console.WriteLine($"Downloaded File: {blockBlob.Name}");
                    }
                }
                else if (blob is CloudAppendBlob appendBlob)
                {
                    if (appendBlob.Name.Contains(LogFileToDownload))
                    {
                        Console.WriteLine($"Downloading File: {appendBlob.Name}");
                        appendBlob.DownloadToFile(appendBlob.Name, FileMode.Create);
                        Console.WriteLine($"Downloaded File: {appendBlob.Name}");
                    }

                    Console.WriteLine($"File: {appendBlob.Name}");
                }
                else if (blob is CloudBlobDirectory blobDirectory)
                {
                    Directory.CreateDirectory(blobDirectory.Prefix);
                    Console.WriteLine($"Folder: {blobDirectory.Prefix}");
                    if (blobDirectory.Prefix == ContentOfFolderToDownload)
                        DownloadBlobs(blobDirectory.ListBlobs());
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }
    }
}