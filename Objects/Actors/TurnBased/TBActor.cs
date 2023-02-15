using Godot;
using System;
using System.Collections.Generic;

public class TBActor : Node2D
{
    private const String _textPopupResourcePath = "res://Objects/Static/TextPopup/TextPopup.tscn";
    private PackedScene _textPopupScene;
    private TBAction _pendingAction;
    private TBActor _pendingTarget;

    [Export] private int _maxHealth = 100;
    private int _health = 100;

    protected List<TBAction> _availableActions = new List<TBAction>();

    private AnimatedSprite _sprite;

    private int _incomingDamage = 0;
    private int _incomingHitsCount = 0;

    [Export] public bool IsEnemy = false;
    [Export] private List<PackedScene> _defaultActions = new List<PackedScene>();


    [Signal] public delegate void ActionReady();
    [Signal] public delegate void HealthChanged(int oldHealth, int newHealth);

    public override void _Ready()
    {
        base._Ready();

        _textPopupScene = GD.Load<PackedScene>(_textPopupResourcePath);
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");

        PopulateActionsFromArray();
        PopulateActionsFromChildNodes();
    }

    public void ClearPreviousTurn()
    {
        _pendingAction = null;
        _pendingTarget = null;
    }

    private void PopulateActionsFromArray()
    {
        foreach (PackedScene scene in _defaultActions)
        {
            Node instance = scene.Instance();
            if (!(instance is TBAction))
            {
                instance.QueueFree();
                GD.PrintErr($"Action '{instance.Name}' assigned to '{Name}' is not of type 'TurnAction'");
                continue;
            }
            AddChild(instance);
        }
    }

    private void PopulateActionsFromChildNodes()
    {
        foreach (Node node in GetChildren())
        {
            if (node is TBAction)
            {
                TBAction action = (TBAction)node;
                _availableActions.Add(action);
            }
        }
    }

    public void SetPendingAction(TBAction action)
    {
        _pendingAction = action;
        EmitSignal("ActionReady");
    }

    public TBAction GetPendingAction()
    {
        return _pendingAction;
    }

    public void SetPendingTarget(TBActor target)
    {
        _pendingTarget = target;
        EmitSignal("ActionReady");
    }

    public TBActor GetPendingTarget()
    {
        return _pendingTarget;
    }

    public void SetIdle()
    {
        if (_sprite.Animation == "default")
        {
            return;
        }
        _sprite.Animation = "default";
        _sprite.Play();
    }

    public bool IsReady()
    {
        if (_pendingAction == null)
        {
            GD.Print($"Actor {Name} is not ready -- they don't have an action chosen.");
            return false;
        }
        if (_pendingAction.RequiresTarget() && _pendingTarget == null)
        {
            GD.Print($"Actor {Name} is not ready -- their current action requires a target, but they haven't chosen one.");
            return false;
        }
        return true;
    }

    public bool TakeAllIncomingDamage()
    {
        if (_incomingDamage == 0)
        {
            return IsAlive();
        }
        int multiplier = GetMultiplier(_incomingHitsCount);
        int damageThisTurn = _incomingDamage * multiplier;
        _incomingDamage = 0;
        _incomingHitsCount = 0;
        PopupText(GenerateDamageText(multiplier, damageThisTurn));
        return TakeDamage(damageThisTurn);
    }

    public void AccumulateDamage(int damage)
    {
        _incomingDamage += damage;
        _incomingHitsCount++;
    }

    private static String GenerateDamageText(int multiplier, int damage)
    {
        String result = "" + damage;
        if (multiplier > 1)
        {
            result += " x";
            result += multiplier;
        }
        return result;
    }

    private static int GetMultiplier(int numHits)
    {
        switch(numHits)
        {
            default: return 0;
            case 1: return 1;
            case 2: return 2;
            case 3: return 4;
            case 4: return 8;
        }
    }

    public void PopupText(String text)
    {
        TextPopup popup = (TextPopup) _textPopupScene.Instance();
        popup.SetText(text);
        GetTree().Root.AddChild(popup);
        Vector2 spriteSize = _sprite.Frames.GetFrame("default", 0).GetSize();
        Vector2 textSize = popup.GetNode<Label>("Label").RectSize;
        popup.RectGlobalPosition = _sprite.GlobalPosition -  new Vector2(textSize.x / 2.0f, textSize.y + spriteSize.y / 2.0f);
    }

    public List<TBAction> GetActions()
    {
        return _availableActions;
    }

    public bool IsAlive()
    {
        return _health > 0;
    }

    /// returns true if the actor is still alive after taking damage
    private bool TakeDamage(int damage)
    {
        int oldHealth = _health;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        if (_health <= damage)
        {
            _health = 0;
            EmitSignal("HealthChanged", oldHealth, 0);
            return false;
        }
        else
        {
            _health -= damage;
            EmitSignal("HealthChanged", oldHealth, _health);
            return true;
        }
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public int GetHealth()
    {
        return _health;
    }
}
