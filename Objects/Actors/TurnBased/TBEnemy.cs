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

    public void ChooseAction()
    {
        SetPendingAction(_availableActions[_random.RandiRange(0, _availableActions.Count - 1)]);
    }

    public void ChooseTarget(Godot.Collections.Array<TBActor> actorList)
    {
        SetPendingTarget(actorList[_random.RandiRange(0, actorList.Count - 1)]);
    }
}
