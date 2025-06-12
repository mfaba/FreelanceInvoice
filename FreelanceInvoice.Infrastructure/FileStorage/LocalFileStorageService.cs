using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace FreelanceInvoice.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageSettings _settings;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IOptions<FileStorageSettings> settings)
    {
        _environment = environment;
        _settings = settings.Value;
    }

    public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string folder)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new ArgumentException("No file content provided");

        if (fileBytes.Length > _settings.MaxFileSizeInMB * 1024 * 1024)
            throw new ArgumentException($"File size exceeds {_settings.MaxFileSizeInMB}MB limit");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension))
            throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", _settings.AllowedExtensions)}");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, folder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, fileBytes);

        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        var fullPath = Path.Combine(_environment.WebRootPath, filePath);
        if (File.Exists(fullPath))
        {
            await Task.Run(() => File.Delete(fullPath));
        }
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        return $"/{filePath}";
    }
} 