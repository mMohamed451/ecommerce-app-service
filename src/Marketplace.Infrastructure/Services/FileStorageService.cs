using Marketplace.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storageUrl;
    private readonly string _storagePath;

    public FileStorageService(IConfiguration configuration)
    {
        _storageUrl = configuration["Storage:Url"] ?? "http://localhost:9000";
        _storagePath = configuration["Storage:Path"] ?? "./storage";
        
        // Ensure storage directory exists
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        var directory = Path.GetDirectoryName(filePath);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var file = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(file, cancellationToken);
        }

        // Return URL (in production, this would be the actual storage URL)
        return $"{_storageUrl}/{fileName}";
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = fileUrl.Replace($"{_storageUrl}/", "");
            var filePath = Path.Combine(_storagePath, fileName);
            
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath), cancellationToken);
            }
        }
        catch
        {
            // Log error but don't throw - file might not exist
        }
    }

    public async Task<Stream> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = fileUrl.Replace($"{_storageUrl}/", "");
        var filePath = Path.Combine(_storagePath, fileName);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        return await Task.FromResult<Stream>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }
}
