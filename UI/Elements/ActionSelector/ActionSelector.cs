using Godot;
using System;
using System.Collections.Generic;

public class ActionSelector : Panel
{
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

    private TBPlayer _player;

    [Signal] private delegate void ActionSelected(TBPlayer playerSelectedFor);

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
        if (@event.IsActionPressed("ui_select") && Visible)
        {
            ConfirmAction();
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

    public void Focus()
    {
        _listView.FocusMode = FocusModeEnum.All;
        _listView.GrabFocus();
    }

    public void Unfocus()
    {
        _listView.FocusMode = FocusModeEnum.None;
        _listView.ReleaseFocus();
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

    private void SetActionList(List<TBAction> actionList)
    {
        _actionList = actionList;
        PopulateTree();
    }

    public void ClearSelection()
    {
        if (_listView.GetSelected() != null)
        {
            _listView.GetSelected().Deselect(0);
        }
        _popup.Visible = false;
    }

    public void SetPlayer(TBPlayer player)
    {
        _player = player;
        SetActionList(player.GetActions());
        _actorNameLabel.Text = player.Name;
    }

    private void ConfirmAction()
    {
        if (_selectedAction == null)
        {
            return;
        }
        _confirmSoundPlayer.Play();
        _player.SetPendingAction(_selectedAction);
        EmitSignal("ActionSelected", _player);
    }
}
