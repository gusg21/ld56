using Godot;
using System;
using HookyandSmoochus.game;

public enum CharacterType
{
    HOOKY, SMOOCHUS
}

public partial class Player : CharacterBody2D
{
    [Export]
    public CharacterType Type;

    [ExportGroup("Movement")]
    [Export]
    public float MoveSpeed = 20.0f;

    private Sprite2D _gfx;

    public override void _Ready()
    {
        base._Ready();
        
        // Assign PlayerID
        switch (Type)
        {
            case CharacterType.HOOKY:
                GameManager.I.ActiveCharacters.Add(CharacterType.HOOKY, PlayerID.ONE);
                break;
            case CharacterType.SMOOCHUS:
                GameManager.I.ActiveCharacters.Add(CharacterType.SMOOCHUS, PlayerID.TWO);
                break;
        }

        // Acquire references
        _gfx = GetNode<Sprite2D>("GFX");

        // Update graphics
        Texture2D texture = null;
        switch (Type)
        {
            case CharacterType.HOOKY:
                texture = GD.Load<Texture2D>("res://player/hooky.png");
                break;
            case CharacterType.SMOOCHUS:
                texture = GD.Load<Texture2D>("res://player/smoochus.png");
                break;
        }
        _gfx.Texture = texture;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                P1DoMovement();
                break;
            case PlayerID.TWO:
                P2DoMovement();
                break;
        }
        
        MoveAndSlide();
    }

    public void P1DoMovement()
    {
        Vector2 velocity = Velocity;

        // Horizontal movement
        velocity.X = Input.GetAxis("move_left_p1", "move_right_p1") * MoveSpeed;

        // Gravity 
        if (!IsOnFloor()) {
            velocity += new Vector2(0, 9.8f);
        }

        // Move
        Velocity = velocity;
    }

    public void P2DoMovement()
    {
        Vector2 velocity = Velocity;

        // Horizontal movement
        velocity.X = Input.GetAxis("move_left_p2", "move_right_p2") * MoveSpeed;

        // Gravity 
        if (!IsOnFloor()) {
            velocity += new Vector2(0, 9.8f);
        }

        // Move
        Velocity = velocity;
    }
}
