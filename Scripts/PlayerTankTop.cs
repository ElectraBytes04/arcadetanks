using Godot;
using System;

public partial class PlayerTankTop : Node2D
{
    private Node2D _levelRoot;
    private Node2D _player;
    
    public override void _Ready()
    {
        _levelRoot = (Node2D)GetParent();
        _player = _levelRoot.GetNode<Node2D>("PlayerBody");
        Position = _player.Position;
        CallDeferred("SetInitialBarrelRotation");
    }
    
    private void SetInitialBarrelRotation()
    {
        Vector2 aim = GetGlobalMousePosition() - GlobalPosition;
        Rotation = aim.Angle();
    }
    
    public override void _Process(double delta)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 toMouse = mousePos - GlobalPosition;
        Rotation = toMouse.Angle();
        
        Position = _player.Position;
    }
}
