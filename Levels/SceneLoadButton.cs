using Godot;
using System;

public class SceneLoadButton : Button
{
    [Export] private String _nextScene;

    public override void _Pressed()
    {
        base._Pressed();

        GetTree().ChangeScene(_nextScene);
    }
}
