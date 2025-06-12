using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    public BlobStorageService(string connStr)
    {
        String containerName = "images2";
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=imagesstorage100;AccountKey=2RScdgmucRuWYoxMwbIuoKlEL8tSat77otWeyNvUP1FBAuN5yLY/VO0V5p2ACIwuFSmAXRQvEDYm+AStt0Q4Gw==;EndpointSuffix=core.windows.net";
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

    }

    //public async Task<string> UploadFileAsync(IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        throw new ArgumentException("File is null or empty");

    //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    //    var blobClient = _containerClient.GetBlobClient(fileName);

    //    using (var stream = file.OpenReadStream())
    //    {
    //        await blobClient.UploadAsync(stream, new BlobHttpHeaders
    //        {
    //            ContentType = file.ContentType
    //        });
    //    }

    //    return blobClient.Uri.ToString();
    //}
    public async Task UploadFileAsync(string containerName, string blobName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    public async Task<List<string>> ListImageUrlsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var imageUrls = new List<string>();
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            imageUrls.Add(blobClient.Uri.ToString());
        }

        return imageUrls;
    }

    public async Task DeleteFileAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}
