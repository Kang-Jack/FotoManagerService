using System.Collections.Specialized;
using foto_list.Interfaces;
using foto_manager.Models;
using foto_manager.Repositories;
using foto_manager.Utils;

namespace foto_manager.Services
{
    /// <summary>
    /// MySQL版本的照片管理服务实现
    /// </summary>
    public class MySqlPhotoManager : IFotoManger
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPhotoRepository _photoRepository;

        public MySqlPhotoManager(IFileSystem fileSystem, IPhotoRepository photoRepository)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
        }

        /// <summary>
        /// 创建照片列表文件，并将照片信息保存到MySQL数据库
        /// </summary>
        /// <param name="listFileName">列表文件名</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <returns>操作结果</returns>
        public async Task<string> CreateListFileAsync(string listFileName, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allFiles = new StringCollection();
            await ListAllFilesAsync(allFiles, fullPath, "*.*", true);

            // 将照片信息保存到数据库
            var photos = new List<Photo>();
            foreach (var file in allFiles)
            {
                var fileInfo = new FileInfo(file);
                var photo = new Photo
                {
                    DeviceName = Environment.MachineName,
                    AlbumName = Path.GetDirectoryName(file) ?? string.Empty,
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FileExtension = fileInfo.Extension,
                    FileStatus = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                photos.Add(photo);
            }

            // 批量添加到数据库
            await _photoRepository.AddPhotosAsync(photos);

            // 同时写入文件以保持兼容性
            return await WriteListFileAsync(listFileName, allFiles);
        }

        /// <summary>
        /// 创建照片列表文件，并将照片信息保存到MySQL数据库
        /// </summary>
        /// <param name="writer">文件写入器</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <returns>操作结果</returns>
        public async Task<string> CreateListFileAsync(StreamWriter writer, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(photoFolderPath);
            StringCollection allFiles = new StringCollection();
            await ListAllFilesAsync(allFiles, fullPath, "*.*", true);

            // 将照片信息保存到数据库
            var photos = new List<Photo>();
            foreach (var file in allFiles)
            {
                var fileInfo = new FileInfo(file);
                var photo = new Photo
                {
                    DeviceName = Environment.MachineName,
                    AlbumName = Path.GetDirectoryName(file) ?? string.Empty,
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FileExtension = fileInfo.Extension,
                    FileStatus = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                photos.Add(photo);

                // 写入文件
                await writer.WriteLineAsync(file);
            }

            // 批量添加到数据库
            await _photoRepository.AddPhotosAsync(photos);
            await writer.FlushAsync();

            return "Success";
        }

        /// <summary>
        /// 生成差异报告
        /// </summary>
        /// <param name="listFileName">列表文件名</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <param name="reportFileName">报告文件名</param>
        /// <returns>操作结果</returns>
        public async Task<string> GenerateDiffReportsAsync(string listFileName, string photoFolderPath, string reportFileName = "")
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            if (string.IsNullOrEmpty(listFileName) || !await _fileSystem.FileExistsAsync(listFileName))
                return ConstDef.ConstErrFotolistFile;

            var baselineDir = Path.GetDirectoryName(listFileName);
            var baselineDiffFile = string.IsNullOrEmpty(reportFileName) ?
                Path.Combine(baselineDir ?? string.Empty, ConstDef.ConstBaselineDiffFileName) :
                Path.Combine(baselineDir ?? string.Empty, $"{Path.GetFileNameWithoutExtension(reportFileName)}_baseline.txt");

            var targetDiffFile = string.IsNullOrEmpty(reportFileName) ?
                Path.Combine(baselineDir ?? string.Empty, ConstDef.ConstTargetDiffFileName) :
                Path.Combine(baselineDir ?? string.Empty, $"{Path.GetFileNameWithoutExtension(reportFileName)}_target.txt");

            using var baselineWriter = new StreamWriter(baselineDiffFile, false);
            using var targetWriter = new StreamWriter(targetDiffFile, false);

            return await GenerateDiffReportsAsync(listFileName, photoFolderPath, baselineWriter, targetWriter);
        }

        /// <summary>
        /// 生成差异报告
        /// </summary>
        /// <param name="listFilePath">列表文件路径</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <param name="baselineWriter">基准写入器</param>
        /// <param name="targetWriter">目标写入器</param>
        /// <returns>操作结果</returns>
        public async Task<string> GenerateDiffReportsAsync(string listFilePath, string photoFolderPath, StreamWriter baselineWriter, StreamWriter targetWriter)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstErrFotoPath;

            if (string.IsNullOrEmpty(listFilePath) || !await _fileSystem.FileExistsAsync(listFilePath))
                return ConstDef.ConstErrFotolistFile;

            // 从数据库获取基准照片列表
            var baselinePhotos = await _photoRepository.GetAllPhotosAsync();
            var baselinePhotoNames = baselinePhotos.Select(p => $"{p.FileName}{p.FileExtension}").ToList();

            // 获取目标文件夹中的照片
            var targetFiles = await _fileSystem.GetFilesAsync(photoFolderPath, "*.*");
            var targetPhotoNames = targetFiles.Select(Path.GetFileName).ToList();

            // 找出基准中有但目标中没有的照片
            var missingInTarget = baselinePhotoNames.Except(targetPhotoNames);
            foreach (var photo in missingInTarget)
            {
                await baselineWriter.WriteLineAsync(photo);
            }

            // 找出目标中有但基准中没有的照片
            var missingInBaseline = targetPhotoNames.Except(baselinePhotoNames);
            foreach (var photo in missingInBaseline)
            {
                await targetWriter.WriteLineAsync(photo);
            }

            await baselineWriter.FlushAsync();
            await targetWriter.FlushAsync();

            return "Success";
        }

        /// <summary>
        /// 清理照片
        /// </summary>
        /// <param name="listFileName">列表文件名</param>
        /// <param name="reportFileName">报告文件名</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <returns>操作结果</returns>
        public async Task<string> CleanPhotoAsync(string listFileName, string reportFileName, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstInvalidFotoPath;

            if (string.IsNullOrEmpty(listFileName) || !await _fileSystem.FileExistsAsync(listFileName))
                return ConstDef.ConstInvalidListPath;

            var reportFilePath = Path.Combine(Path.GetDirectoryName(listFileName) ?? string.Empty, reportFileName);
            using var writer = new StreamWriter(reportFilePath, false);

            return await CleanPhotoAsync(listFileName, writer, photoFolderPath);
        }

        /// <summary>
        /// 清理照片
        /// </summary>
        /// <param name="listFilePath">列表文件路径</param>
        /// <param name="writer">写入器</param>
        /// <param name="photoFolderPath">照片文件夹路径</param>
        /// <returns>操作结果</returns>
        public async Task<string> CleanPhotoAsync(string listFilePath, StreamWriter writer, string photoFolderPath)
        {
            if (string.IsNullOrEmpty(photoFolderPath) || !await _fileSystem.DirectoryExistsAsync(photoFolderPath))
                return ConstDef.ConstInvalidFotoPath;

            if (string.IsNullOrEmpty(listFilePath) || !await _fileSystem.FileExistsAsync(listFilePath))
                return ConstDef.ConstInvalidListPath;

            // 从数据库获取照片列表
            var baselinePhotos = await _photoRepository.GetAllPhotosAsync();
            var baselinePhotoNames = baselinePhotos.Select(p => $"{p.FileName}{p.FileExtension}").ToList();

            // 获取目标文件夹中的照片
            var targetFiles = await _fileSystem.GetFilesAsync(photoFolderPath, "*.*");

            // 创建临时移除文件夹
            var removedFolderPath = Path.Combine(photoFolderPath, ConstDef.ConstTempRemoveFolderName);
            if (!await _fileSystem.DirectoryExistsAsync(removedFolderPath))
                await _fileSystem.CreateDirectoryAsync(removedFolderPath);

            // 移动不在基准列表中的文件到移除文件夹
            foreach (var file in targetFiles)
            {
                var fileName = Path.GetFileName(file);
                if (!baselinePhotoNames.Contains(fileName))
                {
                    var destPath = Path.Combine(removedFolderPath, fileName);
                    await _fileSystem.MoveFileAsync(file, destPath);
                    await writer.WriteLineAsync(file);

                    // 更新数据库中的文件状态
                    var photoName = Path.GetFileNameWithoutExtension(file);
                    var photoExt = Path.GetExtension(file);
                    var photo = baselinePhotos.FirstOrDefault(p => p.FileName == photoName && p.FileExtension == photoExt);
                    if (photo != null)
                    {
                        photo.FileStatus = "Removed";
                        photo.UpdatedAt = DateTime.UtcNow;
                        await _photoRepository.UpdatePhotoAsync(photo);
                    }
                }
            }

            await writer.FlushAsync();
            return ConstDef.ConstMesgReturnList + writer.BaseStream;
        }

        /// <summary>
        /// 列出所有文件
        /// </summary>
        /// <param name="allFiles">文件集合</param>
        /// <param name="rootPath">根路径</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <param name="includeSubFolders">是否包含子文件夹</param>
        /// <returns>异步任务</returns>
        private async Task ListAllFilesAsync(StringCollection allFiles, string rootPath, string searchPattern, bool includeSubFolders)
        {
            var files = await _fileSystem.GetFilesAsync(rootPath, searchPattern);
            foreach (var file in files)
            {
                allFiles.Add(file);
            }

            if (includeSubFolders)
            {
                var subFolders = await _fileSystem.GetDirectoriesAsync(rootPath);
                foreach (var folder in subFolders)
                {
                    await ListAllFilesAsync(allFiles, folder, searchPattern, includeSubFolders);
                }
            }
        }

        /// <summary>
        /// 写入列表文件
        /// </summary>
        /// <param name="listFileName">列表文件名</param>
        /// <param name="allFiles">文件集合</param>
        /// <returns>操作结果</returns>
        private async Task<string> WriteListFileAsync(string listFileName, StringCollection allFiles)
        {
            try
            {
                using var writer = new StreamWriter(listFileName, false);
                foreach (var file in allFiles)
                {
                    await writer.WriteLineAsync(file);
                }
                await writer.FlushAsync();
                return ConstDef.ConstMesgReturnList + listFileName;
            }
            catch (Exception ex)
            {
                return ConstDef.ConstErrWriteFile + ex.Message;
            }
        }
    }
}