using Godot;
using System;

public class TurnBasedActor : Node2D
{
    private TurnAction _pendingAction;
    [Signal] public delegate void ActionReady();

    public override void _Ready()
    {
        _pendingAction = new TurnAction("Do nothing.");
    }

    public virtual void SetActionForTurn()
    {
        EmitSignal("ActionReady");
    }

    protected void SetUpcomingAction(TurnAction action)
    {
        _pendingAction = action;
    }

    public TurnAction GetUpcomingAction()
    {
        return _pendingAction;
    }
}
