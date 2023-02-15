using Godot;
using System;

public class CombatUI : Control
{
    private TurnQueue _turnQueue;

    [Export] private NodePath _statusBarPath;
    private StatusBar _statusBar;

    [Export] private NodePath _actionSelectorPath;
    private ActionSelector _actionSelector;

    [Export] private NodePath _targetSelectorPath;
    private TargetSelector _targetSelector;

    public override void _Ready()
    {
        _statusBar = GetNode<StatusBar>(_statusBarPath);
        _actionSelector = GetNode<ActionSelector>(_actionSelectorPath);
        _targetSelector = GetNode<TargetSelector>(_targetSelectorPath);
    }

    public void AttachTurnQueue(TurnQueue turnQueue)
    {
        _turnQueue = turnQueue;
        _turnQueue.Connect("TurnStarted", this, "UpdatePlayerActions");
        _statusBar.SetPlayers(turnQueue.GetTeam(false));
        UpdatePlayerActions();
    }

    private void ActivateActionSelector(TBPlayer player)
    {
        _actionSelector.Visible = true;
        _actionSelector.Focus();
        _actionSelector.SetPlayer(player);
    }

    private void ActivateTargetSelector(TBPlayer player)
    {
        TBAction currentAction = player.GetPendingAction();
        _targetSelector.Visible = true;
        bool targetEnemyTeam = !player.IsEnemy;
        if (currentAction.TargetAllies())
        {
            targetEnemyTeam = !targetEnemyTeam;
        }
        _targetSelector.SetPlayer(player);
        _targetSelector.SetTargetList(_turnQueue.GetTeam(targetEnemyTeam));
        _targetSelector.Focus();
    }

    private async void UpdatePlayerActions()
    {
        foreach (TBPlayer player in _turnQueue.GetTeam(false))
        {
            if (!player.IsReady())
            {
                ActivateActionSelector(player);
                await (ToSignal(_actionSelector, "ActionSelected"));
                _actionSelector.Unfocus();
                if (!player.GetPendingAction().RequiresTarget())
                {
                    _actionSelector.ClearSelection();
                    continue;
                }
                ActivateTargetSelector(player);
                await (ToSignal(_targetSelector, "TargetSelected"));

                _actionSelector.ClearSelection();
                _targetSelector.Visible = false;
            }
        }
        _actionSelector.Visible = false;
    }
}
