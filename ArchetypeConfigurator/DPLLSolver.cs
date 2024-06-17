namespace ArchetypeConfigurator;

public class DPLLSolver
{
    public bool Solve(List<List<int>> clauses, Dictionary<int, bool> assignment)
    {
        if (clauses.All(clause => clause.Any(literal => assignment.ContainsKey(Math.Abs(literal)) && assignment[Math.Abs(literal)] == (literal > 0))))
        {
            return true;
        }

        if (clauses.Any(clause => clause.All(literal => assignment.ContainsKey(Math.Abs(literal)) && assignment[Math.Abs(literal)] != (literal > 0))))
        {
            return false;
        }
        
        var unassigned = clauses.SelectMany(clause => clause)
            .Select(literal => Math.Abs(literal))
            .Distinct()
            .FirstOrDefault(variable => !assignment.ContainsKey(variable));

        if (unassigned == 0)
        {
            return false;
        }
        
        var assignmentWithTrue = new Dictionary<int, bool>(assignment)
        {
            [unassigned] = true
        };

        if (Solve(clauses, assignmentWithTrue))
        {
            return true;
        }
        
        var assignmentWithFalse = new Dictionary<int, bool>(assignment)
        {
            [unassigned] = false
        };

        return Solve(clauses, assignmentWithFalse);
    }
}