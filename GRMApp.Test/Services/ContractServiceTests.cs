using GRMApp.Models;
using GRMApp.Readers;
using GRMApp.Services;
using NSubstitute;
using Xunit;
using Assert = Xunit.Assert;

namespace GRMApp.Test.Services;

public class ContractServiceTests
{
    private readonly ISourceDataReader _mockReader;
    private readonly ContractService _mockContractService;

    private static readonly List<DistributionContract> DefaultDistributionContracts =
    [
        new("ITunes", "digital download"),
        new("YouTube", "streaming")
    ];

    private static readonly MusicContract Record1 = new(
        "Tinie Tempah",
        "Frisky (Live from SoHo)",
        ["digital download", "streaming"],
        new DateOnly(2012, 2, 1),
        null);

    private static readonly MusicContract Record2 = new(
        "Tinie Tempah",
        "Miami 2 Ibiza",
        ["digital download"],
        new DateOnly(2012, 2, 1),
        null);

    private static readonly MusicContract Record3 = new(
        "Tinie Tempah",
        "Till I'm Gone",
        ["digital download"],
        new DateOnly(2012, 8, 1),
        null);

    private static readonly MusicContract Record4 = new(
        "Monkey Claw",
        "Black Mountain",
        ["digital download"],
        new DateOnly(2012, 2, 1),
        null);

    private static readonly MusicContract Record5 = new(
        "Monkey Claw",
        "Iron Horse",
        ["digital download", "streaming"],
        new DateOnly(2012, 6, 1),
        null);

    private static readonly MusicContract Record6 = new(
        "Monkey Claw",
        "Motor Mouth",
        ["digital download", "streaming"],
        new DateOnly(2011, 3, 1),
        null);

    private static readonly MusicContract Record7 = new(
        "Monkey Claw",
        "Christmas Special",
        ["streaming"],
        new DateOnly(2012, 12, 25),
        new DateOnly(2012, 12, 31));

    private static readonly List<MusicContract> DefaultMusicContracts =
    [
        Record1, Record2, Record3, Record4, Record5, Record6, Record7
    ];

    public ContractServiceTests()
    {
        _mockReader = Substitute.For<ISourceDataReader>();
        _mockContractService = new ContractService(_mockReader);

        _mockReader.ReadDistributionContracts(Arg.Any<string>()).Returns(DefaultDistributionContracts);
        _mockReader.ReadMusicContracts(Arg.Any<string>()).Returns(DefaultMusicContracts);
    }

    // Tests provided from instructions
    // Test Scenario 1
    [Fact]
    public void GetContracts_WhenTestScenario1_MustReturnExpectedOutput()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 3, 1));

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st March 2012"]).ToList();
        
        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal([Record4, Record6, Record1, Record2], result);
    }
    
    // Test Scenario 2
    [Fact]
    public void GetContracts_WhenTestScenario2_MustReturnExpectedOutput()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 4, 1));

        // Act
        var result = _mockContractService.GetContracts(["YouTube", "1st April 2012"]).ToList();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal([Record6, Record1], result);
    }
    
    // Test Scenario 3
    [Fact]
    public void GetContracts_WhenTestScenario3_MustReturnExpectedOutput()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 12, 27));

        // Act
        var result = _mockContractService.GetContracts(["YouTube", "27th Dec 2012"]).ToList();
        
        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal([Record7, Record5, Record6, Record1], result);
    }

    // --- Valid input / filtering ---

    [Fact]
    public void GetContracts_WhenValidInput_MustReturnContractsMatchingPartnerUsage()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 3, 1));

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st March 2012"]).ToList();

        // Assert
        Assert.All(result, c => Assert.Contains("digital download", c.Usages));
    }

    [Fact]
    public void GetContracts_WhenValidInput_MustExcludeContractsWhereEffectiveDateIsBeforeStartDate()
    {
        // Arrange
        var effectiveDate = new DateOnly(2012, 3, 1);
        _mockReader.ParseDate(Arg.Any<string>()).Returns(effectiveDate);

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st March 2012"]).ToList();

        // Assert
        Assert.All(result, c => Assert.True(c.StartDate <= effectiveDate));
    }

    [Fact]
    public void GetContracts_WhenValidInput_MustExcludeContractsWhereEffectiveDateIsAfterEndDate()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2016, 1, 1));

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st January 2016"]).ToList();

        // Assert
        // Christmas Special ended 2012 so should not appear for a 2016 effective date
        Assert.DoesNotContain(result, c => c.Title == "Christmas Special");
    }

    [Fact]
    public void GetContracts_WhenValidInput_MustIncludeContractsWithNullEndDate()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2099, 1, 1));

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st January 2099"]).ToList();

        // Assert
        Assert.Contains(result, c => c.Title == "Miami 2 Ibiza");
    }

    // --- Ordering ---

    [Fact]
    public void GetContracts_WhenValidInput_MustReturnResultsOrderedByArtistThenTitle()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 3, 1));
        _mockReader.ReadMusicContracts(Arg.Any<string>()).Returns([
            new("Artist B", "Title Z", ["digital download"], new DateOnly(2010, 1, 1), null),
            new("Artist A", "Title B", ["digital download"], new DateOnly(2010, 1, 1), null),
            new("Artist A", "Title A", ["digital download"], new DateOnly(2010, 1, 1), null),
        ]);

        // Act
        var result = _mockContractService.GetContracts(["ITunes", "1st March 2012"]).ToList();

        // Assert
        Assert.Equal("Artist A", result[0].Artist);
        Assert.Equal("Title A", result[0].Title);
        Assert.Equal("Artist A", result[1].Artist);
        Assert.Equal("Title B", result[1].Title);
        Assert.Equal("Artist B", result[2].Artist);
    }

    // --- Invalid input ---

    [Fact]
    public void GetContracts_WhenEmptyArgs_MustReturnEmptyList()
    {
        // Act
        var result = _mockContractService.GetContracts([]);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetContracts_WhenInvalidDateFormat_MustReturnEmptyList()
    {
        // Act
        var result = _mockContractService.GetContracts(["ITunes", "01/03/2012"]);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetContracts_WhenMissingDateInArgument_MustReturnEmptyList()
    {
        // Act
        var result = _mockContractService.GetContracts(["ITunes"]);

        // Assert
        Assert.Empty(result);
    }

    // --- Partner matching ---

    [Fact]
    public void GetContracts_WhenPartnerNameIsCaseInsensitive_MustReturnResults()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 3, 1));

        // Act
        var result = _mockContractService.GetContracts(["itunes", "1st March 2012"]).ToList();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetContracts_WhenUnknownPartner_MustThrowInvalidOperationException()
    {
        // Arrange
        _mockReader.ParseDate(Arg.Any<string>()).Returns(new DateOnly(2012, 3, 1));

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() =>
            _mockContractService.GetContracts(["UnknownPartner", "1st March 2012"]).ToList());
    }
}