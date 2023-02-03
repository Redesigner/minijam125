using Godot;
using System;

// A struct containing the data for each character's behavior on any given turn.
public struct TurnAction
{
    [Export] public String ActionName;
    [Export] public int BeatsPerMeasure;

    public TurnAction(String actionName)
    {
        ActionName = actionName;
        BeatsPerMeasure = 4;
    }
}
