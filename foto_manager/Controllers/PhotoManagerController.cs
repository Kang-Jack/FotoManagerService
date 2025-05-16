using Microsoft.AspNetCore.Mvc;
using foto_list.Models;
using foto_list.Services;
using foto_list.Interfaces;
using foto_manager.Utils;
using System.Text;

namespace foto_list.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoManagerController : ControllerBase
    {
        private readonly IFotoManger _photoManager;

        /// <summary>
        /// 检查路径是否有效
        /// </summary>
        /// <param name="path">要检查的路径</param>
        /// <returns>如果路径有效返回true，否则返回false</returns>
        private bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            
            // 检查是否包含不安全字符
            var invalidChars = Path.GetInvalidPathChars();
            var additionalInvalidChars = new[] { '<', '>', '|', '?', '*' };
            
            // 允许冒号（:）在Windows路径中，但不允许在文件名中
            // 允许引号（"）在测试路径中
            return !path.Any(c => invalidChars.Contains(c) || additionalInvalidChars.Contains(c));
        }
        
        /// <summary>
        /// 标准化路径格式
        /// </summary>
        /// <param name="path">要标准化的路径</param>
        /// <returns>标准化后的路径</returns>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            // 解码URL并移除引号
            path = Uri.UnescapeDataString(path?.Trim('"') ?? string.Empty);

            // 替换路径分隔符为当前系统的分隔符
            path = path.Replace('\\', Path.DirectorySeparatorChar)
                       .Replace('/', Path.DirectorySeparatorChar);

            // 移除不安全的字符
            var invalidChars = Path.GetInvalidPathChars();
            var additionalInvalidChars = new[] { '<', '>', '|', '?', '*' };
            path = new string(path.Where(c => !invalidChars.Contains(c) && !additionalInvalidChars.Contains(c)).ToArray());

            return path;
        }

        public PhotoManagerController(IFotoManger photoManager)
        {
            _photoManager = photoManager;
        }

        [HttpGet("createList")]
        public async Task<ActionResult> CreatePhotoList([FromQuery] string photoFolderPath)
        {
            // 先验证路径
            if (!IsValidPath(photoFolderPath))
                return BadRequest(ConstDef.ConstInvalidFotoPath);
                
            // 再标准化路径
            photoFolderPath = NormalizePath(photoFolderPath);
            
            // Use memory stream instead of temp file
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            {
                var result = await _photoManager.CreateListFileAsync(writer, photoFolderPath);
                
                if (result == ConstDef.ConstErrFotoPath)
                    return BadRequest(ConstDef.ConstInvalidFotoPath);
            }
            
            // Reset position to beginning of stream for reading
            memoryStream.Position = 0;
            
            return File(memoryStream, "text/plain", ConstDef.ConstlistFileName);
        }

        [HttpGet("generateDiffReport")]
        public async Task<ActionResult> GenerateDiffReport([FromQuery] string listFilePath, [FromQuery] string photoFolderPath, [FromQuery] string reportType = "baseline")
        {
            // 先验证路径
            if (!IsValidPath(listFilePath))
                return BadRequest("Invalid list file path");
                
            if (!IsValidPath(photoFolderPath))
                return BadRequest(ConstDef.ConstInvalidFotoPath);
                
            // 再标准化路径
            listFilePath = NormalizePath(listFilePath);
            photoFolderPath = NormalizePath(photoFolderPath);
            
            // Use memory streams for both baseline and target reports
            var baselineStream = new MemoryStream();
            var targetStream = new MemoryStream();
            
            using (var baselineWriter = new StreamWriter(baselineStream, Encoding.UTF8, leaveOpen: true))
            using (var targetWriter = new StreamWriter(targetStream, Encoding.UTF8, leaveOpen: true))
            {
                var result = await _photoManager.GenerateDiffReportsAsync(listFilePath, photoFolderPath, baselineWriter, targetWriter);
                
                if (result == ConstDef.ConstErrFotoPath)
                    return BadRequest(ConstDef.ConstInvalidFotoPath);
                if (result == ConstDef.ConstErrFotolistFile)
                    return BadRequest("Invalid list file");
            }
            
            // Determine which stream to return based on report type
            var fileName = reportType.ToLower() == "target" ? ConstDef.ConstTargetDiffFileName : ConstDef.ConstBaselineDiffFileName;
            var responseStream = reportType.ToLower() == "target" ? targetStream : baselineStream;
            
            // Reset position to beginning of stream for reading
            responseStream.Position = 0;
            
            return File(responseStream, "text/plain", fileName);
        }

        [HttpGet("cleanPhotos")]
        public async Task<ActionResult> CleanPhotos([FromQuery] string listFilePath, [FromQuery] string photoFolderPath)
        {
            // 先验证路径
            if (!IsValidPath(listFilePath))
                return BadRequest(ConstDef.ConstInvalidListPath);
                
            if (!IsValidPath(photoFolderPath))
                return BadRequest(ConstDef.ConstInvalidFotoPath);
                
            // 再标准化路径
            listFilePath = NormalizePath(listFilePath);
            photoFolderPath = NormalizePath(photoFolderPath);
            var reportFileName = ConstDef.ConstRemovedFileName;
            
            // Use memory stream instead of temp file
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            {
                var result = await _photoManager.CleanPhotoAsync(listFilePath, writer, photoFolderPath);
                
                if (result == ConstDef.ConstErrFotoPath)
                    return BadRequest(ConstDef.ConstInvalidFotoPath);
                if (result == ConstDef.ConstErrFotolistFile)
                    return BadRequest("Invalid list file");
            }
            
            // Reset position to beginning of stream for reading
            memoryStream.Position = 0;
            
            return File(memoryStream, "text/plain", reportFileName);
        }
    }
}