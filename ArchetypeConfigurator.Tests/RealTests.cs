﻿using FluentAssertions;

namespace ArchetypeConfigurator.Tests;

internal class RealTests
{
    private ConfiguratorFacade _configuratorFacade;
    private const int WheelId = 1;
    private const int EngineId = 2;
    private const int LeatherSeatsId = 3;
    private const int RoofRackId = 4;

    [SetUp]
    public void Setup()
    {
        _configuratorFacade = new ConfiguratorFacade();
        _configuratorFacade.AddVariable(WheelId);
        _configuratorFacade.AddVariable(EngineId);
        _configuratorFacade.AddVariable(LeatherSeatsId);
        _configuratorFacade.AddVariable(RoofRackId);
    }

    [Test]
    public void WhenLiteralIsLocked_ItCannotBeChanged()
    {
        _configuratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _configuratorFacade.PickVariable(WheelId);
        _configuratorFacade.PickVariable(LeatherSeatsId);
        _configuratorFacade.PickVariable(RoofRackId);
        var action = () => _configuratorFacade.PickVariable(EngineId);
        action.Should().Throw<InvalidOperationException>().WithMessage("Cannot set locked status");
    }

    [Test]
    public void ShouldLockLiteral_IfItIsExcluded()
    {
        _configuratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));

        _configuratorFacade.PickVariable(WheelId);

        _configuratorFacade.Variables.Should().HaveCount(4);
        _configuratorFacade.Variables.Should()
            .Contain(x => x.Id == EngineId && x.Value == false && x.Locked && !x.IsUserDecision);
        _configuratorFacade.Variables.Should()
            .Contain(x => x.Id == WheelId && x.Value == true && !x.Locked && x.IsUserDecision);
    }

    [Test]
    public void ShouldFail_IfThereIsNoSatisfyingRule()
    {
        _configuratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _configuratorFacade.PickVariable(WheelId);
        _configuratorFacade.AddIncludeRule(new IncludeRule(RoofRackId, [EngineId]));

        var action = () => _configuratorFacade.PickVariable(RoofRackId);
        action.Should().Throw<InvalidOperationException>().WithMessage("Literal is not satisfiable");
    }

    [Test]
    public void ThereAreNoRequiredParts_CannotBeSatisfied()
    {
        _configuratorFacade.AddIncludeRule(new IncludeRule(WheelId, new[] { EngineId, LeatherSeatsId }));
        _configuratorFacade.PickVariable(WheelId);
        _configuratorFacade.PickVariable(EngineId);
        _configuratorFacade.PickVariable(LeatherSeatsId);
    }

    [Test]
    public void RevertDecision_ShouldUnlockRequiredPart()
    {
        _configuratorFacade.AddExcludeRule(new ExcludeRule(WheelId, EngineId));
        _configuratorFacade.PickVariable(WheelId);

        _configuratorFacade.Variables.Should()
            .Contain(x => x.Id == EngineId && x.Value == false && x.Locked);
        
        _configuratorFacade.RevertDecision(WheelId);
        
        _configuratorFacade.Variables.Should()
            .Contain(x => x.Id == EngineId && x.Value == null && !x.Locked);
        
        _configuratorFacade.PickVariable(EngineId);
        
        _configuratorFacade.Variables.Should()
            .Contain(x => x.Id == EngineId && x.Value == true && !x.Locked);

    }
    
    [Test]
    public void ShouldNotSayConfigurationIsFulfilled_IfIncludeRuleIsOptional()
    {
        _configuratorFacade.AddIncludeRule(new IncludeRule(WheelId, [EngineId, LeatherSeatsId]));
        _configuratorFacade.PickVariable(WheelId);
        _configuratorFacade.GetMissingClauses().Should().HaveCount(1);
        _configuratorFacade.GetMissingClauses()[0].Should().BeEquivalentTo([-WheelId, EngineId, LeatherSeatsId]);

        _configuratorFacade.IsFulfilled().Should().BeFalse();
        
        _configuratorFacade.PickVariable(EngineId);
        
        _configuratorFacade.IsFulfilled().Should().BeTrue();
        _configuratorFacade.GetMissingClauses().Should().BeEmpty();
    }
    
    [Test]
    public void IsNotFulfilled_IfUserHasNotMadeAnyDecision()
    {
        _configuratorFacade.AddIncludeRule(new IncludeRule(WheelId, [EngineId, LeatherSeatsId]));

        _configuratorFacade.IsFulfilled().Should().BeFalse();
    }
    
    [Test]
    public void ConfigurationIsFulfilled_WhenIncludeRuleContainsOnlyOnePart_AndOnlyPartRequiringConclusionIsNeeded()
    {
        _configuratorFacade.AddIncludeRule(new IncludeRule(WheelId, [LeatherSeatsId]));
        _configuratorFacade.PickVariable(WheelId);

        _configuratorFacade.IsFulfilled().Should().BeTrue();
    }
}