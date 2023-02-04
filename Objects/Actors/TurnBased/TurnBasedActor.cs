using Godot;
using System;
using System.Collections.Generic;

public class TurnBasedActor : Node2D
{
    private const String _textPopupResourcePath = "res://Objects/Static/TextPopup/TextPopup.tscn";
    private PackedScene _textPopupScene;
    private TurnAction _pendingAction;
    private TurnBasedActor _pendingTarget;


    protected List<TurnAction> _availableActions = new List<TurnAction>();
    protected Godot.Collections.Array<TurnBasedActor> _availableTargets = new Godot.Collections.Array<TurnBasedActor>();

    private AnimatedSprite _sprite;

    private int _incomingDamage = 0;
    private int _incomingHits = 0;

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
            if (!(instance is TurnAction))
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
            if (node is TurnAction)
            {
                TurnAction action = (TurnAction)node;
                _availableActions.Add(action);
            }
        }
    }

    public virtual void SetActionForTurn()
    {
        EmitSignal("ActionReady");
    }

    public virtual void SetTargetForTurn()
    {
        EmitSignal("TargetReady");
    }

    public TurnBasedActor GetPendingTarget()
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

    public void SetUpcomingAction(TurnAction action)
    {
        _pendingAction = action;
    }

    protected void SetUpcomingTarget(TurnBasedActor target)
    {
        _pendingTarget = target;
    }

    public TurnAction GetUpcomingAction()
    {
        return _pendingAction;
    }
    
    public void SetAvailableTargets(Godot.Collections.Array<TurnBasedActor> targets)
    {
        _availableTargets = targets;
    }

    public void TakeAllIncomingDamage()
    {
        if (_incomingDamage == 0)
        {
            return;
        }
        String damageText = "" + _incomingDamage;

        if (_incomingHits > 1)
        {
            int multiplier = (int)Math.Pow(2, _incomingHits - 1);
            damageText += (" x" + multiplier);
        }
        _incomingDamage = 0;
        _incomingHits = 0;

        PopupText(damageText);
    }
    public void AccumulateDamage(int damage)
    {
        _incomingDamage += damage;
        _incomingHits++;
    }

    public void PopupText(String text)
    {
        TextPopup popup = (TextPopup) _textPopupScene.Instance();
        popup.SetText(text);
        AddChild(popup);
        Vector2 spriteSize = _sprite.Frames.GetFrame("default", 0).GetSize();
        Vector2 textSize = popup.GetNode<Label>("Label").RectSize;
        popup.RectPosition = new Vector2(-textSize.x / 2.0f, -textSize.y - spriteSize.y / 2.0f);
    }

    public List<TurnAction> GetActions()
    {
        return _availableActions;
    }
}
