using System;
using System.IO;
using GRMApp.Readers;

class Program
{
    static void Main()
    {
        var data = SourceDataReader.ReadMusicLicenses(@"TextFiles\MusicContracts.txt");
        
        Console.WriteLine("Hello world");
    }
}