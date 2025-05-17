using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using foto_manager.Data;

namespace foto_manager.Services
{
    /// <summary>
    /// 数据库迁移服务，用于在应用启动时自动创建或更新数据库结构
    /// </summary>
    public static class DatabaseMigrationService
    {
        /// <summary>
        /// 在应用启动时执行数据库迁移
        /// </summary>
        /// <param name="host">应用主机</param>
        /// <returns>应用主机</returns>
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<PhotoManagerDbContext>();
                    
                    // 确保数据库已创建
                    context.Database.EnsureCreated();
                    
                    // 如果有待处理的迁移，则应用它们
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<PhotoManagerDbContext>>();
                    logger.LogError(ex, "数据库迁移过程中发生错误");
                }
            }
            return host;
        }
    }
}