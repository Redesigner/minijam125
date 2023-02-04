using Godot;
using System;

public class TurnQueue : Node2D
{
    [Export] public int BPM = 120;

    private bool _awaitingActions = false;
    private bool _playing = false;

    private float _currentTime = 0.0f;

    private float _bpmTimeRatio = 1.0f;

    private Godot.Collections.Array<TurnAction> _tracks = new Godot.Collections.Array<TurnAction>();
    private Godot.Collections.Array<TurnAction> _enemyTracks = new Godot.Collections.Array<TurnAction>();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bpmTimeRatio = (float) BPM / 60.0f;
        GD.Print("Press 'Space' to begin your turn.");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_playing)
        {
            _currentTime += delta * _bpmTimeRatio;
            bool didEnemyBlock = false;
            Godot.Collections.Array<TurnAction> actionsThisBeat = new Godot.Collections.Array<TurnAction>();
            foreach (TurnAction track in _tracks)
            {
                if (track.BeatsToPlay.Count > 0 && track.BeatsToPlay[0] <= _currentTime)
                {
                    actionsThisBeat.Add(track);
                    track.BeatsToPlay.RemoveAt(0);
                }
            }
            foreach (TurnAction track in _enemyTracks)
            {
                if (track.BeatsToPlay.Count > 0 && track.BeatsToPlay[0] <= _currentTime)
                {
                    if (!didEnemyBlock)
                    {
                        actionsThisBeat.Clear();
                        didEnemyBlock = true;
                    }
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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("StartTurn") && !_playing)
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
        /* foreach (Node actor in GetChildren())
        {
            TurnBasedActor turnBasedActor = (TurnBasedActor)actor;
            TurnAction action = turnBasedActor.GetUpcomingAction();
            GD.Print($"{turnBasedActor.Name} used action: {action.ActionName}");
        }
        GD.Print("Turn end.\n"); */
        AccumulateTracks();
        _playing = true;
    }

    private void AccumulateTracks()
    {
        _tracks.Clear();
        foreach (Node actor in GetChildren())
        {
            TurnBasedActor turnBasedActor = (TurnBasedActor) actor;
            TurnAction action = turnBasedActor.GetUpcomingAction();
            if (turnBasedActor.IsEnemy)
            {
                _enemyTracks.Add(action);
            }
            else
            {
                _tracks.Add(action);
            }
            action.RefreshBeats();
        }
    }
}
