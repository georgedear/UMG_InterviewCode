using System.Text.RegularExpressions;
using GRMApp.Models;
using GRMApp.Readers;

class Program
{
    private static string _inputRegexPattern = @"^(\w+?)\s+(\d{1,2}(?:st|nd|rd|th)\s+\w+\s+\d{4})$";
    
    static List<MusicContract> Main(string[] args)
    {
        var input = args.Length > 0 ? string.Join(" ", args) : string.Empty;
        
        var regexMatch = Regex.Match(input, _inputRegexPattern);
        if (string.IsNullOrEmpty(input) || !regexMatch.Success)
        {
            Console.WriteLine($"Incorrect format for '{input}'. Input should be of format: 'Partner Date'");
            return [];
        }
        
        var inputDeliveryPartner = regexMatch.Groups[1].Value;
        var inputEffectiveDate = SourceDataReader.ParseDate(regexMatch.Groups[2].Value);
        
        var distributionContracts = SourceDataReader
            .ReadDistributionContracts(@"TextFiles\DistributionContracts.txt");
        
        // Assumed here that only a single entry will exist for a given partner. Could exit early with blank array alternatively
        var chosenUsage = distributionContracts
            .Single(x => x.Partner.ToLowerInvariant() == inputDeliveryPartner.ToLowerInvariant()).Usage;
        
        var musicContracts = SourceDataReader
            .ReadMusicContracts(@"TextFiles\MusicContracts.txt");
        
        return musicContracts
            .Where(contract => contract.Usages.Contains(chosenUsage))
            .Where(contract => contract.StartDate <= inputEffectiveDate && 
                               (contract.EndDate is null || contract.EndDate >= inputEffectiveDate))
            .ToList();
    }
}