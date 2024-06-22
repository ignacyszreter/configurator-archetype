namespace ArchetypeConfigurator;

public static class DPLLSolver
{
    public static Dictionary<int, bool>? Solve(List<List<int>> clauses, Dictionary<int, bool> assignment)
    {
        if (clauses.All(clause => clause.Any(literal =>
                assignment.ContainsKey(Math.Abs(literal)) && assignment[Math.Abs(literal)] == (literal > 0))))
        {
            return assignment;
        }

        if (clauses.Any(clause => clause.All(literal =>
                assignment.ContainsKey(Math.Abs(literal)) && assignment[Math.Abs(literal)] != (literal > 0))))
        {
            return null;
        }

        var unassigned = clauses.SelectMany(clause => clause)
            .Select(literal => Math.Abs(literal))
            .Distinct()
            .FirstOrDefault(variable => !assignment.ContainsKey(variable));

        if (unassigned == 0)
        {
            return null;
        }

        var assignmentWithTrue = new Dictionary<int, bool>(assignment)
        {
            [unassigned] = true
        };

        var result = Solve(clauses, assignmentWithTrue);
        if (result is not null)
        {
            return result;
        }

        var assignmentWithFalse = new Dictionary<int, bool>(assignment)
        {
            [unassigned] = false
        };

        return Solve(clauses, assignmentWithFalse);
    }

    public static bool IsFormulaSatisfied(List<List<int>> clauses, Dictionary<int, bool> assignment)
    {
        return clauses.All(clause => clause.Any(literal =>
            assignment.ContainsKey(Math.Abs(literal)) && assignment[Math.Abs(literal)] == (literal > 0))
    }
}