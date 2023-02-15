using Godot;
using System;
using System.Collections.Generic;

public class TargetSelector : Panel
{
    [Export] private NodePath _cursorPath;
    private Control _cursor;

    [Export] private AudioStream _cursorSound;
    [Export] private AudioStream _confirmSound;
    private AudioStreamPlayer _audioStreamPlayer;
    private AudioStreamPlayer _confirmSoundPlayer;

    private Godot.Collections.Array<TBActor> _targets;
    private int _selectionIndex;

    [Signal] public delegate void TargetSelected();

    public override void _Ready()
    {
        _audioStreamPlayer = new AudioStreamPlayer();
        AddChild(_audioStreamPlayer);

        _confirmSoundPlayer = new AudioStreamPlayer();
        AddChild(_confirmSoundPlayer);

        _audioStreamPlayer.Stream = _cursorSound;
        _confirmSoundPlayer.Stream = _confirmSound;

        _cursor = GetNode<Control>(_cursorPath);
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible)
        {
            return;
        }
        if (@event.IsActionPressed("ui_right"))
        {
            if (_selectionIndex + 1 >= _targets.Count)
            {
                _selectionIndex = 0;
            }
            else
            {
                _selectionIndex++;
            }
            SetCursorPosition(_selectionIndex);
        }
        else if (@event.IsActionPressed("ui_left"))
        {
            if (_selectionIndex <= 0)
            {
                _selectionIndex = _targets.Count - 1;
            }
            else
            {
                _selectionIndex--;
            }
            SetCursorPosition(_selectionIndex);
        }
        else if (@event.IsActionPressed("ui_select"))
        {
            _confirmSoundPlayer.Play();
            HideCursor();
            EmitSignal("TargetSelected");
        }
    }

    public void SetTargetList(Godot.Collections.Array<TBActor> targets)
    {
        _targets = targets;
        if (targets.Count > 0)
        {
            _selectionIndex = 0;
            SetCursorPosition(0);
            GD.Print("Displaying target selector");
            Visible = true;
        }
    }

    public TBActor GetSelected()
    {
        return _targets[_selectionIndex];
    }

    private void SetCursorPosition(int listIndex)
    {
        _audioStreamPlayer.Play();
        TBActor target = _targets[listIndex];
        AnimatedSprite sprite = target.GetNode<AnimatedSprite>("AnimatedSprite");

        _cursor.RectSize = sprite.Frames.GetFrame("default", 0).GetSize();
        _cursor.RectGlobalPosition = sprite.GlobalPosition - _cursor.RectSize / 2.0f;
    }

    private void HideCursor()
    {
        GD.Print("Hiding target selector");
        Visible = false;
    }
}
