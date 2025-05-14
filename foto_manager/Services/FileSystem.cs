using System.IO;
using foto_list.Interfaces;

namespace foto_list.Services
{
    public class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public Task<bool> DirectoryExistsAsync(string path)
        {
            return Task.FromResult(Directory.Exists(path));
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public Task<string[]> GetFilesAsync(string path, string searchPattern)
        {
            return Task.FromResult(Directory.GetFiles(path, searchPattern));
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public Task<string[]> GetDirectoriesAsync(string path)
        {
            return Task.FromResult(Directory.GetDirectories(path));
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public Task CreateDirectoryAsync(string path)
        {
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public Task<bool> FileExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public StreamReader OpenText(string path)
        {
            return File.OpenText(path);
        }

        public Task<StreamReader> OpenTextAsync(string path)
        {
            return Task.FromResult(File.OpenText(path));
        }

        public StreamWriter CreateText(string path)
        {
            return File.CreateText(path);
        }

        public Task<StreamWriter> CreateTextAsync(string path)
        {
            return Task.FromResult(File.CreateText(path));
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public void MoveFile(string sourcePath, string targetPath)
        {
            File.Move(sourcePath, targetPath);
        }

        public Task MoveFileAsync(string sourcePath, string targetPath)
        {
            File.Move(sourcePath, targetPath);
            return Task.CompletedTask;
        }
    }
}