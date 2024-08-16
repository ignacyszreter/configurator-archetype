namespace ArchetypeConfigurator;

public class ConfiguratorFacade
{
    private readonly IVariablesRepository _variablesRepository;
    private readonly List<IncludeRule> _includeRules = new List<IncludeRule>();
    private readonly List<ExcludeRule> _excludeRules = new List<ExcludeRule>();
    private HashSet<int> _knownValues = new HashSet<int>();
    private HashSet<int> _disabledValues = new HashSet<int>();

    public ConfiguratorFacade(IVariablesRepository variablesRepository)
    {
        _variablesRepository = variablesRepository;
    }

    public IReadOnlySet<Variable> Variables => _variablesRepository.GetVariables();

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
        var variables = _variablesRepository.GetVariables();
        variables.Add(new Variable(id));
        _variablesRepository.Save(variables);
    }

    public void PickVariable(int variableId)
    {
        var variables = _variablesRepository.GetVariables();
        var variable = variables.First(x => x.Id == variableId);
        //TODO: do not do it when CheckCurrentDecisionSetFunction fails
        variable.Set(true);
        _knownValues.Clear();
        var newVariables = CheckCurrentDecisionSetFunction.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), variables);
        _variablesRepository.Save(newVariables);
    }

    public void RevertDecision(int variableId)
    {
        var variables = _variablesRepository.GetVariables();
        var variable = variables.First(x => x.Id == variableId);
        variable.Reset();
        _disabledValues.Clear();
        var newVariables = CheckCurrentDecisionSetFunction.Exec(_knownValues, _disabledValues,
            RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), variables);
        _variablesRepository.Save(newVariables);
    }

    public bool IsFulfilled()
    {
        //should be fulfilled if include rule contains only one choice and it is blocked?
        var variables = _variablesRepository.GetVariables();
        if (!variables.Any(x => x.IsUserDecision)) return false;
        var assignments = variables.ToDictionary(x => x.Id, x => x.Value ?? false);
        return DPLLSolver.IsFormulaSatisfied
            (RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules), assignments);
    }

    public List<List<int>> GetMissingClauses()
    {
        var assignments = _variablesRepository.GetVariables().ToDictionary(x => x.Id, x => x.Value ?? false);
        return DPLLSolver.GetMissingClauses(RulesAndPartsToClauses.ConvertRulesToClauses(_includeRules, _excludeRules),
            assignments);
    }
}