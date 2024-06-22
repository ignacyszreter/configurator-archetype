using FluentAssertions;

namespace ArchetypeConfigurator.Tests;

internal class RealTests
{
    private CarConfiguratorFacade _carConfiguratorFacade;
    private const int WheelId = 1;
    private const int EngineId = 2;
    private const int LeatherSeatsId = 3;
    private const int RoofRackId = 4;

    [SetUp]
    public void Setup()
    {
        _carConfiguratorFacade = new CarConfiguratorFacade();
        _carConfiguratorFacade.AddVariable(WheelId);
        _carConfiguratorFacade.AddVariable(EngineId);
        _carConfiguratorFacade.AddVariable(LeatherSeatsId);
        _carConfiguratorFacade.AddVariable(RoofRackId);
    }

    [Test]
    public void WhenLiteralIsLocked_ItCannotBeChanged()
    {
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _carConfiguratorFacade.MakeDecision(WheelId);
        _carConfiguratorFacade.MakeDecision(LeatherSeatsId);
        _carConfiguratorFacade.MakeDecision(RoofRackId);
        var action = () => _carConfiguratorFacade.MakeDecision(EngineId);
        action.Should().Throw<InvalidOperationException>().WithMessage("Cannot set locked status");
    }

    [Test]
    public void ShouldLockLiteral_IfItIsExcluded()
    {
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));

        _carConfiguratorFacade.MakeDecision(WheelId);

        _carConfiguratorFacade.Variables.Should().HaveCount(4);
        _carConfiguratorFacade.Variables.Should()
            .Contain(x => x.Literal == EngineId && x.Value == false && x.Locked && !x.IsUserDecision);
        _carConfiguratorFacade.Variables.Should()
            .Contain(x => x.Literal == WheelId && x.Value == true && !x.Locked && x.IsUserDecision);
    }

    [Test]
    public void ShouldFail_IfThereIsNoSatisfyingRule()
    {
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _carConfiguratorFacade.MakeDecision(WheelId);
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(RoofRackId, [EngineId]));

        var action = () => _carConfiguratorFacade.MakeDecision(RoofRackId);
        action.Should().Throw<InvalidOperationException>().WithMessage("Literal is not satisfiable");
    }

    [Test]
    public void ThereAreNoRequiredParts_CannotBeSatisfied()
    {
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(WheelId, new[] { EngineId, LeatherSeatsId }));
        _carConfiguratorFacade.MakeDecision(WheelId);
        _carConfiguratorFacade.MakeDecision(EngineId);
        _carConfiguratorFacade.MakeDecision(LeatherSeatsId);
    }

    [Test]
    public void RevertDecision_ShouldUnlockRequiredPart()
    {
        _carConfiguratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _carConfiguratorFacade.MakeDecision(WheelId);

        _carConfiguratorFacade.Variables.Should()
            .Contain(x => x.Literal == EngineId && x.Value == false && x.Locked);
        
        _carConfiguratorFacade.RevertDecision(WheelId);
        
        _carConfiguratorFacade.Variables.Should()
            .Contain(x => x.Literal == EngineId && x.Value == null && !x.Locked);
        
        _carConfiguratorFacade.MakeDecision(EngineId);
        
        _carConfiguratorFacade.Variables.Should()
            .Contain(x => x.Literal == EngineId && x.Value == true && !x.Locked);

    }
    
    [Test]
    public void ShouldNotSayConfigurationIsFulfilled_IFThereAreMissingParts()
    {
        _carConfiguratorFacade.AddIncludeRule(new IncludeRule(WheelId, [EngineId]));
        _carConfiguratorFacade.MakeDecision(WheelId);

        _carConfiguratorFacade.IsFullfilled().Should.BeFalse();

    }
}