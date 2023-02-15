using Godot;
using System;

public class StatusBar : Panel
{
    [Export] private NodePath _containerNodePath;
    private Control _container;
    private Godot.Collections.Array<PlayerStatus> _playerStatuses = new Godot.Collections.Array<PlayerStatus>();

    [Export] private PackedScene _playerStatusUI;
    public override void _Ready()
    {
        _container = GetNode<Control>(_containerNodePath);
    }

    public void SetPlayers(Godot.Collections.Array<TBActor> players)
    {
        ClearEntries();
        int currentOffsetY = 0;
        foreach (TBPlayer player in players)
        {
            PlayerStatus status = (PlayerStatus) _playerStatusUI.Instance();
            GD.Print(_container.Name);
            _container.AddChild(status);
            _playerStatuses.Add(status);
            status.SetPlayer(player);
            status.MarginTop = currentOffsetY;
            currentOffsetY += (int) status.RectSize.y;
        }
    }

    private void ClearEntries()
    {
        foreach(PlayerStatus status in _playerStatuses)
        {
            status.QueueFree();
        }
        _playerStatuses.Clear();
    }
}
