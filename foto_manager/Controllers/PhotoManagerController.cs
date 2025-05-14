using Microsoft.AspNetCore.Mvc;
using foto_list.Models;
using foto_list.Services;
using foto_list.Interfaces;
using foto_manager.Utils;

namespace foto_list.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoManagerController : ControllerBase
    {
        private readonly IFotoManger _photoManager;

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
            path = new string(path.Where(c => !invalidChars.Contains(c)).ToArray());

            return path;
        }

        public PhotoManagerController(IFotoManger photoManager)
        {
            _photoManager = photoManager;
        }

        [HttpGet("createList")]
        public async Task<ActionResult> CreatePhotoList([FromQuery] string photoFolderPath)
        {
            photoFolderPath = NormalizePath(photoFolderPath);
            var listFileName = Path.Combine(Path.GetTempPath(), ConstDef.ConstlistFileName);
            var result = await _photoManager.CreateListFileAsync(listFileName, photoFolderPath);
            
            if (result == ConstDef.ConstErrFotoPath)
                return BadRequest("Invalid photo folder path");

            if (!System.IO.File.Exists(listFileName))
                return NotFound("File not found");

            return File(System.IO.File.OpenRead(listFileName), "text/plain", ConstDef.ConstlistFileName);
        }

        [HttpGet("generateDiffReport")]
        public async Task<ActionResult> GenerateDiffReport([FromQuery] string listFilePath, [FromQuery] string photoFolderPath, [FromQuery] string reportType = "baseline")
        {
            listFilePath = NormalizePath(listFilePath);
            photoFolderPath = NormalizePath(photoFolderPath);
            var result = await _photoManager.GenerateDiffReportsAsync(listFilePath, photoFolderPath);
            
            if (result == ConstDef.ConstErrFotoPath)
                return BadRequest("Invalid photo folder path");
            if (result == ConstDef.ConstErrFotolistFile)
                return BadRequest("Invalid list file");

            var tempPath = Path.GetTempPath();
            var fileName = reportType.ToLower() == "target" ? ConstDef.ConstTargetDiffFileName : ConstDef.ConstBaselineDiffFileName;
            var filePath = Path.Combine(tempPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Report file not found");

            return File(System.IO.File.OpenRead(filePath), "text/plain", fileName);
        }

        [HttpGet("cleanPhotos")]
        public async Task<ActionResult> CleanPhotos([FromQuery] string listFilePath, [FromQuery] string photoFolderPath)
        {
            listFilePath = NormalizePath(listFilePath);
            photoFolderPath = NormalizePath(photoFolderPath);
            var reportFileName = ConstDef.ConstRemovedFileName;
            var result = await _photoManager.CleanPhotoAsync(listFilePath, reportFileName, photoFolderPath);
            
            if (result == ConstDef.ConstErrFotoPath)
                return BadRequest("Invalid photo folder path");
            if (result == ConstDef.ConstErrFotolistFile)
                return BadRequest("Invalid list file");

            var filePath = Path.Combine(Path.GetTempPath(), reportFileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Report file not found");

            return File(System.IO.File.OpenRead(filePath), "text/plain", reportFileName);
        }
    }
}