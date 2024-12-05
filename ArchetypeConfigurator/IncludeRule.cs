namespace ArchetypeConfigurator;

public record IncludeRule
{
    public IncludeRule(int id, IReadOnlyCollection<int> requiredVariables)
    {
        Id = id;
        RequiredVariables = requiredVariables;
    }

    public int Id { get; init; }

    //only one of the required parts must be present
    public IReadOnlyCollection<int> RequiredVariables { get; init; }
}