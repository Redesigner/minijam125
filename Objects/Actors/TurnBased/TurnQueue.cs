using Godot;
using System;

public class TurnQueue : Node2D
{
    private bool _awaitingActions = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Press 'Space' to begin your turn.");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("StartTurn"))
        {
            GetActionsForEachActor();
        }
    }

    public async void GetActionsForEachActor()
    {
        if (_awaitingActions)
        {
            return;
        }

        foreach (Node actor in GetChildren())
        {
            TurnBasedActor turnBasedActor = (TurnBasedActor) actor;
            turnBasedActor.CallDeferred("SetActionForTurn");

            await ToSignal(turnBasedActor, "ActionReady");
            GD.Print($"Actor {turnBasedActor.Name} readied action");
        }

        ExecuteActions();
    }

    public void ExecuteActions()
    {
        foreach (Node actor in GetChildren())
        {
            TurnBasedActor turnBasedActor = (TurnBasedActor)actor;
            TurnAction action = turnBasedActor.GetUpcomingAction();
            GD.Print($"{turnBasedActor.Name} used action: {action.ActionName}");
        }
        GD.Print("Turn end.\n");
    }
}
