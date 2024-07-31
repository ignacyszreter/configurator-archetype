namespace ArchetypeConfigurator;

internal static class CheckCurrentDecisionSetFunction
{
    public static HashSet<Variables> Exec(HashSet<int> knownValues, HashSet<int> disabledValues, List<List<int>> clauses,
        HashSet<Variables> variables)
    {
        var userDecisions = variables.Where(x => x.IsUserDecision).ToHashSet();
        foreach (var unassignedVariable in variables.Where(x => !userDecisions.Contains(x)))
        {
            var canBeTrue = CheckSatisfiabilityFunction.Exec(GetFormula(clauses, userDecisions), unassignedVariable.Id, knownValues, disabledValues);
            var canBeFalse = CheckSatisfiabilityFunction.Exec(GetFormula(clauses, userDecisions), -unassignedVariable.Id, knownValues, disabledValues);
            if (!canBeTrue && !canBeFalse) throw new InvalidOperationException("Literal is not satisfiable");
            if (!canBeTrue && !unassignedVariable.Locked) unassignedVariable.Set(false);
            if (!canBeFalse && !unassignedVariable.Locked) unassignedVariable.Set(true);
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

    private static List<List<int>> GetFormula(List<List<int>> clauses, HashSet<Variables> decisions)
    {
        var formula = new List<List<int>>();
        foreach (var decision in decisions)
        {
            if (decision.Value.HasValue)
            {
                formula.Add([decision.Value.Value ? decision.Id : -decision.Id]);
            }
        }

        formula.AddRange(clauses);
        return formula;
    }
}