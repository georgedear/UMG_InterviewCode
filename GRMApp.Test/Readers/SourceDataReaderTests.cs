using GRMApp.Readers;
using Xunit;
using Assert = Xunit.Assert;

namespace GRMApp.Test.Readers;

public class SourceDataReaderTests
{
    private readonly ISourceDataReader _reader = new SourceDataReader();

    // TODO: Can test ReadMusicContracts correctly manipulates
    
    // TODO: Can test ReadDistributionContracts correctly manipulates

    [Fact]
    public void ParseDate_WhenValidDateString_MustReturnDate()
    {
        // Act
        var result = _reader.ParseDate("3rd March 2012");
        
        // Assert
        
        Assert.Equal(new DateOnly(2012, 3, 3), result);
    }
    
    [Fact]
    public void ParseDate_WhenMonthIsAbbreviated_MustReturnDate()
    {
        // Act
        var result = _reader.ParseDate("3rd Mar 2012");
        
        // Assert
        Assert.Equal(new DateOnly(2012, 3, 3), result);
    }
    
    [Fact]
    public void ParseDate_WhenInvalidFormat_MustThrowFormatException()
    {
        // Assert
        Assert.Throws<FormatException>(() => _reader.ParseDate("01/03/2012"));
    }

    [Fact]
    public void ParseDate_WithInvalidMonth_ThrowsFormatException()
    {
        // Assert
        Assert.Throws<FormatException>(() => _reader.ParseDate("1st Marching 2012"));
    }

    [Fact]
    public void ParseDate_WithEmptyString_ThrowsFormatException()
    {
        // Assert
        Assert.Throws<FormatException>(() => _reader.ParseDate(string.Empty));
    }
}