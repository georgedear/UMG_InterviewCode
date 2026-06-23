namespace GRMApp.Models;

public record MusicContract(
    string Artist,
    string Title,
    List<string> Usages,
    DateOnly StartDate,
    DateOnly? EndDate
);