namespace ArchetypeConfigurator;

public record IncludeRule
{
    public IncludeRule(int id, IReadOnlyCollection<int> requiredParts)
    {
        Id = id;
        RequiredParts = requiredParts;
    }

    public int Id { get; init; }

    //only one of the required parts must be present
    public IReadOnlyCollection<int> RequiredParts { get; init; }
}