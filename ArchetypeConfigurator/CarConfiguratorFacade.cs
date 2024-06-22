namespace ArchetypeConfigurator;

public class CarConfiguratorFacade
{
    private readonly List<IncludeRule> _includeRules = new List<IncludeRule>();
    private readonly List<ExcludeRule> _excludeRules = new List<ExcludeRule>();
    private HashSet<int> _knownValues = new HashSet<int>();
    private HashSet<int> _disabledValues = new HashSet<int>();
    private HashSet<Variable> _variables = new HashSet<Variable>();
    public IReadOnlySet<Variable> Variables => _variables;


    public void AddIncludeRule(IncludeRule rule)
    {
        _includeRules.Add(rule);
    }

    public void AddExcludeRule(ExcludeRule rule)
    {
        _excludeRules.Add(rule);
    }

    public void AddVariable(int literal)
    {
        _variables.Add(new Variable(literal));
    }

    public void MakeDecision(int literal)
    {
        var variable = _variables.First(x => x.Literal == literal);
        //TODO: do not do it when TestVars fails
        variable.Set(true);
        _knownValues.Clear();
        var variables =  TestVars.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }

    public void RevertDecision(int literal)
    {
        var variable = _variables.First(x => x.Literal == literal);
        //TODO: do not do it when TestVars fails
        _disabledValues.Clear();
        var variables =  TestVars.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }

    public bool CanConfigureCar(IReadOnlyCollection<int> partIds)
    {
        var clauses = RulesAndPartsToClauses.ConvertRulesAndPartsToClauses(_includeRules, _excludeRules, partIds);
        return DPLLSolver.Solve(clauses, new Dictionary<int, bool>()) is not null;
    }
}

public class Variable
{
    public bool Locked { get; private set; }
    public bool? Value { get; private set; }
    public int Literal { get; }
    public bool IsUserDecision => !Locked && Value.HasValue;

    public void Lock()
    {
        if (Value is null) throw new InvalidOperationException("Cannot lock status without value");
        Locked = true;
    }

    public void UnlockAndReset()
    {
        Locked = false;
        Value = null;
    }

    public void Set(bool value)
    {
        if (Locked) throw new InvalidOperationException("Cannot set locked status");
        Value = value;
    }

    public Variable(int literal)
    {
        Literal = literal;
        Locked = false;
        Value = null;
    }

    public Variable(int literal, bool? value)
    {
        Value = value;
        Literal = literal;
        Locked = false;
    }

    protected bool Equals(Variable other)
    {
        return Locked == other.Locked && Value == other.Value && Literal == other.Literal;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Literal == ((Variable)obj).Literal;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Literal);
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

    //only one of the required parts must be present
    public IReadOnlyCollection<int> RequiredParts { get; init; }
}

public record ExcludeRule(int Id, int ExcludedPartId);