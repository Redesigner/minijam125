using Godot;
using System;
using System.Collections.Generic;

public class TurnBasedPlayer : TurnBasedActor
{
    private bool _awaitingInput = false;
    private bool _awaitingTarget = false;

    public override void SetActionForTurn()
    {
        String message = "Please choose an action";
        for (int i = 0; i < _availableActions.Count; i++)
        {
            message += $" {i + 1}: {_availableActions[i].Name}";
            if (i < _availableActions.Count - 1)
            {
                message += ", ";
            }
        }
        GD.Print(message);
        _awaitingInput = true;
    }
    public override void SetTargetForTurn()
    {
        if (_availableTargets.Count <= 0)
        {
            GD.Print("No targets to choose from.");
            base.SetTargetForTurn();
            return;
        }

        String message = "Please choose a target";
        for (int i = 0; i < _availableTargets.Count; i++)
        {
            message += $" {i + 1}: {_availableTargets[i].Name}";
            if (i < _availableTargets.Count - 1)
            {
                message += ", ";
            }
        }
        GD.Print(message);
        _awaitingTarget = true;
    }

    public override void _Input(InputEvent @event)
    {

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
        HandleRequestedInput(numKeyPressed);
    }

    private void HandleRequestedInput(uint scanCode)
    {
        if (_awaitingInput)
        {
            if (scanCode > 0 && scanCode <= _availableActions.Count)
            {
                SetUpcomingAction(_availableActions[(int) scanCode - 1]);
                EmitSignal("ActionReady");
                _awaitingInput = false;
                return;
            }
        }
        if (_awaitingTarget)
        {
            if (scanCode > 0 && scanCode <= _availableTargets.Count)
            {
                SetUpcomingTarget(_availableTargets[(int)scanCode - 1]);
                EmitSignal("TargetReady");
                _awaitingTarget = false;
                return;
            }
        }
    }
}
