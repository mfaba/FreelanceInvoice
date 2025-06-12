namespace FreelanceInvoice.Infrastructure.FileStorage;

public class FileStorageSettings
{
    public int MaxFileSizeInMB { get; set; }
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
} 