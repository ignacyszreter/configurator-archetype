using System;
using System.Collections.Generic;
using System.Linq;

public class Literal
{
    public int Variable { get; }
    public bool IsPositive { get; }

    public Literal(int variable, bool isPositive)
    {
        Variable = variable;
        IsPositive = isPositive;
    }

    public override string ToString()
    {
        return (IsPositive ? "" : "-") + Variable;
    }

    // Negate the literal
    public Literal Negate() => new Literal(Variable, !IsPositive);
}

public class Clause
{
    public List<Literal> Literals { get; }

    public Clause(IEnumerable<Literal> literals)
    {
        Literals = literals.ToList();
    }

    public override string ToString()
    {
        return string.Join(" OR ", Literals);
    }
}

public class CNFFormula
{
    public List<Clause> Clauses { get; }

    public CNFFormula(IEnumerable<Clause> clauses)
    {
        Clauses = clauses.ToList();
    }
}

public class DPLLSolver
{
    public bool Solve(CNFFormula formula)
    {
        return DPLL(formula.Clauses, new Dictionary<int, bool>());
    }

    private bool DPLL(List<Clause> clauses, Dictionary<int, bool> assignments)
    {
        while (true)
        {
            bool progress = false;
            clauses = PropagateUnits(clauses, assignments, ref progress);

            if (!progress)
            {
                if (clauses.Count == 0) return true; // Satisfied
                if (clauses.Any(clause => !clause.Literals.Any())) return false; // Conflict

                // Select a variable to assign
                var variable = SelectVariable(clauses);

                // Try true and then false
                foreach (var value in new[] { true, false })
                {
                    var newAssignments = new Dictionary<int, bool>(assignments) { [variable] = value };
                    var newClauses = clauses.Select(clause => new Clause(clause.Literals.Where(l => l.Variable != variable))).ToList();
                    if (DPLL(newClauses, newAssignments))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    private List<Clause> PropagateUnits(List<Clause> clauses, Dictionary<int, bool> assignments, ref bool progress)
    {
        var unitClauses = clauses.Where(c => c.Literals.Count == 1).ToList();
        foreach (var unit in unitClauses)
        {
            var literal = unit.Literals.First();
            if (assignments.TryGetValue(literal.Variable, out bool assignedValue))
            {
                if (assignedValue != literal.IsPositive)
                {
                    return new List<Clause>(); // Conflict
                }
            }
            else
            {
                assignments[literal.Variable] = literal.IsPositive;
                clauses = clauses.Where(clause => !clause.Literals.Any(l => l.Variable == literal.Variable && l.IsPositive != literal.IsPositive)).ToList();
                progress = true;
            }
        }
        return clauses;
    }


    private int SelectVariable(List<Clause> clauses)
    {
        return clauses.SelectMany(c => c.Literals)
                      .GroupBy(l => l.Variable)
                      .OrderByDescending(g => g.Count())
                      .First().Key;
    }
}

