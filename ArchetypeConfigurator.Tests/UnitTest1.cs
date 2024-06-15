// using ArchetypeConfigurator.V2;
//
// namespace ArchetypeConfigurator.Tests;
//
// public class Tests
// {
//     [SetUp]
//     public void Setup()
//     {
//     }
//
//     [Test]
//     public void Test1()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, false) }),
//             new Clause(new[] { new Literal(1, false), new Literal(2, true) }),
//             new Clause(new[] { new Literal(2, true), new Literal(3, true) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Assert.True(result);
//     }
//     
//     [Test]
//     public void TestThreeVariablesConflicting()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, true) }),
//             new Clause(new[] { new Literal(2, true), new Literal(3, false) }),
//             new Clause(new[] { new Literal(3, true), new Literal(1, false) }),
//             new Clause(new[] { new Literal(1, false), new Literal(2, false), new Literal(3, false) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Assert.False(result);
//     }
//     
//     [Test]
//     public void TestLargerFiveVariables()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, true), new Literal(3, true) }),
//             new Clause(new[] { new Literal(1, false), new Literal(2, false) }),
//             new Clause(new[] { new Literal(2, true), new Literal(3, false), new Literal(4, true) }),
//             new Clause(new[] { new Literal(2, false), new Literal(4, false) }),
//             new Clause(new[] { new Literal(3, true), new Literal(4, false), new Literal(5, true) }),
//             new Clause(new[] { new Literal(3, false), new Literal(5, false) }),
//             new Clause(new[] { new Literal(5, false), new Literal(4, false), new Literal(1, true) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Assert.False(result);
//     }
//     
//     [Test]
//     public void TestMultipleClausesFourVariables()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, true), new Literal(3, true), new Literal(4, true) }),
//             new Clause(new[] { new Literal(1, false), new Literal(2, false) }),
//             new Clause(new[] { new Literal(2, false), new Literal(3, false) }),
//             new Clause(new[] { new Literal(3, false), new Literal(4, false) }),
//             new Clause(new[] { new Literal(4, false), new Literal(1, false) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Assert.False(result);
//     }
//     
//     [Test]
//     public void TestExtendedChainOfVariables()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, true) }),
//             new Clause(new[] { new Literal(1, false), new Literal(3, false) }),
//             new Clause(new[] { new Literal(3, true), new Literal(4, true) }),
//             new Clause(new[] { new Literal(2, false), new Literal(4, false) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Assert.False(result);
//     }
//
//     [Test]
//     public void TestComplexSatisfiableScenario()
//     {
//         var clauses = new List<Clause>
//         {
//             new Clause(new[] { new Literal(1, true), new Literal(2, true), new Literal(3, true), new Literal(4, true) }),
//             new Clause(new[] { new Literal(1, true), new Literal(2, false), new Literal(3, true) }),
//             new Clause(new[] { new Literal(4, true) })
//         };
//
//         var formula = new CNFFormula(clauses);
//         var solver = new DPLLSolver();
//         bool result = solver.Solve(formula);
//         Console.WriteLine("SAT: " + result);
//         Assert.IsTrue(result);
//     }
//
//
//
// }