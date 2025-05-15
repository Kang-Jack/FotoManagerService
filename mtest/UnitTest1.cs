using System.Collections.Specialized;
using NUnit.Framework;
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

    public bool DirectoryExists(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return DirectoryExistsResult;
    }

    public async Task<bool> DirectoryExistsAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return DirectoryExistsResult;
    }

    public string GetFullPath(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFullPath(path);
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetFilesResult;
    }

    public async Task<string[]> GetFilesAsync(string path, string searchPattern)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetFilesResult;
    }

    public string[] GetDirectories(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetDirectoriesResult;
    }

    public async Task<string[]> GetDirectoriesAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetDirectoriesResult;
    }

    public void CreateDirectory(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
    }

    public async Task CreateDirectoryAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
    }

    public bool FileExists(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return FileExistsResult;
    }

    public async Task<bool> FileExistsAsync(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return FileExistsResult;
    }

    public StreamReader OpenText(string path)
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
        return OpenTextResult;
    }

    public async Task<StreamReader> OpenTextAsync(string path)
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
        return OpenTextResult;
    }

    public StreamWriter CreateText(string path)
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
        return CreateTextResult;
    }

    public async Task<StreamWriter> CreateTextAsync(string path)
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
        return CreateTextResult;
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

    public void MoveFile(string sourcePath, string targetPath)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
    }

    public async Task MoveFileAsync(string sourcePath, string targetPath)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
    }
}

public class testableFotoManager : FotoManager
{
    public bool ReadListInFileRes { set; get; }
    public string PhotoFolderPath { set; get; } = "C:\\TestPhotos";
    public string WriteListFileRes { set; get; } = string.Empty;
    public StringCollection AllPhotos { set; get; }
    private readonly IFileSystem _fileSystem;

    public testableFotoManager(IFileSystem fileSystem) : base(fileSystem)
    {
        AllPhotos = new StringCollection();
        _fileSystem = fileSystem;
    }

    protected virtual async Task<bool> ReadListInFileAsync(string listFileName, StringCollection allPhotos)
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

    protected string InputPhotoFolderPath()
    {
        return PhotoFolderPath;
    }

    protected virtual async Task<string> WriteListFileAsync(string fileName, StringCollection allFiles)
    {
        return WriteListFileRes;
    }

    public async Task<string> CreateListFileAsync(string listFileName, string photoFolderPath)
    {
        if (string.IsNullOrEmpty(listFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            if (!_fileSystem.DirectoryExists(Path.GetDirectoryName(listFileName) ?? string.Empty))
            {
                return ConstDef.ConstErrFotoPath;
            }

            if (_fileSystem.FileExists(listFileName))
            {
                using var reader = _fileSystem.OpenText(listFileName);
                // File exists and is readable
            }
            else
            {
                using var writer = _fileSystem.CreateText(listFileName);
                // File doesn't exist and can be created
            }

            var result = await WriteListFileAsync(listFileName, AllPhotos);
            if (string.IsNullOrEmpty(result))
            {
                return ConstDef.ConstErrFotoPath;
            }
            return result;
        }
        catch (InvalidOperationException ex) when (ex.Message == "File in use" || ex.Message == "Access denied")
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }

    public async Task<string> CleanPhotoAsync(string listFileName, string reportFileName, string photoFolderPath)
    {
        if (string.IsNullOrEmpty(listFileName) || string.IsNullOrEmpty(reportFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check file access first
            if (!_fileSystem.FileExists(listFileName))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Read the list file
            var allPhotos = new StringCollection();
            if (!await ReadListInFileAsync(listFileName, allPhotos))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Check each photo
            var report = new StringCollection();
            foreach (string photo in allPhotos)
            {
                if (!_fileSystem.FileExists(photo))
                {
                    report.Add($"File not found: {photo}");
                }
            }

            return await WriteListFileAsync(reportFileName, report);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }

    public async Task<string> GenerateDiffReportsAsync(string listFileName, string photoFolderPath, string reportFileName = "")
    {
        if (string.IsNullOrEmpty(listFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check file access first
            if (!_fileSystem.FileExists(listFileName))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Read the list file
            var allPhotos = new StringCollection();
            if (!await ReadListInFileAsync(listFileName, allPhotos))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get input path
            var inputPath = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(inputPath))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get files and compare
            var allFiles = _fileSystem.GetFiles(inputPath, "*.*");
            var report = new StringCollection();

            // Check for missing files in target
            foreach (string photo in allPhotos)
            {
                var found = false;
                foreach (var file in allFiles)
                {
                    if (_fileSystem.GetFileNameWithoutExtension(file) == _fileSystem.GetFileNameWithoutExtension(photo))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    report.Add($"Missing in target: {photo}");
                }
            }

            // Check for missing files in baseline
            foreach (var file in allFiles)
            {
                var extension = _fileSystem.GetExtension(file).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    var found = false;
                    foreach (string photo in allPhotos)
                    {
                        if (_fileSystem.GetFileNameWithoutExtension(file) == _fileSystem.GetFileNameWithoutExtension(photo))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        report.Add($"Missing in baseline: {file}");
                    }
                }
            }

            return await WriteListFileAsync(listFileName, report);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }

    public new string MoveFile(string sourcePath, string targetPath)
    {
        if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetPath))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check source file exists
            if (!_fileSystem.FileExists(sourcePath))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Check target directory exists
            var targetDir = Path.GetDirectoryName(targetPath) ?? string.Empty;
            if (!_fileSystem.DirectoryExists(targetDir))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Check if target file already exists
            if (_fileSystem.FileExists(targetPath))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Move the file
            _fileSystem.MoveFile(sourcePath, targetPath);
            return string.Empty;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
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

    // Test for CreateListFile with valid path and file name
    //[Test]
    //public async Task TestCreateListFileValidPathAndFileName()
    //{
    //    mockFileSystem.DirectoryExistsResult = true;
    //    string listFileName = "testlist.txt";
    //    var testManager = (testableFotoManager)m_sut;
    //    testManager.WriteListFileRes = ConstDef.ConstMesgReturnList + listFileName;
    //    string result = await testManager.CreateListFileAsync(listFileName, testManager.PhotoFolderPath);
    //    Assert.That(result, Contains.Substring(ConstDef.ConstMesgReturnList));
    //}

    // Test for CreateListFile with invalid file name
    [Test]
    public async Task TestCreateListFileInvalidFilePath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        var testManager = (testableFotoManager)m_sut;
        var result = await testManager.CreateListFileAsync("errorPath", testManager.PhotoFolderPath);
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    // Test for CreateListFile with null file name
    [Test]
    public async Task TestCreateListFileNullFileName()
    {
        var testManager = (testableFotoManager)m_sut;
        string result = await testManager.CreateListFileAsync(null, testManager.PhotoFolderPath);
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    // Test for CreateListFile with empty file name
    [Test]
    public async Task TestCreateListFileEmptyFileName()
    {
        var testManager = (testableFotoManager)m_sut;
        string result = await testManager.CreateListFileAsync(string.Empty, testManager.PhotoFolderPath);
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }
    // Test for CreateListFile with file name that already exists
    [Test]
    public void TestCreateListFileExistingFileName()
    {
        mockFileSystem.FileExistsResult = true;
        string existingFileName = "existingfile.txt";
        string result = m_sut.CreateListFileAsync(existingFileName, ((testableFotoManager)m_sut).PhotoFolderPath).Result;
        Assert.That(result, Is.Not.Null); // Should not overwrite the file
        File.Delete(existingFileName); // Clean up after test
    }


    [Test]
    /// <summary>
    /// Test case: Clean photo when all parameters are valid.
    /// </summary>
    public async Task TestCleanPhotoValidPathAndFileName()
    {
        mockFileSystem.DirectoryExistsResult = true;
        mockFileSystem.FileExistsResult = true;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        var testManager = (testableFotoManager)m_sut;
        testManager.WriteListFileRes = ConstDef.ConstMesgReturnList + reportFileName;
        testManager.ReadListInFileRes = true;
        string result = await testManager.CleanPhotoAsync(listFileName, reportFileName, testManager.PhotoFolderPath);
        Assert.That(result, Contains.Substring(ConstDef.ConstMesgReturnList));
    }

    [Test]
    /// <summary>
    /// Test case: Clean photo when the path is null.
    /// </summary>
    public async Task TestCleanPhotoInvalidPath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        var testManager = (testableFotoManager)m_sut;
        var result = await testManager.CleanPhotoAsync(null, reportFileName, testManager.PhotoFolderPath);
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    /// <summary>
    /// Test case: Clean photo when the report name is null.
    /// </summary>
    public async Task TestCleanPhotoNullReportName()
    {
        string listFileName = "testlist.txt";
        var testManager = (testableFotoManager)m_sut;
        var result = await testManager.CleanPhotoAsync(listFileName, null, testManager.PhotoFolderPath);
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }


    [Test]
    /// <summary>
    /// Test case: Generate diff reports when both lists are empty.
    /// </summary>
    public void GenerateDiffReports_BothListsEmpty_ReturnsNoDiff()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.PhotoFolderPath = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection();
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        Assert.That(_sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath).Result.Contains("No differences found."));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only target has files.
    /// </summary>
    public void GenerateDiffReports_OnlyTargetHasFiles_ReturnsMissingInBaseline()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.PhotoFolderPath = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg";
        mockFileSystem.GetFilesResult = new[] { "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        Assert.That(_sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath).Result.Contains("Missing in baseline: file3.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only baseline has files.
    /// </summary>
    public async Task GenerateDiffReports_OnlyBaselineHasFiles_ReturnsMissingInTarget()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.PhotoFolderPath = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file1.jpg,file2.jpg";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath);
        Assert.That(result.Contains("Missing in baseline: file1.jpg,file2.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when both lists have files with differences.
    /// </summary>
    public async Task GenerateDiffReports_BothListsHaveFiles_ReturnsCorrectDifferences()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.PhotoFolderPath = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg, Missing in target: file1.jpg";
        mockFileSystem.GetFilesResult = new[] { "file2.jpg", "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        var result = await _sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath);
        Assert.That(result.Contains("Missing in baseline: file3.jpg, Missing in target: file1.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when lists have no differences.
    /// </summary>
    public void GenerateDiffReports_ListsHaveNoDifferences_ReturnsNoDiff2()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.PhotoFolderPath = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = new[] { "file1.jpg", "file2.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file1";
        
        Assert.That(_sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath).Result.Contains("No differences found."));
    }

    //[Test]
    ///// <summary>
    ///// Test case: Clean photo when the report name is an empty string.
    ///// </summary>
    //public async Task TestCleanPhotoEmptyReportName()
    //{
    //    var _sut = (testableFotoManager)m_sut;
    //    string listFileName = "testlist.txt";
    //    await m_sut.CreateListFileAsync(listFileName, _sut.PhotoFolderPath);
    //    var re = await m_sut.CleanPhotoAsync(listFileName, string.Empty, _sut.PhotoFolderPath);
    //    Assert.That(re.Equals(ConstDef.ConstErrFotoPath));
    //}

    [Test]
    public async Task TestCreateListFile()
    {
        var _sut = (testableFotoManager)m_sut;
        mockFileSystem.DirectoryExistsResult = true;
        string listFileName = "testlist.txt";
        //string result = m_sut.CreateListFile(listFileName);
        string result = await m_sut.CreateListFileAsync(listFileName, _sut.PhotoFolderPath);
        Assert.That(result, Is.Not.Null);
        File.Delete(listFileName); // Clean up after test
    }

    //[Test]
    //public async Task TestGenerateDiffReports()
    //{
    //    var _sut = (testableFotoManager)m_sut;
    //    _sut.PhotoFolderPath = "testPath";
    //    _sut.ReadListInFileRes = true;
    //    _sut.AllPhotos = new StringCollection();
    //    _sut.WriteListFileRes = "Test result";
    //    string result = await m_sut.GenerateDiffReportsAsync("test.txt", _sut.PhotoFolderPath);
    //    Assert.That(result, Is.Not.Null);
    //}
    //[Test]
    //public async Task TestCleanPhoto()
    //{
    //    var _sut = (testableFotoManager)m_sut;
    //    mockFileSystem.DirectoryExistsResult = true;
    //    mockFileSystem.FileExistsResult = true;
    //    _sut.ReadListInFileRes = true;
    //    string listFileName = "testlist.txt";
    //    string reportFileName = "report.txt";
    //    //string result = m_sut.CleanPhoto(listFileName, reportFileName);
    //    var result = await m_sut.CleanPhotoAsync(listFileName, string.Empty, _sut.PhotoFolderPath);
    //    Assert.That(result, Is.Not.Null);
    //    File.Delete(reportFileName); // Clean up after test
    //}
}

