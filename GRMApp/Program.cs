using GRMApp.Readers;
using GRMApp.Services;
using Microsoft.Extensions.DependencyInjection;
class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddSingleton<ISourceDataReader, SourceDataReader>()
            .AddSingleton<ContractService>()
            .BuildServiceProvider();

        var contractService = services.GetRequiredService<ContractService>();
        
        var filteredContracts = contractService
            .GetContracts(args)
            .ToList();

        // Print header
        Console.WriteLine("Artist|Title|Usages|StartDate|EndDate");
        foreach (var contract in filteredContracts)
        {
            var usages = string.Join(", ", contract.Usages);
            var startDate = contract.StartDate.ToString("d MMM yyyy");
            var endDate = contract.EndDate?.ToString("d MMM yyyy") ?? "";
    
            Console.WriteLine($"{contract.Artist}|{contract.Title}|{usages}|{startDate}|{endDate}");
        }
    }
}