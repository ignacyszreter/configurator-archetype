namespace ArchetypeConfigurator;

internal static class TestVars
{
    public static HashSet<Variable> Exec(HashSet<int> knownValues, HashSet<int> disabledValues, List<List<int>> clauses,
        HashSet<Variable> variables)
    {
        var decisions = variables.Where(x => x.IsUserDecision).ToHashSet();
        foreach (var unassignedVariable in variables.Where(x => !decisions.Contains(x)))
        {
            if (unassignedVariable.Value.HasValue) continue;
            var canBeTrue = TestSat.Exec(GetFormula(clauses, decisions), unassignedVariable.Literal, knownValues, disabledValues);
            var canBeFalse = TestSat.Exec(GetFormula(clauses, decisions), -unassignedVariable.Literal, knownValues, disabledValues);
            if (!canBeTrue && !canBeFalse) throw new InvalidOperationException("Literal is not satisfiable");
            if (!canBeTrue) unassignedVariable.Set(false);
            if (!canBeFalse) unassignedVariable.Set(true);
            if (canBeTrue && canBeFalse)
            {
                unassignedVariable.UnlockAndReset();
                // decisions.Single(x => x.Equals(status)).UnlockAndReset();;
            }
            else
            {
                unassignedVariable.Lock();
                // decisions.Single(x => x.Equals(status)).Lock();
            }
        }
        return variables;
    }

    private static List<List<int>> GetFormula(List<List<int>> clauses, HashSet<Variable> decisions)
    {
        var formula = new List<List<int>>();
        foreach (var decision in decisions)
        {
            if (decision.Value.HasValue)
            {
                formula.Add([decision.Value.Value ? decision.Literal : -decision.Literal]);
            }
        }

        formula.AddRange(clauses);
        return formula;
    }
}



internal static class TestSat
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