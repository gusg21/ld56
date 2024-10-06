using Godot;
using System;
using HookyandSmoochus.game;

public enum CharacterType
{
    HOOKY, SMOOCHUS
}

public partial class Player : CharacterBody2D
{
    private static Player Hooky;
    private static Player Smoochus;

    [Export]
    public CharacterType Type;

    [ExportGroup("Movement")]
    [Export]
    public float MoveSpeed = 100.0f;
    [Export]
    public float AccelSpeed = 50.0f;
    [Export]
    public float JumpHeight = 350.0f;

    private Sprite2D _gfx;
    private bool _isHooked;
    private float _ropeLength;
    private bool _isStuck;
    private float _angularVelocity;

    public override void _Ready()
    {
        base._Ready();

        // Assign PlayerID
        switch (Type)
        {
            case CharacterType.HOOKY:
                GameManager.I.ActiveCharacters.Add(CharacterType.HOOKY, PlayerID.ONE);
                Hooky = this;
                break;
            case CharacterType.SMOOCHUS:
                GameManager.I.ActiveCharacters.Add(CharacterType.SMOOCHUS, PlayerID.TWO);
                Smoochus = this;
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

        PerPlayerInputs();

        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                if (_isHooked)
                    velocity = Swing(velocity);
                break;
            case PlayerID.TWO:
                break;
        }

        if (!_isHooked && !_isStuck)
        {
            velocity = DoMovement(velocity);
        }

        // Gravity 
        if (!IsOnFloor())
        {
            velocity += new Vector2(0, 13f);
        }

        if (_isStuck)
        {
            velocity = Vector2.Zero;
        }

        // Move
        Velocity = velocity;

        MoveAndSlide();

        if (IsOnCeiling())
        {
            Velocity = new Vector2(Velocity.X, 0);
        }
    }

    private void PerPlayerInputs()
    {
        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                {
                    if (Input.IsActionJustPressed("act_p1"))
                    {
                        _isHooked = true;
                        _angularVelocity = 0;
                        _ropeLength = GlobalPosition.DistanceTo(Smoochus.GlobalPosition);
                    }
                    else if (Input.IsActionJustReleased("act_p1"))
                    {
                        _isHooked = false;
                    }
                    break;
                }

            case PlayerID.TWO:
                {
                    if (Input.IsActionJustPressed("act_p2"))
                    {
                        _isStuck = true;
                    }
                    else if (Input.IsActionJustReleased("act_p2"))
                    {
                        _isStuck = false;
                    }
                    break;
                }
        }



    }

    private float GetHorizInput()
    {
        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                return Input.GetAxis("move_left_p1", "move_right_p1");
            case PlayerID.TWO:
                return Input.GetAxis("move_left_p2", "move_right_p2");
        }

        return 0f;

    }

    private bool WasJumpPressed()
    {
        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                return Input.IsActionPressed("jump_p1");
            case PlayerID.TWO:
                return Input.IsActionPressed("jump_p2");
        }

        return false;
    }

    public Vector2 DoMovement(Vector2 velocity)
    {
        Vector2 newVelocity = velocity;

        // Horizontal movement
        Vector2 moveAccel = new Vector2();
        float hInput = GetHorizInput();
        moveAccel.X = hInput * AccelSpeed;
        newVelocity += moveAccel;

        newVelocity.X = Mathf.Clamp(newVelocity.X, -MoveSpeed, MoveSpeed);

        if (hInput < float.Epsilon && IsOnFloor() && !_isHooked)
        {
            // Friction
            newVelocity.X *= 0.9f;
        }

        if (IsOnFloor() && WasJumpPressed())
        {
            newVelocity.Y -= JumpHeight;
        }

        return newVelocity;
    }

    public Vector2 Swing(Vector2 velocity)
    {

        if (_isHooked)
        {
            var param = PhysicsRayQueryParameters2D.Create(GlobalPosition, Smoochus.GlobalPosition, uint.MaxValue, new Godot.Collections.Array<Rid>() { Smoochus.GetRid(), Hooky.GetRid() });
            var results = GetWorld2D().DirectSpaceState.IntersectRay(
                param
            );
            if (results.Count > 0)
            {
                GD.Print(results["position"]);
            }

            velocity = Vector2.Zero;

            Vector2 toHook = Smoochus.GlobalPosition - GlobalPosition;
            Vector2 radius = GlobalPosition - Smoochus.GlobalPosition;

            _angularVelocity += toHook.X * 0.3f;

            velocity += -toHook.Orthogonal().Normalized() * _angularVelocity;

            if (toHook.Length() < 5.0f)
            {
                _isHooked = false;
            }

            // velocity += velocity.Normalized() * toHook.Orthogonal();

            GlobalPosition = Smoochus.GlobalPosition + radius.Normalized() * _ropeLength;

            // velocity += toHook.Normalized() * 4;
        }

        return velocity;
    }

    public void P2Act()
    {

    }
}
