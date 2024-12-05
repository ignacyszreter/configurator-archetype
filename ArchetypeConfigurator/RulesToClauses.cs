namespace ArchetypeConfigurator;

internal static class RulesToClauses
{
    public static List<List<int>> Exec(IReadOnlyCollection<IncludeRule> includeRules,
        IReadOnlyCollection<ExcludeRule> excludeRules)
    {
        var clauses = new List<List<int>>();

        foreach (var includeRule in includeRules)
        {
            var includeRuleClause = new List<int>();
            includeRuleClause.Add(-includeRule.Id);
            includeRuleClause.AddRange(includeRule.RequiredVariables);
            clauses.Add(includeRuleClause);
        }

        foreach (var excludeRule in excludeRules)
        {
            clauses.Add([-excludeRule.Id, -excludeRule.ExcludedVariableId]);
        }

        return clauses;
    }
}