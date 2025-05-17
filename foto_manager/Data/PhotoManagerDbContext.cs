using Microsoft.EntityFrameworkCore;
using foto_manager.Models;

namespace foto_manager.Data
{
    /// <summary>
    /// 照片管理数据库上下文，用于配置Entity Framework Core与MySQL的连接
    /// </summary>
    public class PhotoManagerDbContext : DbContext
    {
        public PhotoManagerDbContext(DbContextOptions<PhotoManagerDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 照片数据集
        /// </summary>
        public DbSet<Photo> Photos { get; set; } = null!;

        /// <summary>
        /// 配置实体映射
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置Photo实体映射到photos表
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("photos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DeviceName).HasColumnName("device_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.AlbumName).HasColumnName("album_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.FileName).HasColumnName("file_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.FileExtension).HasColumnName("file_extension").IsRequired().HasMaxLength(100);
                entity.Property(e => e.FileStatus).HasColumnName("file_status").IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}