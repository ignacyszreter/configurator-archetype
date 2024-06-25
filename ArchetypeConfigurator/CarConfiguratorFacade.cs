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
        var variables = TestVars.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }

    public void RevertDecision(int literal)
    {
        var variable = _variables.First(x => x.Literal == literal);
        //TODO: do not do it when TestVars fails
        variable.Reset();
        _disabledValues.Clear();
        var variables = TestVars.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }

    public bool CanConfigureCar(IReadOnlyCollection<int> partIds)
    {
        var clauses = RulesAndPartsToClauses.ConvertRulesAndPartsToClauses(_includeRules, _excludeRules, partIds);
        return DPLLSolver.Solve(clauses, new Dictionary<int, bool>()) is not null;
    }

    public bool IsFulfilled()
    {
        //should be fulfilled if include rule contains only one choice and it is blocked?
        if (!_variables.Any(x => x.IsUserDecision)) return false;
        var assignments = _variables.ToDictionary(x => x.Literal, x => x.Value ?? false);
        return DPLLSolver.IsFormulaSatisfied
            (RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), assignments);
    }

    public List<List<int>> GetMissingClauses()
    {
        var assignments = _variables.ToDictionary(x => x.Literal, x => x.Value ?? false);
        return DPLLSolver.GetMissingClauses(RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules),
            assignments);
    }
}