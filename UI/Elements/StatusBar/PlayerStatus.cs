using Godot;
using System;

public class PlayerStatus : Control
{
    private TBPlayer _player;

    private Label _health;
    private Label _name;

    public override void _Ready()
    {
        _health = GetNode<Label>("HealthLabel");
        _name = GetNode<Label>("NameLabel");    
    }

    public void SetPlayer(TBPlayer player)
    {
        _player = player;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (_player == null)
        {
            QueueFree();
            return;
        }
        _health.Text = $"{_player.GetHealth()}/{_player.GetMaxHealth()}";
        _name.Text = _player.Name;
    }
}
