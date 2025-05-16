namespace foto_list.Interfaces
{
    public interface IFotoManger
    {
        // Original methods for backward compatibility
        Task<string> CreateListFileAsync(string listFileName, string photoFolderPath);
        Task<string> GenerateDiffReportsAsync(string listFileName, string photoFolderPath, string reportFileName = "");
        Task<string> CleanPhotoAsync(string listFileName, string reportFileName, string photoFolderPath);
        
        // New methods that use StreamWriter instead of file paths
        Task<string> CreateListFileAsync(StreamWriter writer, string photoFolderPath);
        Task<string> GenerateDiffReportsAsync(string listFilePath, string photoFolderPath, StreamWriter baselineWriter, StreamWriter targetWriter);
        Task<string> CleanPhotoAsync(string listFilePath, StreamWriter writer, string photoFolderPath);
    }
}