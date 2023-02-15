using Godot;
using System;

public class TurnQueue : Node2D
{
    [Export] public int BPM = 120;

    private bool _awaitingActions = false;
    private bool _playing = false;
    private bool _awaitingTarget = false;

    private float _currentTime = 0.0f;

    private float _bpmTimeRatio = 1.0f;

    private Godot.Collections.Array<TBActor> _heroes = new Godot.Collections.Array<TBActor>();
    private Godot.Collections.Array<TBActor> _enemies = new Godot.Collections.Array<TBActor>();

    [Export] private NodePath _actionSelectorPath;
    private ActionSelector _actionSelector;

    [Export] private NodePath _targetSelectorPath;
    private TargetSelector _targetSelector;

    [Export] private NodePath _statusBarPath;
    private StatusBar _statusBar;

    [Export] PackedScene _victoryScreen;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _actionSelector = GetNode<ActionSelector>(_actionSelectorPath);
        _targetSelector = GetNode<TargetSelector>(_targetSelectorPath);
        _statusBar = GetNode<StatusBar>(_statusBarPath);

        _bpmTimeRatio = (float) BPM / 60.0f;
        GD.Print("Press 'Space' to begin your turn.");

        foreach (Node child in GetChildren())
        {
            if (child is TBActor)
            {
                TBActor actor = (TBActor) child;
                if (actor.IsEnemy)
                {
                    _enemies.Add(actor);
                }
                else
                {
                    _heroes.Add(actor);
                }
            }
        }
        GD.Print($"Teams populated -- Heroes: {_heroes.Count}, Enemies: {_enemies.Count}");
        _statusBar.CallDeferred("SetPlayers",_heroes);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_playing)
        {
            _currentTime += delta * _bpmTimeRatio;
            Godot.Collections.Array<TBAction> actionsThisBeat = new Godot.Collections.Array<TBAction>();

            foreach (TBActor actor in _enemies)
            {
                TBAction track = actor.GetUpcomingAction();
                if (DidBeatOccur(track))
                {
                    actionsThisBeat.Add(track);
                    track.BeatsToPlay.RemoveAt(0);
                }
            }

            foreach (TBActor actor in _heroes)
            {
                TBAction track = actor.GetUpcomingAction();
                if (DidBeatOccur(track))
                {
                    actionsThisBeat.Add(track);
                    track.BeatsToPlay.RemoveAt(0);
                }
            }

            foreach (TBAction action in actionsThisBeat)
            {
                action.PlayBeat();
                action.Target.AccumulateDamage(action.DamagePerHit);
            }

            TeamTakeDamage(_heroes);
            TeamTakeDamage(_enemies);

            _statusBar.UpdateDisplay();

            if (actionsThisBeat.Count > 0)
            {
                // GD.Print(String.Join(", ", actionsThisBeat));
            }

            if (_currentTime >= 4.0f)
            {
                EndTurn();
            }
        }
    }

    private void EndTurn()
    {
        // GD.Print("Finished playing attacks.");
        _playing = false;
        _awaitingActions = false;
        _currentTime = 0.0f;
        if (_enemies.Count <= 0)
        {
            GetTree().ChangeSceneTo(_victoryScreen);
        }
    }

    private bool DidBeatOccur(TBAction track)
    {
        if (track.BeatsToPlay.Count <= 0)
        {
            return false;
        }
        return track.BeatsToPlay[0] <= _currentTime;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("StartTurn") && !_playing && !_awaitingActions)
        {
            _awaitingActions = true;
            PlanTurns();
        }
    }

    public async void PlanTurns()
    {
        // Handle each actor's turn
        foreach (TBActor turnBasedActor in _heroes)
        {
            // Select action
            _actionSelector.Visible = true;
            _actionSelector.CallDeferred("SetActionList", turnBasedActor.GetActions());
            _actionSelector.SetFocus(true);
            _actionSelector.SetActorName(turnBasedActor.Name);
            await ToSignal(_actionSelector, "ActionReady");
            _actionSelector.SetFocus(false); 
            _actionSelector.ClearSelection();

            // Select each target
            turnBasedActor.SetUpcomingAction(_actionSelector.GetSelectedAction());
            _targetSelector.SetTargetList(GetTargets(true));
            await ToSignal(_targetSelector, "TargetSelected");

            // GD.Print("TurnQueue received target selection");

            turnBasedActor.GetUpcomingAction().Target = _targetSelector.GetSelected();
            turnBasedActor.GetUpcomingAction().RefreshBeats();
        }
        foreach (TBActor actor in _enemies)
        {
            // GD.Print($"\n{actor.Name}'s turn!");
            actor.ChooseAction();
            actor.ChooseTarget(GetTargets(false));
            actor.GetUpcomingAction().RefreshBeats();
        }
        _actionSelector.Visible = false;
        _playing = true;
    }

    public Godot.Collections.Array<TBActor> GetTargets(bool targetEnemies)
    {
        if (targetEnemies)
        {
            return _enemies;
        }
        return _heroes;
    }

    private void TeamTakeDamage(Godot.Collections.Array<TBActor> team)
    {
        Godot.Collections.Array<TBActor> deathQueue = new Godot.Collections.Array<TBActor>();
        foreach (TBActor actor in team)
        {
            if (!actor.TakeAllIncomingDamage())
            {
                deathQueue.Add(actor);
            }
        }
        foreach (TBActor actor in deathQueue)
        {
            team.Remove(actor);
            actor.QueueFree();
            //actor.PlayDeath();
        }
    }
}
