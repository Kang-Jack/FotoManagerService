namespace foto_list.Interfaces
{
    public interface IFotoManger
    {
        Task<string> CreateListFileAsync(string listFileName, string photoFolderPath);
        Task<string> GenerateDiffReportsAsync(string listFileName, string photoFolderPath, string reportFileName = "");
        Task<string> CleanPhotoAsync(string listFileName, string reportFileName, string photoFolderPath);
    }
}