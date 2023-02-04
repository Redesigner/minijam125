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

    private Godot.Collections.Array<TurnBasedActor> _heroes = new Godot.Collections.Array<TurnBasedActor>();
    private Godot.Collections.Array<TurnBasedActor> _enemies = new Godot.Collections.Array<TurnBasedActor>();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bpmTimeRatio = (float) BPM / 60.0f;
        GD.Print("Press 'Space' to begin your turn.");

        foreach (Node child in GetChildren())
        {
            if (child is TurnBasedActor)
            {
                TurnBasedActor actor = (TurnBasedActor) child;
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
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_playing)
        {
            _currentTime += delta * _bpmTimeRatio;
            bool didEnemyBlock = false;
            Godot.Collections.Array<TurnAction> actionsThisBeat = new Godot.Collections.Array<TurnAction>();

            foreach (TurnBasedActor actor in _enemies)
            {
                TurnAction track = actor.GetUpcomingAction();
                if (DidBeatOccur(track))
                {
                    if (!didEnemyBlock && actionsThisBeat.Count > 0)
                    {
                        actionsThisBeat.Clear();
                        didEnemyBlock = true;
                    }
                    actionsThisBeat.Add(track);
                    track.BeatsToPlay.RemoveAt(0);
                }
            }

            foreach (TurnBasedActor actor in _heroes)
            {
                TurnAction track = actor.GetUpcomingAction();
                if (DidBeatOccur(track))
                {
                    actionsThisBeat.Add(track);
                    track.BeatsToPlay.RemoveAt(0);
                }
            }

            foreach (TurnAction action in actionsThisBeat)
            {
                action.PlayBeat();
            }

            if (didEnemyBlock)
            {
                GD.Print("Enemy blocked attacks on this beat.");
            }
            else if (actionsThisBeat.Count > 0)
            {
                GD.Print(String.Join(", ", actionsThisBeat));
            }

            if (_currentTime >= 4.0f)
            {
                GD.Print("Finished playing attacks.");
                _playing = false;
                _currentTime = 0.0f;
            }
        }

    }

    private bool DidBeatOccur(TurnAction track)
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
        if (@event.IsActionPressed("StartTurn") && !_playing)
        {
            GetActionsForEachActor();
        }
        if (_awaitingTarget)
        {

        }
    }

    public async void GetActionsForEachActor()
    {
        if (_awaitingActions)
        {
            return;
        }

        foreach (TurnBasedActor turnBasedActor in _heroes)
        {
            turnBasedActor.CallDeferred("SetActionForTurn");
            await ToSignal(turnBasedActor, "ActionReady");
            turnBasedActor.SetAvailableTargets(GetTargets(true));
            turnBasedActor.CallDeferred("SetTargetForTurn");
            await ToSignal(turnBasedActor, "TargetReady");

            turnBasedActor.GetUpcomingAction().RefreshBeats();
        }

        ExecuteActions();
    }

    public void ExecuteActions()
    {
        _playing = true;
    }

    public Godot.Collections.Array<TurnBasedActor> GetTargets(bool targetEnemies)
    {
        if (targetEnemies)
        {
            return _enemies;
        }
        return _heroes;
    }
}
