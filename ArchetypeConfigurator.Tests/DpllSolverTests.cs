namespace ArchetypeConfigurator.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class DpllSolverTests
{
    [TestCase("47Variables_324Clauses_UNSAT", false)]
    [TestCase("3Variables_8Clauses_UNSAT", false)]
    [TestCase("12Variables_36Clauses_UNSAT", false)]
    [TestCase("12Variables_36Clauses_SAT", true)]
    [TestCase("2Variables_3Clauses_SAT", true)]
    // [TestCase("78Variables_364Clauses_SAT", true)] takes years to complete
    [TestCase("138Variables_519Clauses_SAT", true)]
    [TestCase("138Variables_519Clauses_UNSAT", false)]
    [TestCase("1Variable_2Clauses_UNSAT", false)]
    [TestCase("23Variables_75Clauses_SAT", true)]
    [Parallelizable(ParallelScope.Self)]
    public void Testt(string fileName, bool sat)
    {
        var filePath = Path.Combine("TestCases", $"{fileName}.txt");
        var clauses = LoadClauses(filePath);
        var result = DPLLSolver.Solve(clauses, new Dictionary<int, bool>());
        Assert.That(result is not null == sat);
    }
    
    public List<List<int>> LoadClauses(string filePath)
    {
        var clauses = new List<List<int>>();
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