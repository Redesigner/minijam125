using Godot;
using System;
using System.Collections.Generic;

// A class containing the data for each character's behavior on any given turn.
public class TurnAction : Node
{
    // [Export] public String ActionName;
    [Export] public int BeatsPerMeasure;
    [Export] private AudioStream _soundEffect;
    [Export] private String _beatPattern = "";
    [Export] public int DamagePerHit = 1;

    private List<float> _beats;
    public List<float> BeatsToPlay;

    private AudioStreamPlayer _audioPlayer;
    /* public TurnAction(String actionName)
    {
        ActionName = actionName;
        BeatsPerMeasure = 4;
        _beats = new List<float> {0, 1, 2, 3};
    }

    public TurnAction(String actionName, int beatsPerMeasure)
    {
        ActionName = actionName;
        BeatsPerMeasure = beatsPerMeasure;
        GenerateBeats();
    } */

    public override void _Ready()
    {
        base._Ready();
        GenerateBeats();
        _audioPlayer = new AudioStreamPlayer();
        AddChild(_audioPlayer);
        _audioPlayer.Stream = _soundEffect;
    }

    private void GenerateBeats()
    {
        _beats = new List<float>();
        if (BeatsPerMeasure == 0)
        {
            return;
        }
        float noteLength = 4.0f / (float)BeatsPerMeasure;

        int beatIndex = 0;
        for (float i = 0; i < 4.0f; i += noteLength)
        {
            if (_beatPattern.Length - 1 >= beatIndex && _beatPattern[beatIndex] == 'x')
            {
                _beats.Add(i);
            }
            beatIndex++;
        }
    }

    private void ParseBeatString()
    {

    }

    public void RefreshBeats()
    {
        BeatsToPlay = new List<float>(_beats);
    }

    // What happens each time a beat is executed! Animation, sounds etc.
    public void PlayBeat()
    {
        // GD.Print($"*{Name}*");
        _audioPlayer.Play();
    }

    public override String ToString()
    {
        return $"*{Name}*";
    }
}
