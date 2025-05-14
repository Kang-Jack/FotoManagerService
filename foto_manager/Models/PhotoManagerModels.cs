namespace foto_list.Models
{
    public class CreateListRequest
    {
        public string PhotoFolderPath { get; set; } = string.Empty;
    }

    public class CreateListResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ListFilePath { get; set; } = string.Empty;
    }

    public class GenerateDiffReportRequest
    {
        public string ListFilePath { get; set; } = string.Empty;
        public string PhotoFolderPath { get; set; } = string.Empty;
    }

    public class GenerateDiffReportResponse
    {
        public string Message { get; set; } = string.Empty;
        public string BaselineDiffFilePath { get; set; } = string.Empty;
        public string TargetDiffFilePath { get; set; } = string.Empty;
    }

    public class CleanPhotoRequest
    {
        public string ListFilePath { get; set; } = string.Empty;
        public string PhotoFolderPath { get; set; } = string.Empty;
    }

    public class CleanPhotoResponse
    {
        public string Message { get; set; } = string.Empty;
        public string RemovedFilesReportPath { get; set; } = string.Empty;
    }
}