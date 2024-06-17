namespace ArchetypeConfigurator.Tests;

internal class CarConfiguratorFacadeTests
{
    private CarConfiguratorFacade _carConfiguratorFacade;

    [SetUp]
    public void Setup()
    {
        _carConfiguratorFacade = new CarConfiguratorFacade();
    }

    [Test]
    public void TwoExcludingParts_CannotBeSatisfied()
    {
        var wheelId = 1;
        var engineId = 2;
        var leatherSeatsId = 3;
        var roofRackId = 4;
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(wheelId, engineId));
        var result =
            _carConfiguratorFacade.CanConfigureCar(new List<int> { wheelId, engineId, leatherSeatsId, roofRackId });
        Assert.False(result);
    }
    
    [Test]
    public void ExcludingRuleSatisfied_CanBeSatisfied()
    {
        var wheelId = 1;
        var engineId = 2;
        var leatherSeatsId = 3;
        var roofRackId = 4;
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(wheelId, engineId));
        var result =
            _carConfiguratorFacade.CanConfigureCar(new List<int> { wheelId, leatherSeatsId, roofRackId });
        Assert.True(result);
    }

    [Test]
    public void ThereAreNoRequiredParts_CannotBeSatisfied()
    {
        var wheelId = 1;
        var engineId = 2;
        var leatherSeatsId = 3;
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(wheelId, new[] { engineId, leatherSeatsId }));
        var result =
            _carConfiguratorFacade.CanConfigureCar(new List<int> { wheelId });
        Assert.False(result);
    }
    
    [Test]
    public void ThereIsOneOfRequiredParts_CanBeSatisfied()
    {
        var wheelId = 1;
        var engineId = 2;
        var leatherSeatsId = 3;
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(wheelId, new[] { engineId, leatherSeatsId }));
        var result =
            _carConfiguratorFacade.CanConfigureCar(new List<int> { wheelId, engineId });
        Assert.True(result);
    }
    
    [Test]
    public void ThereAreBothRequiredParts_CanBeSatisfied()
    {
        var wheelId = 1;
        var engineId = 2;
        var leatherSeatsId = 3;
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(wheelId, new[] { engineId, leatherSeatsId }));
        var result =
            _carConfiguratorFacade.CanConfigureCar(new List<int> { wheelId, engineId, leatherSeatsId });
        Assert.True(result);
    }
}