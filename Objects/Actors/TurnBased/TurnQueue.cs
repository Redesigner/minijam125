using Godot;
using System;
using System.Numerics;

public class TurnQueue : Node2D
{
    [Export] public int BPM = 120;

    private bool _awaitingActions = false;
    private bool _playing = false;
    private bool _awaitingTarget = false;

    private float _currentTime = 0.0f;

    private float _bpmTimeRatio = 1.0f;

    private Godot.Collections.Array<TBActor> _actors = new Godot.Collections.Array<TBActor>();

    private Godot.Collections.Array<TBActor> _heroes = new Godot.Collections.Array<TBActor>();
    private Godot.Collections.Array<TBActor> _enemies = new Godot.Collections.Array<TBActor>();

    [Export] PackedScene _victoryScreen;

    [Signal] private delegate void TurnStarted();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bpmTimeRatio = (float) BPM / 60.0f;

        foreach (Node child in GetChildren())
        {
            if (child is TBActor)
            {
                TBActor actor = (TBActor) child;
                _actors.Add(actor);
                actor.Connect("ActionReady", this, "OnActionReadied");

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
        PrepareTurn();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_playing)
        {
            TickTracks(delta); 

            if (_currentTime >= 4.0f)
            {
                EndTurn();
            }
        }
    }

    public void OnActionReadied()
    {
        GD.Print("ActionReadied");
        if (IsTurnReady() && !_playing)
        {
            GenerateTracks();
        }
    }

    private bool IsTurnReady()
    {
        foreach (TBActor actor in _heroes)
        {
            if (!actor.IsReady())
            {
                return false;
            }
        }
        GD.Print("Turn ready!!!");
        return true;
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
        PrepareTurn();
    }

    private bool DidBeatOccur(TBAction track)
    {
        if (track.BeatsToPlay.Count <= 0)
        {
            return false;
        }
        return track.BeatsToPlay[0] <= _currentTime;
    }

    public Godot.Collections.Array<TBActor> GetTeam(bool enemyTeam)
    {
        if (enemyTeam)
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

    private void TickTracks(float deltaTime)
    {
        _currentTime += deltaTime * _bpmTimeRatio;
        Godot.Collections.Array<TBAction> actionsThisBeat = new Godot.Collections.Array<TBAction>();

        foreach (TBActor actor in _enemies)
        {
            TBAction track = actor.GetPendingAction();
            if (DidBeatOccur(track))
            {
                actionsThisBeat.Add(track);
                track.BeatsToPlay.RemoveAt(0);
            }
        }

        foreach (TBActor actor in _heroes)
        {
            TBAction track = actor.GetPendingAction();
            if (DidBeatOccur(track))
            {
                actionsThisBeat.Add(track);
                track.BeatsToPlay.RemoveAt(0);
            }
        }

        foreach (TBAction action in actionsThisBeat)
        {
            action.PlayBeat();
        }

        TeamTakeDamage(_heroes);
        TeamTakeDamage(_enemies);
    }

    private void PrepareTurn()
    {
        GD.Print($"{_actors.Count} actors at end of turn.");

        DeleteDeadActors(_actors);
        DeleteDeadActors(_heroes);
        DeleteDeadActors(_enemies);

        GD.Print($"{_actors.Count} actors remaining after purge");
        foreach (TBActor actor in _actors)
        {
            actor.ClearPreviousTurn();
        }

        foreach (TBEnemy enemy in _enemies)
        {
            enemy.ChooseAction();
            if (!enemy.GetPendingAction().RequiresTarget())
            {
                continue;
            }
            enemy.ChooseTarget(GetTeam(enemy.GetPendingAction().TargetAllies()));
        }
        EmitSignal("TurnStarted");
    }

    private void GenerateTracks()
    {
        foreach (TBActor actor in _actors)
        {
            actor.GetPendingAction().RefreshBeats();
        }
        _playing = true;
    }

    private void DeleteDeadActors(Godot.Collections.Array<TBActor> team)
    {
        Godot.Collections.Array<int> deletionQueue = new Godot.Collections.Array<int>();
        for (int i = 0; i < team.Count; i++)
        {
            if (team[i] == null)
            {
                deletionQueue.Add(i);
            }
        }

        foreach (int deadActor in deletionQueue)
        {
            team.RemoveAt(deadActor);
        }
    }
}
