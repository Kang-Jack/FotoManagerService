using System.Collections.Specialized;
using foto_list.Interfaces;
using foto_manager.Utils;

namespace foto_list.Services
{
    public class FotoManager : IFotoManger
    {
        private readonly IFileSystem _fileSystem;

        public FotoManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        // New implementation using StreamWriter instead of file path
        public async Task<string> CreateListFileAsync(StreamWriter writer, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allFiles = new StringCollection();
            await ListAllFilesAsync(allFiles, fullPath, "*.*", true);

            // Write directly to the provided StreamWriter
            foreach (var file in allFiles)
            {
                await writer.WriteLineAsync(file);
            }
            await writer.FlushAsync();

            return "Success";
        }

        public async Task<string> CreateListFileAsync(string listFileName, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allFiles = new StringCollection();
            await ListAllFilesAsync(allFiles, fullPath, "*.*", true);

            return await WriteListFileAsync(listFileName, allFiles);     
        }

        public async Task<string> GenerateDiffReportsAsync(string listFileName, string photoFolderPath, string reportFileName = "")
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allPhotosInBaseline = new StringCollection();

            if (!await ReadListInFileAsync(listFileName, allPhotosInBaseline))
                return ConstDef.ConstErrFotolistFile;

            StringCollection allFilesInTarget = new StringCollection();
            await ListAllFilesAsync(allFilesInTarget, fullPath, "*.*", true);

            StringCollection allMissingFileInTarget = new StringCollection();
            StringCollection allMissingFileInBaseline = new StringCollection();

            foreach (var name in allPhotosInBaseline)
            {
                if (!allFilesInTarget.Contains(name))
                    allMissingFileInTarget.Add(name);
            }

            foreach (var name in allFilesInTarget)
            {
                if (!allPhotosInBaseline.Contains(name))
                    allMissingFileInBaseline.Add(name);
            }

            var tempPath = Path.GetTempPath();
            var baselineDiffFileName = string.IsNullOrEmpty(reportFileName) 
                ? _fileSystem.Combine(tempPath, ConstDef.ConstBaselineDiffFileName)
                : _fileSystem.Combine(tempPath, $"{_fileSystem.GetFileNameWithoutExtension(reportFileName)}_baseline{_fileSystem.GetExtension(reportFileName)}");
            var targetDiffFileName = string.IsNullOrEmpty(reportFileName)
                ? _fileSystem.Combine(tempPath, ConstDef.ConstTargetDiffFileName)
                : _fileSystem.Combine(tempPath, $"{_fileSystem.GetFileNameWithoutExtension(reportFileName)}_target{_fileSystem.GetExtension(reportFileName)}");

            var result = await WriteListFileAsync(baselineDiffFileName, allMissingFileInBaseline);
            result += "  " + await WriteListFileAsync(targetDiffFileName, allMissingFileInTarget);

            return result;
        }
        
        // New implementation using StreamWriters instead of file paths
        public async Task<string> GenerateDiffReportsAsync(string listFilePath, string photoFolderPath, StreamWriter baselineWriter, StreamWriter targetWriter)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allPhotosInBaseline = new StringCollection();

            if (!await ReadListInFileAsync(listFilePath, allPhotosInBaseline))
                return ConstDef.ConstErrFotolistFile;

            StringCollection allFilesInTarget = new StringCollection();
            await ListAllFilesAsync(allFilesInTarget, fullPath, "*.*", true);

            StringCollection allMissingFileInTarget = new StringCollection();
            StringCollection allMissingFileInBaseline = new StringCollection();

            foreach (var name in allPhotosInBaseline)
            {
                if (!allFilesInTarget.Contains(name))
                    allMissingFileInTarget.Add(name);
            }

            foreach (var name in allFilesInTarget)
            {
                if (!allPhotosInBaseline.Contains(name))
                    allMissingFileInBaseline.Add(name);
            }

            // Write directly to the provided StreamWriters
            foreach (var file in allMissingFileInBaseline)
            {
                await baselineWriter.WriteLineAsync(file);
            }
            await baselineWriter.FlushAsync();

            foreach (var file in allMissingFileInTarget)
            {
                await targetWriter.WriteLineAsync(file);
            }
            await targetWriter.FlushAsync();

            return "Success";
        }

        public async Task<string> CleanPhotoAsync(string listFileName, string reportFileName, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allPhotos = new StringCollection();

            if (!await ReadListInFileAsync(listFileName, allPhotos))
                return ConstDef.ConstErrFotolistFile;

            StringCollection removedFiles = new StringCollection();
            await CleanAllFilesAsync(allPhotos, removedFiles, fullPath, "*.*", true);

            var tempPath = Path.GetTempPath();
            var removedFileReport = _fileSystem.Combine(tempPath, reportFileName);
            return await WriteListFileAsync(removedFileReport, removedFiles);
        }
        
        // New implementation using StreamWriter instead of file path
        public async Task<string> CleanPhotoAsync(string listFilePath, StreamWriter writer, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allPhotos = new StringCollection();

            if (!await ReadListInFileAsync(listFilePath, allPhotos))
                return ConstDef.ConstErrFotolistFile;

            StringCollection removedFiles = new StringCollection();
            await CleanAllFilesAsync(allPhotos, removedFiles, fullPath, "*.*", true);

            // Write directly to the provided StreamWriter
            foreach (var file in removedFiles)
            {
                await writer.WriteLineAsync(file);
            }
            await writer.FlushAsync();

            return "Success";
        }

        private async Task ListAllFilesAsync(StringCollection allFiles, string path, string pattern, bool recursive)
        {
            var files = await _fileSystem.GetFilesAsync(path, pattern);
            foreach (var file in files)
            {
                allFiles.Add(_fileSystem.GetFileName(file));
            }

            if (recursive)
            {
                var directories = await _fileSystem.GetDirectoriesAsync(path);
                foreach (var directory in directories)
                {
                    await ListAllFilesAsync(allFiles, directory, pattern, recursive);
                }
            }
        }

        private async Task CleanAllFilesAsync(StringCollection allPhotos, StringCollection removedFiles, string path, string pattern, bool recursive)
        {
            var files = await _fileSystem.GetFilesAsync(path, pattern);
            foreach (var file in files)
            {
                var fileName = _fileSystem.GetFileName(file);
                if (!allPhotos.Contains(fileName))
                {
                    var removedFolderPath = _fileSystem.Combine(path, ConstDef.ConstTempRemoveFolderName);
                    await _fileSystem.CreateDirectoryAsync(removedFolderPath);
                    var targetPath = _fileSystem.Combine(removedFolderPath, fileName);
                    await _fileSystem.MoveFileAsync(file, targetPath);
                    removedFiles.Add(fileName);
                }
            }

            if (recursive)
            {
                var directories = await _fileSystem.GetDirectoriesAsync(path);
                foreach (var directory in directories)
                {
                    if (!_fileSystem.GetFileName(directory).Equals(ConstDef.ConstTempRemoveFolderName))
                        await CleanAllFilesAsync(allPhotos, removedFiles, directory, pattern, recursive);
                }
            }
        }

        private async Task<bool> ReadListInFileAsync(string listFileName, StringCollection allPhotos)
        {
            if (!await _fileSystem.FileExistsAsync(listFileName))
                return false;

            using (var reader = await _fileSystem.OpenTextAsync(listFileName))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                        allPhotos.Add(line);
                }
            }

            return true;
        }

        private async Task<string> WriteListFileAsync(string listFileName, StringCollection allFiles)
        {
            try
            {
                using (var writer = await _fileSystem.CreateTextAsync(listFileName))
                {
                    foreach (var file in allFiles)
                    {
                        await writer.WriteLineAsync(file);
                    }
                }

                return ConstDef.ConstMesgReturnList + listFileName;
            }
            catch (Exception ex)
            {
                return ConstDef.ConstErrWriteFile + ex.Message;
            }
        }
    }
}