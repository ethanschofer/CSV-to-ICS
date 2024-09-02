using FluentAssertions;
using Xunit;

namespace CsvToIcs.Tests;

public class ProgramTests
{
    [Fact]
    public void HelpCheck_NoArguments_ReturnsFalse()
    {
        // Arrange
        var args = new string[] { };

        // Act
        var result = Program.HelpCheck(args);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HelpCheck_FirstArgumentIsNotHelp_ReturnsFalse()
    {
        // Arrange
        var args = new string[] { "something" };

        // Act
        var result = Program.HelpCheck(args);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HelpCheck_FirstArgumentIs_ReturnsTrue()
    {
        // Arrange
        var args = new string[] { "-h" };

        // Act
        var result = Program.HelpCheck(args);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HelpCheck_FirstArgumentIsHelp_ReturnsTrue()
    {
        // Arrange
        var args = new string[] { "--help" };

        // Act
        var result = Program.HelpCheck(args);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HelpCheck_FirstArgumentIsHelpWithOtherArguments_ReturnsTrue()
    {
        // Arrange
        var args = new string[] { "--help", "something" };

        // Act
        var result = Program.HelpCheck(args);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ProcessArguments_ThereAreNoArguments_ArgumentsHaveDefaults()
    {
        // Arrange
        var args = new string[] { };

        // Act
        var result = Program.ProcessArguments(args);

        // Assert
        result.CsvFilePath.Should().Be("c:\\CSVtoICS\\CSV\\events.csv");
        result.IcsDirectoryPath.Should().Be("c:\\CSVtoICS\\ICS");
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ProcessArguments_ArgsAreNull_ArgumentsHaveDefaults()
    {
        // Arrange
        string[]? args = null;

        // Act
        var result = Program.ProcessArguments(args);

        // Assert
        result.CsvFilePath.Should().Be("c:\\CSVtoICS\\CSV\\events.csv");
        result.IcsDirectoryPath.Should().Be("c:\\CSVtoICS\\ICS");
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ProcessArguments_ThereAreArguments_ArgumentsHaveValues()
    {
        // Arrange
        var args = new string[] { "path1", "path2" };

        // Act
        var result = Program.ProcessArguments(args);

        // Assert
        result.CsvFilePath.Should().Be("path1");
        result.IcsDirectoryPath.Should().Be("path2");
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ProcessArguments_ThereIsOneArgument_OtherArgHasDefault()
    {
        // Arrange
        var args = new string[] { "path1" };

        // Act
        var result = Program.ProcessArguments(args);

        // Assert
        result.CsvFilePath.Should().Be("path1");
        result.IcsDirectoryPath.Should().Be("c:\\CSVtoICS\\ICS");
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ProcessArguments_FirstArgumentsIsEmpty_OtherArgHasDefault()
    {
        // Arrange
        var args = new string[] { string.Empty,"path2" };

        // Act
        var result = Program.ProcessArguments(args);

        // Assert
        result.CsvFilePath.Should().Be("c:\\CSVtoICS\\CSV\\events.csv");
        result.IcsDirectoryPath.Should().Be("path2");
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ValidateArguments_FileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var args = new Arguments
        {
            CsvFilePath = "path1"
        };

        // Act
        var result = Program.ValidateArguments(args);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().Be("The .CSV file path1 does not exist.");
    }

    [Fact]
    public void ValidateArguments_FileAndDirectoryExists_ReturnsTrue()
    {
        // Arrange
        var args = new Arguments
        {
            CsvFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}test.csv",
            IcsDirectoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}"
        };

        // Act
        var result = Program.ValidateArguments(args);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ValidationMessage.Should().Be("Arguments are valid.");
    }

    [Fact]
    public void ValidateArguments_FileDoesNotExists_ReturnsFalse()
    {
        // Arrange
        var args = new Arguments
        {
            CsvFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}alternate.csv",
            IcsDirectoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}"
        };

        // Act
        var result = Program.ValidateArguments(args);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().Be($"The .CSV file {AppDomain.CurrentDomain.BaseDirectory}alternate.csv does not exist.");
    }

    [Fact]
    public void ValidateArguments_DirectoryDoesNotExists_ReturnsFalse()
    {
        // Arrange
        var args = new Arguments
        {
            CsvFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}test.csv",
            IcsDirectoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}alternate"
        };

        // Act
        var result = Program.ValidateArguments(args);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationMessage.Should().Be($"The directory {AppDomain.CurrentDomain.BaseDirectory}alternate, the location where .ICS files are to be saved, does not exist.");
    }

    [Fact]
    public void ClearDirectory_DirectoryHasFiles_FilesAreRemoved()
    {
        var random = new Random();
        var randomNumber = random.Next(1, 1000);
        var folderName = $"testfolder-{randomNumber}";

        var newDirectory = Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{folderName}");

        var file1 = File.Create($"{newDirectory.FullName}\\file1.txt");
        file1.Close();

        newDirectory.GetFiles().Should().HaveCount(1);

        Program.ClearDirectory(newDirectory.FullName);

        newDirectory.GetFiles().Should().BeEmpty();

        newDirectory.Delete();
    }

    [Fact]
    public void ConvertCsvToIcs_CsvFileIsGood_IcsFileCreated()
    {
        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}test.csv";

        var random = new Random();
        var randomNumber = random.Next(1, 1000);
        var folderName = $"testfolder-{randomNumber}";

        var newDirectory = Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{folderName}");

        newDirectory.GetFiles().Should().HaveCount(0);

        Program.ConvertCsvToIcs(filePath,newDirectory.FullName);

        var files = newDirectory.GetFiles();
        files.Should().HaveCount(1);

        var file = files.FirstOrDefault();
        file.Should().NotBeNull();
        file.Extension.Should().Be(".ics");

        Program.ClearDirectory(newDirectory.FullName);

        newDirectory.GetFiles().Should().BeEmpty();

        newDirectory.Delete();

    }
}