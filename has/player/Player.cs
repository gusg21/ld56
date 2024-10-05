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

        Velocity += new Vector2(0, 9.8f);

        MoveAndSlide();

        // if (IsOnFloor()) {
        //     GD.Print("FUICK");
        // }
    }
}
