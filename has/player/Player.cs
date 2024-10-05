using Godot;
using System;

public enum PlayerType
{
    HOOKY, SMOOCHUS
}

public partial class Player : CharacterBody2D
{
    [Export]
    public PlayerType Type;

    [ExportGroup("Movement")]
    [Export]
    public float MoveSpeed = 4.0f;

    private Sprite2D _gfx;

    public override void _Ready()
    {
        base._Ready();

        // Acquire references
        _gfx = GetNode<Sprite2D>("GFX");

        // Update graphics
        Texture2D texture = null;
        switch (Type)
        {
            case PlayerType.HOOKY:
                texture = GD.Load<Texture2D>("res://player/hooky.png");
                break;
            case PlayerType.SMOOCHUS:
                texture = GD.Load<Texture2D>("res://player/smoochus.png");
                break;
        }
        _gfx.Texture = texture;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector2 velocity = Velocity;

        // Horizontal movement
        velocity.X = Input.GetAxis("move_left", "move_right") * MoveSpeed;

        // Gravity 
        if (!IsOnFloor()) {
            velocity += new Vector2(0, 9.8f);
        }

        // Move
        Velocity = velocity;
        MoveAndSlide();
    }
}
