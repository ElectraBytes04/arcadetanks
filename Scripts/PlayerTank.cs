using System;
using Godot;

public partial class PlayerTank : Node2D 
{
    private Vector2 _velocity = Vector2.Zero;
    
    private Area2D _area;
    private CollisionShape2D _collider;
        
    [Export] public float moveSpeed { get; set; } = 120f;
    [Export] public float moveEase { get; set; } = 5f;
    [Export] public float rotationSpeed { get; set; } = 20f;
    
    [Export] public bool strafe { get; set; } = true;
    
    public override void _Ready()
    {
        if (!strafe)
        {
            rotationSpeed = 3f;
        }
        
        _area = GetNode<Area2D>("Area");
        _collider = _area.GetNode<CollisionShape2D>("AreaCollider");
        
        GD.Print(strafe);
    }
    
    public override void _Process(double delta)
    {
        if (strafe)
        {
            NormalMovement(delta);
        }
        else
        {
            TankMovement(delta);
        }
    }
    
    private void NormalMovement(double delta)
    {
        Vector2 input = new Vector2(
            Input.GetActionStrength("right") - Input.GetActionStrength("left"),
            Input.GetActionStrength("down") - Input.GetActionStrength("up")
        ).Normalized();

        Vector2 targetVelocity = input * moveSpeed;
        
        _velocity = _velocity.Lerp(targetVelocity, (float)(delta * moveEase));
        
        if (_velocity.Length() > 1)
        {
            float targetAngle = _velocity.Angle();
            Rotation = Mathf.LerpAngle(
                Rotation, targetAngle, (float)(delta * rotationSpeed)
            );
        }
        
        CollisionTest(delta);
    }
    
    private void TankMovement(double delta)
    {
        float inputY = (
            Input.GetActionStrength("up") - Input.GetActionStrength("down")
        );
        
        float inputX = (
            Input.GetActionStrength("right") - Input.GetActionStrength("left")
        );
        
        Rotation += inputX * rotationSpeed * (float)delta;
        Vector2 facing = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
        
        Vector2 targetVelocity = facing * inputY * moveSpeed;
        
        _velocity = _velocity.Lerp(targetVelocity, (float)(delta * moveEase));
        
        CollisionTest(delta);
    }
    
    private void CollisionTest(double  delta)
    {
        Vector2 proposedMove = Position + _velocity * (float)delta;

        var spaceState = GetWorld2D().DirectSpaceState;

        var query = new PhysicsShapeQueryParameters2D
        {
            Shape = _collider.Shape,
            Transform = new Transform2D(0, proposedMove),
            Margin = 0.01f,
            CollideWithAreas = true,
            CollideWithBodies = true,
            Exclude = new Godot.Collections.Array<Rid> { _area.GetRid() },
        };

        var collisions = spaceState.IntersectShape(query, 1);

        if (collisions.Count == 0)
        {
            Position = proposedMove;
        }
        else
        {
            var collider = (Node2D)collisions[0]["collider"];
            
            GD.Print("Collision with: ", collider);
            
            if (collider.IsInGroup("levelBoundaries"))
            {
                GD.Print("Hit a level boundary!");
            }
            
            _velocity = Vector2.Zero;
        }
    }
}
