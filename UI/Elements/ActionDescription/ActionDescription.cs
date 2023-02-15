using Godot;
using System;

public class ActionDescription : Control
{
    [Export] private NodePath _titlePath;
    [Export] private NodePath _descriptionPath;

    private Label _title;
    private Label _description;

    private TBAction _associatedAction;

    public override void _Ready()
    {
        _title = GetNode<Label>(_titlePath);
        _description = GetNode<Label>(_descriptionPath);
    }

    public void SetAction(TBAction action)
    {
        _title.Text = action.Name;
        _description.Text = action.Description;
    }
}
