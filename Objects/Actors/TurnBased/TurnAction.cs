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
    [Export] public String AnimationName;

    private List<float> _beats;
    public List<float> BeatsToPlay = new List<float>();

    private AudioStreamPlayer _audioPlayer;
    private TurnBasedActor _actor;

    public override void _Ready()
    {
        base._Ready();

        _actor = GetParent<TurnBasedActor>();
        _audioPlayer = new AudioStreamPlayer();
        AddChild(_audioPlayer);
        _audioPlayer.Stream = _soundEffect;

        GenerateBeats();
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

    public void RefreshBeats()
    {
        BeatsToPlay = new List<float>(_beats);
    }

    // What happens each time a beat is executed! Animation, sounds etc.
    public void PlayBeat()
    {
        _audioPlayer.Play();
        _actor.PopupText("" + DamagePerHit);
        if (AnimationName != null)
        {
            _actor.GetNode<AnimatedSprite>("AnimatedSprite").Animation = AnimationName;
            _actor.GetNode<AnimatedSprite>("AnimatedSprite").Play();
        }
    }

    public override String ToString()
    {
        return $"*{Name}*";
    }
}
