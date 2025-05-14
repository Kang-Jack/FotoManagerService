# 照片管理Web服务

这是一个用于管理照片的Web API服务，基于.NET 8构建。该服务提供照片列表创建、差异比较和清理功能。

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

## 启动服务

1. 克隆仓库
2. 使用Visual Studio打开解决方案
3. 运行项目
4. 访问 https://localhost:5001/swagger 查看API文档
