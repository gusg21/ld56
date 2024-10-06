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
    [Export]
    public float JumpHeight = 300.0f;

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
        
        Vector2 velocity = Velocity;

        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                velocity = P1GetInput(velocity);
                break;
            case PlayerID.TWO:
                velocity = P2GetInput(velocity);
                break;
        }
        
        // Gravity 
        if (!IsOnFloor()) {
            velocity += new Vector2(0, 9.8f);
        }
        
        // Move
        Velocity = velocity;
        
        MoveAndSlide();
    }

    public Vector2 P1GetInput(Vector2 velocity)
    {
        Vector2 newVelocity = velocity;
        // Horizontal movement
        newVelocity.X = Input.GetAxis("move_left_p1", "move_right_p1") * MoveSpeed;

        if (IsOnFloor() && Input.IsActionPressed("jump_p1"))
        {
            newVelocity.Y -= JumpHeight;
        }

        return newVelocity;
    }

    public Vector2 P2GetInput(Vector2 velocity)
    {
        Vector2 newVelocity = velocity;

        // Horizontal movement
        newVelocity.X = Input.GetAxis("move_left_p2", "move_right_p2") * MoveSpeed;
        
        if (IsOnFloor() && Input.IsActionPressed("jump_p2"))
        {
            newVelocity.Y -= JumpHeight;
        }

        return newVelocity;
    }
}
