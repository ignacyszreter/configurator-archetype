namespace ArchetypeConfigurator;

internal static class RulesAndPartsToClauses
{
    public static List<List<int>> ConvertRulesAndPartsToClauses(IReadOnlyCollection<IncludeRule> includeRules,
        IReadOnlyCollection<ExcludeRule> excludeRules, IReadOnlyCollection<int> partIds)
    {
        var clauses = new List<List<int>>();

        foreach (var includeRule in includeRules)
        {
            var includeRuleClause = new List<int>();
            includeRuleClause.Add(-includeRule.Id);
            includeRuleClause.AddRange(includeRule.RequiredParts);
            clauses.Add(includeRuleClause);
        }

        foreach (var excludeRule in excludeRules)
        {
            if (partIds.Contains(excludeRule.Id) && partIds.Contains(excludeRule.ExcludedPartId))
            {
                clauses.Add([-excludeRule.Id, -excludeRule.ExcludedPartId]);
            }
        }

        var allLiterals = new HashSet<int>();
        foreach (var clause in clauses)
        {
            foreach (var literal in clause)
            {
                allLiterals.Add(literal);
            }
        }

        foreach (var literal in allLiterals)
        {
            if (partIds.Contains(literal))
            {
                clauses.Add([literal]);
            }
            else
            {
                clauses.Add([-literal]);
            }
        }

        return clauses;
    }
}