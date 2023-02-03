using Godot;
using System;
using System.Collections.Generic;

public class TurnBasedPlayer : TurnBasedActor
{
    private List<TurnAction> _availableActions;
    private bool _awaitingInput = false;

    public override void _Ready()
    {
        base._Ready();
        _availableActions = new List<TurnAction>{ new TurnAction("Kick"), new TurnAction("Punch") };
    }

    public override void SetActionForTurn()
    {
        String message = "Please choose an action";
        for (int i = 0; i < _availableActions.Count; i++)
        {
            message += $" {i + 1}: {_availableActions[i].ActionName}";
            if (i < _availableActions.Count - 1)
            {
                message += ", ";
            }
        }
        GD.Print(message);
        _awaitingInput = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_awaitingInput)
        {
            return;
        }
        if (!(@event is InputEventKey))
        {
            return;
        }
        InputEventKey inputEventKey = (InputEventKey) @event;
        if (!inputEventKey.IsPressed())
        {
            return;
        }
        uint numKeyPressed = inputEventKey.Scancode - 48;
        if (numKeyPressed > 0 && numKeyPressed <= _availableActions.Count)
        {
            SetUpcomingAction(_availableActions[(int) numKeyPressed - 1]);
            EmitSignal("ActionReady");
        }
    }
}
