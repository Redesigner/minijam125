using Godot;
using System;
using System.Collections.Generic;

public class ActionSelector : Panel
{
    [Signal] delegate void ActionReady();
    private Tree _listView;
    private TreeItem _root;
    private ActionDescription _popup;

    private List<TurnAction> _actionList = new List<TurnAction>();
    private TurnAction _selectedAction;
    public override void _Ready()
    {
        _listView = GetNode<Tree>("Tree");
        _popup = GetNode<ActionDescription>("Popup");
        _root = _listView.CreateItem();

        _popup.Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("ui_select"))
        {
            EmitSignal("ActionReady");
        }
    }

    private void PopulateTree()
    {
        _listView.Clear();
        _root = _listView.CreateItem();
        foreach (TurnAction entry in _actionList)
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
    }

    public void _onTreeNothingSelected()
    {
        _popup.Visible = false;
    }

    private TurnAction GetTurnActionByName(String name)
    {
        foreach (TurnAction action in _actionList)
        {
            if (action.Name == name)
            {
                return action;
            }
        }
        return null;
    }

    public void SetActionList(List<TurnAction> actionList)
    {
        _actionList = actionList;
        PopulateTree();
    }

    public TurnAction GetSelectedAction()
    {
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
        _listView.GetSelected().Deselect(0);
    }
}
