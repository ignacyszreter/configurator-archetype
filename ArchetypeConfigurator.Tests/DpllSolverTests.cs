namespace ArchetypeConfigurator.Tests;

using ArchetypeConfigurator.V2;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class DpllSolverTests
{
    [TestCase("47Variables_324Clauses_UNSAT", false)]
    [TestCase("3Variables_8Clauses_UNSAT", false)]
    [TestCase("12Variables_36Clauses_UNSAT", false)]
    [TestCase("12Variables_36Clauses_SAT", true)]
    [TestCase("2Variables_3Clauses_SAT", true)]
    [TestCase("78Variables_364Clauses_SAT", true)]
    [TestCase("138Variables_518Clauses_SAT", true)]
    [TestCase("138Variables_518Clauses_UNSAT", false)]
    [TestCase("1Variable_2Clauses_UNSAT", false)]
    [Parallelizable(ParallelScope.Self)]
    public void Testt(string fileName, bool sat)
    {
        var solver = new DPLLSolver();
        var filePath = Path.Combine("TestCases", $"{fileName}.txt");
        var clauses = LoadClauses(filePath);
        var result = solver.Solve(clauses, new Dictionary<int, bool>());
        Assert.That(result == sat);
    }
    
    public List<List<int>> LoadClauses(string filePath)
    {
        List<List<int>> clauses = new List<List<int>>();
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var numbers = line.Split(' ')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(int.Parse)
                .ToList();
            clauses.Add(numbers);
        }
        return clauses;
    }
}