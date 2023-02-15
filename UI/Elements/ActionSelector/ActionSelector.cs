using Godot;
using System;
using System.Collections.Generic;

public class ActionSelector : Panel
{
    [Signal] delegate void ActionReady();
    private Tree _listView;
    private TreeItem _root;
    private ActionDescription _popup;
    private bool _awaitingInput = false;

    private List<TBAction> _actionList = new List<TBAction>();
    private Label _actorNameLabel;
    private TBAction _selectedAction;

    [Export] private AudioStream _cursorSound;
    [Export] private AudioStream _confirmSound;
    private AudioStreamPlayer _audioStreamPlayer;
    private AudioStreamPlayer _confirmSoundPlayer;

    public override void _Ready()
    {
        _audioStreamPlayer = new AudioStreamPlayer();
        AddChild(_audioStreamPlayer);

        _confirmSoundPlayer = new AudioStreamPlayer();
        AddChild(_confirmSoundPlayer);

        _audioStreamPlayer.Stream = _cursorSound;
        _confirmSoundPlayer.Stream = _confirmSound;

        _actorNameLabel = GetNode<Label>("Panel/Label");

        _listView = GetNode<Tree>("Tree");
        _popup = GetNode<ActionDescription>("Popup");
        _root = _listView.CreateItem();

        _popup.Visible = false;
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_awaitingInput)
        {
            return;
        }
        if (@event.IsActionPressed("ui_select"))
        {
            if (_selectedAction == null)
            {
                return;
            }
            GD.Print("Action selected");
            _confirmSoundPlayer.Play();
            EmitSignal("ActionReady");
        }
    }

    private void PopulateTree()
    {
        _awaitingInput = true;
        _listView.Clear();
        _root = _listView.CreateItem();
        _selectedAction = null;
        foreach (TBAction entry in _actionList)
        {
            TreeItem treeItem = _listView.CreateItem(_root);
            treeItem.SetText(0, entry.Name);
        }
    }
    public void _onTreeItemSelected()
    {
        TreeItem selectedItem = _listView.GetSelected();
        _selectedAction = GetTurnActionByName(selectedItem.GetText(0));
        _popup.SetAction(_selectedAction);
        _popup.Visible = true;
        _audioStreamPlayer.Play();
    }

    public void _onTreeNothingSelected()
    {
        _popup.Visible = false;
    }

    private TBAction GetTurnActionByName(String name)
    {
        foreach (TBAction action in _actionList)
        {
            if (action.Name == name)
            {
                return action;
            }
        }
        return null;
    }

    public void SetActionList(List<TBAction> actionList)
    {
        _actionList = actionList;
        PopulateTree();
    }

    public TBAction GetSelectedAction()
    {
        _awaitingInput = false;
        return _selectedAction;
    }

    public void SetFocus(bool focused)
    {
        if (focused)
        {
            _listView.GrabFocus();
        }
        else
        {
            _listView.ReleaseFocus();
        }
    }

    public void ClearSelection()
    {
        if (_listView.GetSelected() != null)
        {
            _listView.GetSelected().Deselect(0);
        }
    }

    public void SetActorName(string actorName)
    {
        _actorNameLabel.Text = actorName;
    }
}
