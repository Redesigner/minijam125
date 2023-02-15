using Godot;
using System;

public class CombatSceneRoot : Node2D
{
    [Export] private NodePath _turnQueuePath;
    private TurnQueue _turnQueue;

    [Export] private NodePath _combatUIPath;
    private CombatUI _combatUI;

    public override void _Ready()
    {
        _turnQueue = GetNode<TurnQueue>(_turnQueuePath);
        _combatUI = GetNode<CombatUI>(_combatUIPath);

        _combatUI.AttachTurnQueue(_turnQueue);
    }
}
