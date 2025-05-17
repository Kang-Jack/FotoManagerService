using Microsoft.EntityFrameworkCore;
using foto_manager.Data;
using foto_manager.Models;

namespace foto_manager.Repositories
{
    /// <summary>
    /// 照片仓储实现类，使用Entity Framework Core操作MySQL数据库
    /// </summary>
    public class PhotoRepository : IPhotoRepository
    {
        private readonly PhotoManagerDbContext _context;

        public PhotoRepository(PhotoManagerDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有照片
        /// </summary>
        /// <returns>照片列表</returns>
        public async Task<IEnumerable<Photo>> GetAllPhotosAsync()
        {
            return await _context.Photos.ToListAsync();
        }

        /// <summary>
        /// 根据ID获取照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>照片对象</returns>
        public async Task<Photo?> GetPhotoByIdAsync(long id)
        {
            return await _context.Photos.FindAsync(id);
        }

        /// <summary>
        /// 根据设备名称获取照片
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <returns>照片列表</returns>
        public async Task<IEnumerable<Photo>> GetPhotosByDeviceNameAsync(string deviceName)
        {
            return await _context.Photos
                .Where(p => p.DeviceName == deviceName)
                .ToListAsync();
        }

        /// <summary>
        /// 根据相册名称获取照片
        /// </summary>
        /// <param name="albumName">相册名称</param>
        /// <returns>照片列表</returns>
        public async Task<IEnumerable<Photo>> GetPhotosByAlbumNameAsync(string albumName)
        {
            return await _context.Photos
                .Where(p => p.AlbumName == albumName)
                .ToListAsync();
        }

        /// <summary>
        /// 添加照片
        /// </summary>
        /// <param name="photo">照片对象</param>
        /// <returns>添加的照片</returns>
        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        /// <summary>
        /// 更新照片
        /// </summary>
        /// <param name="photo">照片对象</param>
        /// <returns>更新结果</returns>
        public async Task<bool> UpdatePhotoAsync(Photo photo)
        {
            _context.Entry(photo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>删除结果</returns>
        public async Task<bool> DeletePhotoAsync(long id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
                return false;

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 批量添加照片
        /// </summary>
        /// <param name="photos">照片列表</param>
        /// <returns>添加结果</returns>
        public async Task<bool> AddPhotosAsync(IEnumerable<Photo> photos)
        {
            _context.Photos.AddRange(photos);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据文件状态获取照片
        /// </summary>
        /// <param name="fileStatus">文件状态</param>
        /// <returns>照片列表</returns>
        public async Task<IEnumerable<Photo>> GetPhotosByFileStatusAsync(string fileStatus)
        {
            return await _context.Photos
                .Where(p => p.FileStatus == fileStatus)
                .ToListAsync();
        }
    }
}