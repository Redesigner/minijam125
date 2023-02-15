using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class TBEnemy : TBActor
{
    private RandomNumberGenerator _random = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        _random.Randomize();
        foreach (Node node in GetChildren())
        {
            if(node is TBAction)
            {
                _availableActions.Add((TBAction)node);
            }
        }
    }

    public override void ChooseAction()
    {
        int randomActionIndex = _random.RandiRange(0, _availableActions.Count - 1);
        SetUpcomingAction(_availableActions[randomActionIndex]);
    }

    public override void ChooseTarget(Array<TBActor> targets)
    {
        _availableTargets = targets;
        int randomActionIndex = _random.RandiRange(0, targets.Count - 1);
        SetUpcomingTarget(_availableTargets[randomActionIndex]);
    }
}
