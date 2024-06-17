namespace ArchetypeConfigurator;

public class CarConfiguratorFacade
{
    private static readonly DPLLSolver Solver = new DPLLSolver();
    private readonly List<IncludeRule> _includeRules = new List<IncludeRule>();
    private readonly List<ExcludeRule> _excludeRules = new List<ExcludeRule>();
    
    public void AddIncludeRule(IncludeRule rule)
    {
        _includeRules.Add(rule);
    }
    
    public void AddExcludeRule(ExcludeRule rule)
    {
        _excludeRules.Add(rule);
    }
    
    public bool CanConfigureCar(IReadOnlyCollection<int> partIds)
    {
        var clauses = RulesAndPartsToClauses.ConvertRulesAndPartsToClauses(_includeRules, _excludeRules, partIds);
        return Solver.Solve(clauses, new Dictionary<int, bool>());
    }
}

public record IncludeRule
{
    public IncludeRule(int id, IReadOnlyCollection<int> requiredParts)
    {
        Id = id;
        RequiredParts = requiredParts;
    }

    public int Id { get; init; }
    public IReadOnlyCollection<int> RequiredParts { get; init; }
}

public record ExcludeRule(int Id, int ExcludedPartId);