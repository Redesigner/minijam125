using Godot;
using System;
using System.Collections.Generic;

public class TargetSelector : ColorRect
{
    [Export] private NodePath _cursorPath;
    private Control _cursor;

    private Godot.Collections.Array<TurnBasedActor> _targets;
    private int _selectionIndex;

    [Signal] public delegate void TargetSelected();

    public override void _Ready()
    {
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
            GD.Print("target selected");
            HideCursor();
            EmitSignal("TargetSelected");
        }
    }

    public void SetTargetList(Godot.Collections.Array<TurnBasedActor> targets)
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

    public TurnBasedActor GetSelected()
    {
        return _targets[_selectionIndex];
    }

    private void SetCursorPosition(int listIndex)
    {
        TurnBasedActor target = _targets[listIndex];
        AnimatedSprite sprite = target.GetNode<AnimatedSprite>("AnimatedSprite");

        _cursor.RectGlobalPosition = sprite.GlobalPosition;
        _cursor.RectSize = sprite.Frames.GetFrame("default", 0).GetSize();
    }

    private void HideCursor()
    {
        GD.Print("Hiding target selector");
        Visible = false;
    }
}
