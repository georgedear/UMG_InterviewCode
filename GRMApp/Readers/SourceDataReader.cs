using System.Globalization;
using System.Text.RegularExpressions;
using GRMApp.Models;

namespace GRMApp.Readers;

public static class SourceDataReader
{
    public static List<MusicContract> ReadMusicContracts(string filePath)
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
                    Usages:    parts[2].Split(',').Select(u => u.Trim()).ToList(),
                    StartDate: ParseDate(parts[3].Trim()),
                    EndDate:   string.IsNullOrWhiteSpace(parts[4]) ? null : ParseDate(parts[4].Trim())
                );
            })
            .ToList();
    }
    
    public static List<DistributionContract> ReadDistributionContracts(string filePath)
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

    private static DateOnly ParseDate(string date)
    {
        var cleaned = Regex.Replace(date, @"(\d+)(st|nd|rd|th)", "$1");
        return DateOnly.ParseExact(cleaned, ["d MMM yyyy", "d MMMM yyyy"], CultureInfo.InvariantCulture);
    }
}