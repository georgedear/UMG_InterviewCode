using System;
using System.IO;
using GRMApp.Readers;

class Program
{
    static void Main()
    {
        var musicContracts = SourceDataReader
            .ReadMusicContracts(@"TextFiles\MusicContracts.txt");
        
        var distributionContracts = SourceDataReader
            .ReadDistributionContracts(@"TextFiles\DistributionContracts.txt");
        
        Console.WriteLine("Hello world");
    }
}