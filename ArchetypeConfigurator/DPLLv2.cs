namespace ArchetypeConfigurator.V2;


using System;
using System.Collections.Generic;
using System.Linq;

public class DPLLv2
{
    // Represents a literal with a value and a flag indicating if it is negated
    public class Literal
    {
        public int Var { get; set; }
        public bool IsNegated { get; set; }

        public Literal(int var, bool isNegated)
        {
            Var = var;
            IsNegated = isNegated;
        }

        // Returns the inverse of the current literal
        public Literal Negate()
        {
            return new Literal(Var, !IsNegated);
        }
    }

    // Represents a clause as a list of literals
    public class Clause
    {
        public List<Literal> Literals { get; set; } = new List<Literal>();

        // Adds a literal to the clause
        public void AddLiteral(Literal literal)
        {
            Literals.Add(literal);
        }
    }

    // The main function to check if there is a satisfying assignment
    public bool Solve(List<Clause> clauses, Dictionary<int, bool> assignments)
    {
        if (!clauses.Any())
        {
            return true;
        }

        // Unit propagation
        var unitClause = clauses.FirstOrDefault(c => c.Literals.Count == 1);
        if (unitClause != null)
        {
            var literal = unitClause.Literals.First();
            var varAssignments = new Dictionary<int, bool>(assignments);
            varAssignments[literal.Var] = !literal.IsNegated;
            var reducedClauses = ApplyAssignment(clauses, literal.Var, !literal.IsNegated);
            return Solve(reducedClauses, varAssignments);
        }

        // Pure literal elimination
        var pureLiterals = FindPureLiterals(clauses);
        if (pureLiterals.Any())
        {
            var pureLiteral = pureLiterals.First();
            var varAssignments = new Dictionary<int, bool>(assignments);
            varAssignments[pureLiteral.Var] = !pureLiteral.IsNegated;
            var reducedClauses = ApplyAssignment(clauses, pureLiteral.Var, !pureLiteral.IsNegated);
            return Solve(reducedClauses, varAssignments);
        }

        // Choose the first unassigned variable for branching
        var firstClause = clauses.First();
        var firstLiteral = firstClause.Literals.First(l => !assignments.ContainsKey(l.Var));
        var tryTrue = new Dictionary<int, bool>(assignments) { [firstLiteral.Var] = true };
        var tryFalse = new Dictionary<int, bool>(assignments) { [firstLiteral.Var] = false };

        return Solve(ApplyAssignment(clauses, firstLiteral.Var, true), tryTrue) ||
               Solve(ApplyAssignment(clauses, firstLiteral.Var, false), tryFalse);
    }

    // Applies the given assignment to all clauses and returns the new set of clauses
    private List<Clause> ApplyAssignment(List<Clause> clauses, int var, bool value)
    {
        return clauses.Select(clause => new Clause
            {
                Literals = clause.Literals.Where(l => l.Var != var || l.IsNegated == value).ToList()
            })
            .Where(clause => clause.Literals.Count > 0)
            .ToList();
    }

    // Finds all pure literals in the set of clauses
    private IEnumerable<Literal> FindPureLiterals(List<Clause> clauses)
    {
        var literals = clauses.SelectMany(c => c.Literals).GroupBy(l => l.Var);
        foreach (var group in literals)
        {
            bool allPos = group.All(l => !l.IsNegated);
            bool allNeg = group.All(l => l.IsNegated);
            if (allPos || allNeg)
            {
                yield return new Literal(group.Key, allNeg);
            }
        }
    }
}

// Usage
class Program
{
    static void Main()
    {
       
    }
}
