using Godot;
using System;

public class PlayerStatus : Control
{
    private TBPlayer _player;

    private Label _healthLabel;
    private Label _name;

    private int _health;
    private int _maxHealth;

    public override void _Ready()
    {
        _healthLabel = GetNode<Label>("HealthLabel");
        _name = GetNode<Label>("NameLabel");    
    }

    public void SetPlayer(TBPlayer player)
    {
        _player = player;
        _name.Text = player.Name;

        _health = _player.GetHealth();
        _maxHealth = _player.GetMaxHealth();
        UpdateHealthLabel();

        _player.Connect("HealthChanged", this, "OnHealthChanged");
    }

    private void UpdateHealthLabel()
    {
        _healthLabel.Text = $"{_health}/{_maxHealth}";
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        _health = newHealth;
        UpdateHealthLabel();
    }
}
