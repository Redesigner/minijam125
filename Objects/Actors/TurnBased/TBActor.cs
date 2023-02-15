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
    protected Godot.Collections.Array<TBActor> _availableTargets = new Godot.Collections.Array<TBActor>();

    private AnimatedSprite _sprite;

    private int _incomingDamage = 0;
    private int _incomingHitsCount = 0;

    [Export] public bool IsEnemy = false;
    [Export] private List<PackedScene> _defaultActions = new List<PackedScene>();


    [Signal] public delegate void ActionReady();
    [Signal] public delegate void TargetReady();

    public override void _Ready()
    {
        base._Ready();

        _textPopupScene = GD.Load<PackedScene>(_textPopupResourcePath);
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");

        PopulateActionsFromArray();
        PopulateActionsFromChildNodes();
        _pendingAction = _availableActions[0];
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

    public virtual void ChooseAction()
    {
        EmitSignal("ActionReady");
    }

    public virtual void ChooseTarget(Godot.Collections.Array<TBActor> targets)
    {
        _availableTargets = targets;
        EmitSignal("TargetReady");
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

    public void SetUpcomingAction(TBAction action)
    {
        _pendingAction = action;
    }

    public void SetUpcomingTarget(TBActor target)
    {
        _pendingTarget = target;
    }

    public TBAction GetUpcomingAction()
    {
        return _pendingAction;
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
            case 2: return 4;
            case 3: return 8;
            case 4: return 16;
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
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        if (_health <= damage)
        {
            _health = 0;
            return false;
        }
        else
        {
            _health -= damage;
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
