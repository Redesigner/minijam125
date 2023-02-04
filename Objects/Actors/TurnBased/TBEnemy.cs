using Godot;
using System;
using System.Collections.Generic;

public class TBEnemy : TurnBasedActor
{
    private List<TurnAction> _availableActions = new List<TurnAction>();
    private RandomNumberGenerator _random = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        _random.Randomize();

        foreach (Node node in GetChildren())
        {
            if(node is TurnAction)
            {
                _availableActions.Add((TurnAction)node);
            }
        }
    }

    public override void SetActionForTurn()
    {
        int randomActionIndex = _random.RandiRange(0, _availableActions.Count - 1);
        GD.Print("random index: " + randomActionIndex);
        GD.Print($"{Name} chose action: {_availableActions[randomActionIndex].Name} out of {_availableActions.Count} possible actions");
        SetUpcomingAction(_availableActions[randomActionIndex]);
        EmitSignal("ActionReady");
    }
}
