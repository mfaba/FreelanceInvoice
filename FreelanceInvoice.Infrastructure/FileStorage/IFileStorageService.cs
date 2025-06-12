namespace FreelanceInvoice.Infrastructure.FileStorage;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns the relative path
    /// </summary>
    /// <param name="fileBytes">The file content as bytes</param>
    /// <param name="fileName">The original file name</param>
    /// <param name="folder">The folder to save the file in</param>
    /// <returns>The relative path where the file was saved</returns>
    Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string folder);

    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="filePath">The relative path of the file to delete</param>
    Task DeleteFileAsync(string filePath);

    /// <summary>
    /// Gets the URL for accessing a file
    /// </summary>
    /// <param name="filePath">The relative path of the file</param>
    /// <returns>The URL to access the file</returns>
    string GetFileUrl(string filePath);
} 