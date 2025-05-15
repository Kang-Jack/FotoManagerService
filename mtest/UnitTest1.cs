using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
using System;
using foto_list.Services;
using foto_list.Interfaces;
using foto_manager.Utils;
namespace mtest;

public class MockFileSystem : IFileSystem
{
    public bool DirectoryExistsResult { get; set; } = true;
    public bool FileExistsResult { get; set; } = true;
    public string[] GetFilesResult { get; set; } = Array.Empty<string>();
    public string[] GetDirectoriesResult { get; set; } = Array.Empty<string>();
    public string GetFullPathResult { get; set; } = "C:\\TestPath";
    public string CombineResult { get; set; } = "C:\\TestPath\\file.txt";
    public string GetFileNameWithoutExtensionResult { get; set; } = "test";
    public string GetFileNameResult { get; set; } = "test.txt";
    public string GetExtensionResult { get; set; } = ".txt";
    public StreamReader? OpenTextResult { get; set; }
    public StreamWriter? CreateTextResult { get; set; }
    public bool ThrowAccessDenied { get; set; }
    public bool ThrowFileInUse { get; set; }
    public bool OpenTextThrowException { get; set; }
    public bool CreateTextThrowException { get; set; }

    public Task<bool> DirectoryExistsAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.FromResult(DirectoryExistsResult);
    }

    public string GetFullPath(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFullPath(path);
    }

    public Task<string[]> GetFilesAsync(string path, string searchPattern)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.FromResult(GetFilesResult);
    }

    public Task<string[]> GetDirectoriesAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.FromResult(GetDirectoriesResult);
    }

    public Task CreateDirectoryAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.FromResult(FileExistsResult);
    }

    public Task<StreamReader> OpenTextAsync(string path)
    {
        if (OpenTextThrowException)
        {
            throw new IOException("Error reading file");
        }
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        if (ThrowFileInUse)
        {
            throw new InvalidOperationException("File in use");
        }
        if (OpenTextResult == null)
        {
            throw new FileNotFoundException("File not found", path);
        }
        return Task.FromResult(OpenTextResult);
    }

    public Task<StreamWriter> CreateTextAsync(string path)
    {
        if (CreateTextThrowException)
        {
            throw new IOException("Error creating file");
        }
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        if (ThrowFileInUse)
        {
            throw new InvalidOperationException("File in use");
        }
        if (CreateTextResult == null)
        {
            throw new InvalidOperationException("Cannot create file");
        }
        return Task.FromResult(CreateTextResult);
    }

    public string Combine(params string[] paths)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.Combine(paths);
    }

    public string GetFileNameWithoutExtension(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFileNameWithoutExtension(path);
    }

    public string GetFileName(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFileName(path);
    }

    public string GetExtension(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetExtension(path);
    }

    public Task MoveFileAsync(string sourcePath, string targetPath)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Task.CompletedTask;
    }
}

public class testableFotoManager : FotoManager
{
    public bool ReadListInFileRes { set; get; }
    public string WriteListFileRes { set; get; } = string.Empty;
    public StringCollection AllPhotos { set; get; }
    private readonly IFileSystem _fileSystem;

    public testableFotoManager(IFileSystem fileSystem) : base(fileSystem)
    {
        AllPhotos = new StringCollection();
        _fileSystem = fileSystem;
    }

    protected override async Task<bool> ReadListInFileAsync(string listFileName, StringCollection allPhotos)
    {
        if (AllPhotos != null)
        {
            foreach (var photo in AllPhotos)
            {
                allPhotos.Add(photo);
            }
        }
        return ReadListInFileRes;
    }

    protected override async Task<string> WriteListFileAsync(string fileName, StringCollection allFiles)
    {
        return WriteListFileRes;
    }
}

public class MTestFotoManger
{
    private IFotoManger m_sut;
    private MockFileSystem mockFileSystem;
    private string tempDirPath;

    [SetUp]
    public void Setup()
    {
        mockFileSystem = new MockFileSystem();
        m_sut = new testableFotoManager(mockFileSystem);
        // Setup: Create a temporary directory for testing
        tempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirPath);
    }

    [TearDown]
    public void TearDown()
    {
        m_sut = null;
        mockFileSystem = null;
        // Cleanup: Delete the temporary directory after each test
        if (Directory.Exists(tempDirPath))
            Directory.Delete(tempDirPath, true);
    }

    [Test]
    public async Task TestCreateListFileValidPathAndFileName()
    {
        mockFileSystem.DirectoryExistsResult = true;
        string listFileName = "testlist.txt";
        string photoFolderPath = "C:\\TestPhotos";
        string result = await m_sut.CreateListFileAsync(listFileName, photoFolderPath);
        Assert.IsNotNull(result);
        if (File.Exists(listFileName)) File.Delete(listFileName); // Clean up after test
    }

    [Test]
    public async Task TestCreateListFileInvalidFilePath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        var result = await m_sut.CreateListFileAsync("errorPath", "C:\\TestPhotos");
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public async Task TestCreateListFileNullFileName()
    {
        string result = await m_sut.CreateListFileAsync(null, "C:\\TestPhotos");
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public async Task TestCreateListFileEmptyFileName()
    {
        string result = await m_sut.CreateListFileAsync(string.Empty, "C:\\TestPhotos");
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public async Task TestCreateListFileExistingFileName()
    {
        mockFileSystem.FileExistsResult = true;
        string existingFileName = "existingfile.txt";
        string photoFolderPath = "C:\\TestPhotos";
        string result = await m_sut.CreateListFileAsync(existingFileName, photoFolderPath);
        Assert.IsNotNull(result);
        if (File.Exists(existingFileName)) File.Delete(existingFileName);
    }

    [Test]
    public async Task TestCleanPhotoValidPathAndFileName()
    {
        mockFileSystem.DirectoryExistsResult = true;
        mockFileSystem.FileExistsResult = true;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        string photoFolderPath = "C:\\TestPhotos";
        await m_sut.CreateListFileAsync(listFileName, photoFolderPath);
        string result = await m_sut.CleanPhotoAsync(listFileName, reportFileName, photoFolderPath);
        Assert.IsNotNull(result);
        if (File.Exists(reportFileName)) File.Delete(reportFileName);
    }

    [Test]
    public async Task TestCleanPhotoInvalidPath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        string photoFolderPath = "C:\\TestPhotos";
        await m_sut.CreateListFileAsync(listFileName, photoFolderPath);
        var result = await m_sut.CleanPhotoAsync(null, reportFileName, photoFolderPath);
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public async Task TestCleanPhotoNullReportName()
    {
        string listFileName = "testlist.txt";
        string photoFolderPath = "C:\\TestPhotos";
        await m_sut.CreateListFileAsync(listFileName, photoFolderPath);
        var result = await m_sut.CleanPhotoAsync(listFileName, null, photoFolderPath);
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public async Task GenerateDiffReports_BothListsEmpty_ReturnsNoDiff()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection();
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", "C:\\TestPhotos");
        Assert.That(result.Contains("No differences found."));
    }

    [Test]
    public async Task GenerateDiffReports_OnlyTargetHasFiles_ReturnsMissingInBaseline()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg";
        mockFileSystem.GetFilesResult = new[] { "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", "C:\\TestPhotos");
        Assert.That(result.Contains("Missing in baseline: file3.jpg"));
    }

    [Test]
    public async Task GenerateDiffReports_OnlyBaselineHasFiles_ReturnsMissingInTarget()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file1.jpg,file2.jpg";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", "C:\\TestPhotos");
        Assert.That(result.Contains("Missing in baseline: file1.jpg,file2.jpg"));
    }

    [Test]
    public async Task GenerateDiffReports_BothListsHaveFiles_ReturnsCorrectDifferences()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg, Missing in target: file1.jpg";
        mockFileSystem.GetFilesResult = new[] { "file2.jpg", "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", "C:\\TestPhotos");
        Assert.That(result.Contains("Missing in baseline: file3.jpg, Missing in target: file1.jpg"));
    }

    [Test]
    public async Task GenerateDiffReports_ListsHaveNoDifferences_ReturnsNoDiff()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = new[] { "file1.jpg", "file2.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file1";
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", "C:\\TestPhotos");
        Assert.That(result.Contains("No differences found."));
    }

    [Test]
    public async Task TestCleanPhotoEmptyReportName()
    {
        string listFileName = "testlist.txt";
        string photoFolderPath = "C:\\TestPhotos";
        await m_sut.CreateListFileAsync(listFileName, photoFolderPath);
        var result = await m_sut.CleanPhotoAsync(listFileName, string.Empty, photoFolderPath);
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }
}

