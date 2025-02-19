using Moq;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Helpers;

namespace SmartAccountant.Import.Service.Tests.Helpers.FileTypeValidatorTests;

[TestClass]
public class IsValidFile
{
    private FileTypeValidator sut = null!;

    [TestInitialize]
    public void Initialize() => sut = new FileTypeValidator();

    [TestMethod]
    public async Task ReturnsTrueForValidXlsFile()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xls",
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ReturnsFalseForInvalidXlsFile()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xls",
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0x00, 0x00, 0x00, 0x00])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ReturnsTrueForValidXlsxFile()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            OpenReadStream = () => new MemoryStream([0x50, 0x4B, 0x03, 0x04])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ReturnsFalseForInvalidXlsxFile()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            OpenReadStream = () => new MemoryStream([0x00, 0x00, 0x00, 0x00])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ReturnsFalseForUnknownFileExtension()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.unknown",
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("test.xls")]
    [DataRow("test.XLS")]
    public async Task IsCaseInsensitiveForValidFile(string fileName)
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = fileName,
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("test.xls")]
    [DataRow("test.XLS")]
    [DataRow("test.unknown")]
    [DataRow("test.UNKNOWN")]
    public async Task IsCaseInsensitiveForInvalidFile(string fileName)
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = fileName,
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0x00, 0x00, 0x00, 0x00])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(".xls")]
    [DataRow(".unknown")]
    public async Task ReturnsFalseForEmptyFileName(string fileName)
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = fileName,
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("test")]
    [DataRow(".")]
    [DataRow("test.")]
    [DataRow("test..")]
    public async Task ReturnsFalseForEmptyFileExtension(string fileName)
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = fileName,
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ReturnsFalseForInvalidContentType()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xls",
            ContentType = "application/unknown",
            OpenReadStream = () => new MemoryStream([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ReturnsFalseForEmptyContent()
    {
        // Arrange
        var file = new ImportFile
        {
            FileName = "test.xls",
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => new MemoryStream([])
        };

        // Act
        bool result = await sut.IsValidFile(file, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ReturnsCanceledTaskWhenCancellationRequested()
    {
        // Arrange
        using CancellationTokenSource cancellationTokenSource = new(0);

        var mockStream = new Mock<Stream>();
        mockStream.Setup((x) => x.ReadAsync(It.IsAny<Memory<byte>>(), Match.Create((CancellationToken token) => token.IsCancellationRequested)))
            .Throws<OperationCanceledException>();

        var file = new ImportFile
        {
            FileName = "test.xls",
            ContentType = "application/vnd.ms-excel",
            OpenReadStream = () => mockStream.Object,
        };

        // Act
        Task result = sut.IsValidFile(file, cancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(TaskStatus.Canceled, result.Status);
    }
}
