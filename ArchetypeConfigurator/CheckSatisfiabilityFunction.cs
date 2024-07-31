namespace ArchetypeConfigurator;

internal static class CheckSatisfiabilityFunction
{
    public static bool Exec(List<List<int>> formula, int literal, HashSet<int> knownValues, HashSet<int> disabledValues)
    {
        if (knownValues.Contains(literal)) return true;
        if (disabledValues.Contains(literal)) return false;
        var extendedFormula = ExtendFormula(formula, literal, disabledValues);

        var result = DPLLSolver.Solve(extendedFormula, new Dictionary<int, bool>());

        if (result != null)
        {
            var results = result.Select(x => x.Value ? x.Key : -x.Key);
            knownValues.UnionWith(results);
            return true;
        }

        disabledValues.Add(literal);
        return false;
    }

    private static List<List<int>> ExtendFormula(List<List<int>> formula, int literal, HashSet<int> disabledValues)
    {
        var extendedFormula = new List<List<int>>(formula) {
            ([literal]) };

        foreach (var disabledLiteral in disabledValues)
        {
            extendedFormula.Add([-disabledLiteral]);
        }

        return extendedFormula;
    }
}