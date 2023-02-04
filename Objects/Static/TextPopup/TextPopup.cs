using Godot;
using System;

public class TextPopup : Control
{
    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("TextPopup");
    }
    public void SetText(String text)
    {
        GetNode<Label>("Label").Text = text;
        // RectPosition = RectSize / -2;
    }
}
