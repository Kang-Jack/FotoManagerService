using foto_manager.Models;

namespace foto_manager.Repositories
{
    /// <summary>
    /// 照片仓储接口，定义照片数据访问操作
    /// </summary>
    public interface IPhotoRepository
    {
        /// <summary>
        /// 获取所有照片
        /// </summary>
        /// <returns>照片列表</returns>
        Task<IEnumerable<Photo>> GetAllPhotosAsync();

        /// <summary>
        /// 根据ID获取照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>照片对象</returns>
        Task<Photo?> GetPhotoByIdAsync(long id);

        /// <summary>
        /// 根据设备名称获取照片
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <returns>照片列表</returns>
        Task<IEnumerable<Photo>> GetPhotosByDeviceNameAsync(string deviceName);

        /// <summary>
        /// 根据相册名称获取照片
        /// </summary>
        /// <param name="albumName">相册名称</param>
        /// <returns>照片列表</returns>
        Task<IEnumerable<Photo>> GetPhotosByAlbumNameAsync(string albumName);

        /// <summary>
        /// 添加照片
        /// </summary>
        /// <param name="photo">照片对象</param>
        /// <returns>添加结果</returns>
        Task<Photo> AddPhotoAsync(Photo photo);

        /// <summary>
        /// 更新照片
        /// </summary>
        /// <param name="photo">照片对象</param>
        /// <returns>更新结果</returns>
        Task<bool> UpdatePhotoAsync(Photo photo);

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="id">照片ID</param>
        /// <returns>删除结果</returns>
        Task<bool> DeletePhotoAsync(long id);

        /// <summary>
        /// 批量添加照片
        /// </summary>
        /// <param name="photos">照片列表</param>
        /// <returns>添加结果</returns>
        Task<bool> AddPhotosAsync(IEnumerable<Photo> photos);

        /// <summary>
        /// 根据文件状态获取照片
        /// </summary>
        /// <param name="fileStatus">文件状态</param>
        /// <returns>照片列表</returns>
        Task<IEnumerable<Photo>> GetPhotosByFileStatusAsync(string fileStatus);
    }
}