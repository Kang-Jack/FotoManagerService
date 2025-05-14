namespace foto_list.Interfaces
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        Task<bool> DirectoryExistsAsync(string path);
        string GetFullPath(string path);
        string[] GetFiles(string path, string searchPattern);
        Task<string[]> GetFilesAsync(string path, string searchPattern);
        string[] GetDirectories(string path);
        Task<string[]> GetDirectoriesAsync(string path);
        void CreateDirectory(string path);
        Task CreateDirectoryAsync(string path);
        bool FileExists(string path);
        Task<bool> FileExistsAsync(string path);
        StreamReader OpenText(string path);
        Task<StreamReader> OpenTextAsync(string path);
        StreamWriter CreateText(string path);
        Task<StreamWriter> CreateTextAsync(string path);
        string Combine(params string[] paths);
        string GetFileNameWithoutExtension(string path);
        string GetFileName(string path);
        string GetExtension(string path);
        void MoveFile(string sourcePath, string targetPath);
        Task MoveFileAsync(string sourcePath, string targetPath);
    }
}