using System.Globalization;
using System.Text.RegularExpressions;
using GRMApp.Models;

namespace GRMApp.Readers;

public interface ISourceDataReader
{
    List<MusicContract> ReadMusicContracts(string filePath);
    List<DistributionContract> ReadDistributionContracts(string filePath);
    DateOnly ParseDate(string date);
}

public class SourceDataReader: ISourceDataReader
{
    public List<MusicContract> ReadMusicContracts(string filePath)
    {
        return File.ReadAllLines(filePath)
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split('|');

                return new MusicContract(
                    Artist:    parts[0].Trim(),
                    Title:     parts[1].Trim(),
                    Usages:    parts[2].Split(',').Select(x => x.Trim()).ToList(),
                    StartDate: ParseDate(parts[3].Trim()),
                    EndDate:   string.IsNullOrWhiteSpace(parts[4]) ? null : ParseDate(parts[4].Trim())
                );
            })
            .ToList();
    }
    
    public List<DistributionContract> ReadDistributionContracts(string filePath)
    {
        return File.ReadAllLines(filePath)
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split('|');
                
                return new DistributionContract(
                    Partner: parts[0].Trim(),
                    Usage:   parts[1].Trim()
                );
            })
            .ToList();
    }

    public DateOnly ParseDate(string date)
    {
        var cleaned = Regex.Replace(date, @"(\d+)(st|nd|rd|th)", "$1");
        return DateOnly.ParseExact(cleaned, ["d MMM yyyy", "d MMMM yyyy"], CultureInfo.InvariantCulture);
    }
}