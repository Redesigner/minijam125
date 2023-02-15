using Godot;
using System;
using System.Collections.Generic;

public class TBPlayer : TBActor
{
    private bool _awaitingInput = false;
    private bool _awaitingTarget = false;

    public override void ChooseAction()
    {
        
    }
    public override void ChooseTarget(Godot.Collections.Array<TBActor> targets)
    {

    }

    public override void _Input(InputEvent @event)
    {
    }
}
