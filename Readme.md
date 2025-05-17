# 照片管理服务 (Photo Manager Service)

## 项目概述

照片管理服务是一个用于管理照片文件的WebAPI应用程序，提供照片列表创建、比较和清理功能。现在支持两种实现方式：

1. 基于文件系统的实现（默认）
2. 基于MySQL数据库的实现（新增）

## MySQL实现说明

### 数据库结构

```sql
CREATE TABLE photos (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    device_name VARCHAR(100) NOT NULL,
    album_name VARCHAR(100) NOT NULL,
    file_name VARCHAR(100) NOT NULL,
    file_extension VARCHAR(100) NOT NULL,
    file_status VARCHAR(100) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 技术实现

- 使用Entity Framework Core进行数据库操作
- 采用仓储模式分离数据访问逻辑
- 支持与原有文件系统实现共存，可通过配置切换

## 如何使用MySQL实现

### 配置数据库连接

在`appsettings.json`文件中配置MySQL数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "server=localhost;port=3306;database=photomanager;user=photoman;password=photoman"
  },
  "UseMySql": true
}
```

将`UseMySql`设置为`true`即可启用MySQL实现。

### API端点

#### 原有API端点（同时支持文件系统和MySQL实现）

- `POST /api/PhotoManager/CreateList` - 创建照片列表
- `POST /api/PhotoManager/GenerateDiffReport` - 生成差异报告
- `POST /api/PhotoManager/CleanPhoto` - 清理照片

#### MySQL专用API端点

- `GET /api/PhotoMySql` - 获取所有照片
- `GET /api/PhotoMySql/{id}` - 根据ID获取照片
- `GET /api/PhotoMySql/device/{deviceName}` - 根据设备名称获取照片
- `GET /api/PhotoMySql/album/{albumName}` - 根据相册名称获取照片
- `GET /api/PhotoMySql/status/{fileStatus}` - 根据文件状态获取照片
- `POST /api/PhotoMySql` - 添加照片
- `POST /api/PhotoMySql/batch` - 批量添加照片
- `PUT /api/PhotoMySql/{id}` - 更新照片
- `DELETE /api/PhotoMySql/{id}` - 删除照片

## API端点

### 1. 创建照片列表

```http
POST /api/PhotoManager/createList
```

请求体：
```json
{
    "photoFolderPath": "要扫描的照片文件夹路径"
}
```

响应：
```json
{
    "message": "操作结果消息",
    "listFilePath": "生成的列表文件路径"
}
```

### 2. 生成差异报告

```http
POST /api/PhotoManager/generateDiffReport
```

请求体：
```json
{
    "listFilePath": "照片列表文件路径",
    "photoFolderPath": "要比较的照片文件夹路径"
}
```

响应：
```json
{
    "message": "操作结果消息",
    "baselineDiffFilePath": "基准差异文件路径",
    "targetDiffFilePath": "目标差异文件路径"
}
```

### 3. 清理照片

```http
POST /api/PhotoManager/cleanPhotos
```

请求体：
```json
{
    "listFilePath": "照片列表文件路径",
    "photoFolderPath": "要清理的照片文件夹路径"
}
```

响应：
```json
{
    "message": "操作结果消息",
    "removedFilesReportPath": "已移除文件报告路径"
}
```

## 功能说明

1. **创建照片列表**：扫描指定文件夹中的所有照片文件，创建一个列表文件。
2. **生成差异报告**：比较基准列表文件和目标文件夹，生成两个差异报告文件：
   - 基准差异文件：记录在目标文件夹中缺失的文件
   - 目标差异文件：记录在基准列表中缺失的文件
3. **清理照片**：根据列表文件，将目标文件夹中不在列表中的照片移动到removed子文件夹。

## 错误处理

服务会返回以下HTTP状态码：

- 200 OK：操作成功
- 400 Bad Request：输入参数无效（如文件夹路径不存在、列表文件无效等）
- 500 Internal Server Error：服务器内部错误

## 开发环境要求

- .NET 8.0 SDK
- Visual Studio 2022或其他支持.NET 8的IDE

### 前提条件

- .NET 8.0 SDK
- MySQL 8.0或更高版本（如果使用MySQL实现）

## 启动服务

1. 克隆仓库
2. 使用Visual Studio打开解决方案
3. 运行项目
4. 访问 https://localhost:5001/swagger 查看API文档

## Running foto_manager with Docker

You can build and run the `foto_manager` application as a container using the provided Dockerfile:

### Build the Docker image

```sh
docker build -t foto_manager:latest .
```

### Run the Docker container

```sh
docker run --rm -p 8080:5001 foto_manager:latest
```

- The application will be available on port 8080 of your host (adjust the port mapping as needed).
- You can pass environment variables or mount volumes as needed using standard Docker options.
