using Godot;
using System;

public class TurnBasedActor : Node2D
{
    private TurnAction _pendingAction;
    [Export] public bool IsEnemy = false;
    [Signal] public delegate void ActionReady();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is TurnAction)
            {
                _pendingAction = (TurnAction) child;
                return;
            }
        }
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
