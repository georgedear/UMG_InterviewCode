using System.Text.RegularExpressions;
using GRMApp.Models;
using GRMApp.Readers;

namespace GRMApp.Services;

public class ContractService(ISourceDataReader reader)
{
    private const string InputRegexPattern = @"^(\w+?)\s+(\d{1,2}(?:st|nd|rd|th)\s+\w+\s+\d{4})$";

    public IEnumerable<MusicContract> GetContracts(string[] args)
    {
        var input = args.Length > 0 ? string.Join(" ", args) : string.Empty;

        var regexMatch = Regex.Match(input, InputRegexPattern);
        if (string.IsNullOrEmpty(input) || !regexMatch.Success)
        {
            Console.WriteLine($"Incorrect format for '{input}'. Input should be of format: 'Partner Date'");
            return [];
        }

        var inputDeliveryPartner = regexMatch.Groups[1].Value;
        var inputEffectiveDate = reader.ParseDate(regexMatch.Groups[2].Value);

        var distributionContracts = reader
            .ReadDistributionContracts(@"TextFiles\DistributionContracts.txt");

        // Could handle this alternatively by exiting early with message
        var chosenUsage = distributionContracts
            .Single(x => x.Partner.ToLowerInvariant() == inputDeliveryPartner.ToLowerInvariant()).Usage;

        var musicContracts = reader.ReadMusicContracts(@"TextFiles\MusicContracts.txt");

        return musicContracts
            .Where(contract => contract.Usages.Contains(chosenUsage))
            .Where(contract => contract.StartDate <= inputEffectiveDate &&
                               (contract.EndDate is null || contract.EndDate >= inputEffectiveDate))
            .OrderBy(contract => contract.Artist) // Assumed sort by first two fields (based on provided test data)
            .ThenBy(contract => contract.Title);
    }
}