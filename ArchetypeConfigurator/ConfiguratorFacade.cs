namespace ArchetypeConfigurator;

public class ConfiguratorFacade
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

    public void AddVariable(int id)
    {
        _variables.Add(new Variable(id));
    }

    public void PickVariable(int variableId)
    {
        var variable = _variables.First(x => x.Id == variableId);
        var clonedVariables = new HashSet<Variable>(_variables);
        //TODO: do not do it when CheckCurrentDecisionSetFunction fails
        variable.Set(true);
        _knownValues.Clear();
        var variables = CheckCurrentDecisionSetFunction.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }

    public void RevertDecision(int variableId)
    {
        var variable = _variables.First(x => x.Id == variableId);
        //TODO: do not do it when CheckCurrentDecisionSetFunction fails
        variable.Reset();
        _disabledValues.Clear();
        var variables = CheckCurrentDecisionSetFunction.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), _variables);
        _variables = variables;
    }
    
    public bool IsFulfilled()
    {
        //should be fulfilled if include rule contains only one choice and it is blocked?
        if (!_variables.Any(x => x.IsUserDecision)) return false;
        var assignments = _variables.ToDictionary(x => x.Id, x => x.Value ?? false);
        return DPLLSolver.IsFormulaSatisfied
            (RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), assignments);
    }

    public List<List<int>> GetMissingClauses()
    {
        var assignments = _variables.ToDictionary(x => x.Id, x => x.Value ?? false);
        return DPLLSolver.GetMissingClauses(RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules),
            assignments);
    }
}