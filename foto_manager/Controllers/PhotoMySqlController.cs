using Microsoft.AspNetCore.Mvc;
using foto_manager.Models;
using foto_manager.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace foto_manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoMySqlController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotoMySqlController(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        /// <summary>
        /// 获取所有照片
        /// </summary>
        /// <returns>照片列表</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photo>>> GetAllPhotos()
        {
            var photos = await _photoRepository.GetAllPhotosAsync();
            return Ok(photos);
        }

        /// <summary>
        /// 根据ID获取照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>照片对象</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhotoById(long id)
        {
            var photo = await _photoRepository.GetPhotoByIdAsync(id);
            if (photo == null)
                return NotFound($"照片ID {id} 不存在");

            return Ok(photo);
        }

        /// <summary>
        /// 根据设备名称获取照片
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <returns>照片列表</returns>
        [HttpGet("device/{deviceName}")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosByDeviceName(string deviceName)
        {
            var photos = await _photoRepository.GetPhotosByDeviceNameAsync(deviceName);
            return Ok(photos);
        }

        /// <summary>
        /// 根据相册名称获取照片
        /// </summary>
        /// <param name="albumName">相册名称</param>
        /// <returns>照片列表</returns>
        [HttpGet("album/{albumName}")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosByAlbumName(string albumName)
        {
            var photos = await _photoRepository.GetPhotosByAlbumNameAsync(albumName);
            return Ok(photos);
        }

        /// <summary>
        /// 添加照片
        /// </summary>
        /// <param name="photo">照片对象</param>
        /// <returns>添加的照片</returns>
        [HttpPost]
        public async Task<ActionResult<Photo>> AddPhoto(Photo photo)
        {
            if (photo == null)
                return BadRequest("照片数据不能为空");

            var result = await _photoRepository.AddPhotoAsync(photo);
            return CreatedAtAction(nameof(GetPhotoById), new { id = result.Id }, result);
        }

        /// <summary>
        /// 更新照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <param name="photo">照片对象</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoto(long id, Photo photo)
        {
            if (id != photo.Id)
                return BadRequest("照片ID不匹配");

            var existingPhoto = await _photoRepository.GetPhotoByIdAsync(id);
            if (existingPhoto == null)
                return NotFound($"照片ID {id} 不存在");

            var result = await _photoRepository.UpdatePhotoAsync(photo);
            if (!result)
                return StatusCode(500, "更新照片失败");

            return NoContent();
        }

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(long id)
        {
            var existingPhoto = await _photoRepository.GetPhotoByIdAsync(id);
            if (existingPhoto == null)
                return NotFound($"照片ID {id} 不存在");

            var result = await _photoRepository.DeletePhotoAsync(id);
            if (!result)
                return StatusCode(500, "删除照片失败");

            return NoContent();
        }

        /// <summary>
        /// 批量添加照片
        /// </summary>
        /// <param name="photos">照片列表</param>
        /// <returns>添加结果</returns>
        [HttpPost("batch")]
        public async Task<IActionResult> AddPhotos(IEnumerable<Photo> photos)
        {
            if (photos == null || !photos.Any())
                return BadRequest("照片数据不能为空");

            var result = await _photoRepository.AddPhotosAsync(photos);
            if (!result)
                return StatusCode(500, "批量添加照片失败");

            return Ok("批量添加照片成功");
        }

        /// <summary>
        /// 根据文件状态获取照片
        /// </summary>
        /// <param name="fileStatus">文件状态</param>
        /// <returns>照片列表</returns>
        [HttpGet("status/{fileStatus}")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosByFileStatus(string fileStatus)
        {
            var photos = await _photoRepository.GetPhotosByFileStatusAsync(fileStatus);
            return Ok(photos);
        }
    }
}