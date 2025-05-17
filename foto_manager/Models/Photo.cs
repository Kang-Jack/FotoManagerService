using System;

namespace foto_manager.Models
{
    /// <summary>
    /// 照片实体类，映射到MySQL数据库中的photos表
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// 照片ID，主键
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;
        
        /// <summary>
        /// 相册名称
        /// </summary>
        public string AlbumName { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件状态
        /// </summary>
        public string FileStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}