using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using foto_list.Controllers;
using foto_list.Interfaces;
using foto_manager.Utils;

namespace foto_list.Tests
{
    [TestFixture]
    public class UnitTestPhotoManagerController
    {
        private Mock<IFotoManger> _mockPhotoManager;
        private PhotoManagerController _controller;

        [SetUp]
        public void Setup()
        {
            _mockPhotoManager = new Mock<IFotoManger>();
            _controller = new PhotoManagerController(_mockPhotoManager.Object);
        }

        [Test]
        public async Task TestCreateList_ValidPath_ReturnsFile()
        {
            // Arrange
            var testPath = Path.Combine("TestData", "Photos");
            _mockPhotoManager.Setup(m => m.CreateListFileAsync(It.IsAny<StreamWriter>(), testPath))
                .Callback<StreamWriter, string>((writer, path) => writer.Write("Test content"))
                .ReturnsAsync("Success");

            // Act
            var result = await _controller.CreatePhotoList(testPath) as FileResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileDownloadName, Is.EqualTo(ConstDef.ConstlistFileName));
        }

        [Test]
        public async Task TestCreateList_InvalidPath_ReturnsBadRequest()
        {
            // Arrange
            var invalidPath = "<invalid>";
            // We don't need to setup the mock because validation happens before the service is called

            // Act
            var result = await _controller.CreatePhotoList(invalidPath) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value.ToString(), Is.EqualTo(ConstDef.ConstInvalidFotoPath));
            
            // Verify the service was never called with invalid path
            _mockPhotoManager.Verify(m => m.CreateListFileAsync(It.IsAny<StreamWriter>(), invalidPath), Times.Never);
        }

        [Test]
        public async Task TestGenerateDiffReport_ValidPaths_ReturnsFile()
        {
            // Arrange
            var listFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "testlist.txt");
            var testPath = Path.Combine("TestData", "Photos");
            _mockPhotoManager.Setup(m => m.GenerateDiffReportsAsync(listFile, testPath, It.IsAny<StreamWriter>(), It.IsAny<StreamWriter>()))
                .Callback<string, string, StreamWriter, StreamWriter>((listPath, photoPath, baselineWriter, targetWriter) => {
                    baselineWriter.Write("Baseline test content");
                    targetWriter.Write("Target test content");
                })
                .ReturnsAsync("Success");

            // Act
            var result = await _controller.GenerateDiffReport(listFile, testPath, "baseline") as FileResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileDownloadName, Is.EqualTo(ConstDef.ConstBaselineDiffFileName));
        }
        
        [Test]
        public async Task TestGenerateDiffReport_InvalidListPath_ReturnsBadRequest()
        {
            // Arrange
            var invalidListFile = Path.Combine("<invalid>", "list.txt");
            var validPhotoPath = Path.Combine("TestData", "Photos");

            // Act
            var result = await _controller.GenerateDiffReport(invalidListFile, validPhotoPath, "baseline") as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value.ToString(), Is.EqualTo("Invalid list file path"));
            
            // Verify the service was never called with invalid path
            _mockPhotoManager.Verify(m => m.GenerateDiffReportsAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<StreamWriter>(), 
                It.IsAny<StreamWriter>()), 
                Times.Never);
        }
        
        [Test]
        public async Task TestGenerateDiffReport_InvalidPhotoPath_ReturnsBadRequest()
        {
            // Arrange
            var validListFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "testlist.txt");
            var invalidPhotoPath = Path.Combine("<invalid>", "Photos");

            // Act
            var result = await _controller.GenerateDiffReport(validListFile, invalidPhotoPath, "baseline") as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value.ToString(), Is.EqualTo(ConstDef.ConstInvalidFotoPath));
            
            // Verify the service was never called with invalid path
            _mockPhotoManager.Verify(m => m.GenerateDiffReportsAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<StreamWriter>(), 
                It.IsAny<StreamWriter>()), 
                Times.Never);
        }

        [Test]
        public async Task TestCleanPhotos_InvalidListFile_ReturnsBadRequest()
        {
            // Arrange
            var invalidListFile = Path.Combine("<invalid>", "list.txt");
            var testPath = Path.Combine("TestData", "Photos");

            // Act
            var result = await _controller.CleanPhotos(invalidListFile, testPath) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(actual: result?.Value?.ToString(), Is.EqualTo(ConstDef.ConstInvalidListPath));
            
            // Verify the service was never called with invalid path
            _mockPhotoManager.Verify(m => m.CleanPhotoAsync(
                It.IsAny<string>(), 
                It.IsAny<StreamWriter>(), 
                It.IsAny<string>()), 
                Times.Never);
        }
        
        [Test]
        public async Task TestCleanPhotos_InvalidPhotoPath_ReturnsBadRequest()
        {
            // Arrange
            var validListFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "valid.txt");
            var invalidPhotoPath = Path.Combine("<invalid>", "Photos");

            // Act
            var result = await _controller.CleanPhotos(validListFile, invalidPhotoPath) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value.ToString(), Is.EqualTo(ConstDef.ConstInvalidFotoPath));
            
            // Verify the service was never called with invalid path
            _mockPhotoManager.Verify(m => m.CleanPhotoAsync(
                It.IsAny<string>(), 
                It.IsAny<StreamWriter>(), 
                It.IsAny<string>()), 
                Times.Never);
        }
        
        [Test]
        public async Task TestCleanPhotos_ValidPaths_ReturnsFile()
        {
            // Arrange
            var validListFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "valid.txt");
            var validPhotoPath = Path.Combine("TestData", "Photos");
            _mockPhotoManager.Setup(m => m.CleanPhotoAsync(validListFile, It.IsAny<StreamWriter>(), validPhotoPath))
                .Callback<string, StreamWriter, string>((listPath, writer, photoPath) => {
                    writer.Write("Removed files content");
                })
                .ReturnsAsync("Success");

            // Act
            var result = await _controller.CleanPhotos(validListFile, validPhotoPath) as FileResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileDownloadName, Is.EqualTo(ConstDef.ConstRemovedFileName));
        }

        [Test]
        public void TestNormalizePath_InvalidChars_Removed()
        {
            // Arrange
            var pathWithInvalid = Path.Combine("Test", "<Photos>");

            // Act
            var methodInfo = typeof(PhotoManagerController).GetMethod("NormalizePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = methodInfo.Invoke(_controller, new object[] { pathWithInvalid }) as string;

            // Assert
            Assert.That((result.Contains("<") || result.Contains(">")),Is.False);
        }
        
        [Test]
        public void TestIsValidPath_WithInvalidChars_ReturnsFalse()
        {
            // Arrange
            var pathWithInvalid = Path.Combine("Test", "<Photos>");

            // Act
            var methodInfo = typeof(PhotoManagerController).GetMethod("IsValidPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)methodInfo.Invoke(_controller, new object[] { pathWithInvalid });

            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void TestIsValidPath_WithValidPath_ReturnsTrue()
        {
            // Arrange
            var validPath = Path.Combine("Test", "Photos");

            // Act
            var methodInfo = typeof(PhotoManagerController).GetMethod("IsValidPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (bool)methodInfo.Invoke(_controller, new object[] { validPath });

            // Assert
            Assert.That(result, Is.True);
        }
    }
}