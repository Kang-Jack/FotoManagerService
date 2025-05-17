using foto_list.Services;
using foto_list.Interfaces;
using foto_manager.Data;
using foto_manager.Repositories;
using foto_manager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置MySQL数据库连接
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? 
    "server=localhost;port=3306;database=photomanager;user=photoman;password=photoman";

// 注册DbContext
builder.Services.AddDbContext<PhotoManagerDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 注册仓储
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();

// 注册原有依赖
builder.Services.AddSingleton<IFileSystem, FileSystem>();

// 根据配置选择使用哪种实现
var useMySql = builder.Configuration.GetValue<bool>("UseMySql", false);
if (useMySql)
{
    // 使用MySQL版本的照片管理服务
    builder.Services.AddScoped<IFotoManger, MySqlPhotoManager>();
}
else
{
    // 使用原始文件系统版本的照片管理服务
    builder.Services.AddScoped<IFotoManger, FotoManager>();
}

var app = builder.Build();

// 执行数据库迁移（当UseMySql为true时）
if (useMySql)
{
    app.MigrateDatabase();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

