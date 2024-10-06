using Godot;
using System;
using HookyandSmoochus.game;

public enum CharacterType
{
    HOOKY, SMOOCHUS
}

public partial class Player : CharacterBody2D
{
    public static Color HookyColor = new("de2866");
    public static Color SmoochusColor = new("28dea1");

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
    private CpuParticles2D _parts;
    private Line2D _rope;

    private bool _isHooked;
    private float _ropeLength;
    private bool _isStuck;
    private float _angularVelocity;
    private bool _canAct = false;
    private float _timeSinceGround = 0f;

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
        _parts = GetNode<CpuParticles2D>("Parts");
        _rope = GetNode<Line2D>("Rope");
        _rope.AddPoint(Vector2.Zero);
        _rope.AddPoint(Vector2.Zero);

        // Update graphics
        Texture2D texture = null;
        Color color = new();
        switch (Type)
        {
            case CharacterType.HOOKY:
                texture = GD.Load<Texture2D>("res://player/hooky.png");
                color = HookyColor;
                break;
            case CharacterType.SMOOCHUS:
                texture = GD.Load<Texture2D>("res://player/smoochus.png");
                color = SmoochusColor;
                break;
        }
        _gfx.Texture = texture;

        // Set particle colors
        _parts.ColorRamp = new Gradient();
        _parts.ColorRamp.SetColor(0, color);
        _parts.ColorRamp.SetColor(1, new Color(color, 0));
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
            _timeSinceGround += (float)delta;
        }
        else
        {
            if (_isHooked)
            {
                _isHooked = false;
            }
            _canAct = true;
            _timeSinceGround = 0;
        }

        if (_isStuck)
        {
            velocity = Vector2.Zero;
        }

        // Move
        Velocity = velocity;

        MoveAndSlide();

        // Bonk
        if (IsOnCeiling())
        {
            Velocity = new Vector2(Velocity.X, 0);
        }

        // Visuals
        _parts.Emitting = _canAct;
        _rope.Visible = _isHooked;
        if (_isHooked)
        {
            _rope.SetPointPosition(0, Vector2.Zero);
            _rope.SetPointPosition(1, ToLocal(Smoochus.GlobalPosition));
        }
    }

    private void PerPlayerInputs()
    {
        switch (GameManager.I.GetPlayerIDByCharacterType(Type))
        {
            case PlayerID.ONE:
                {
                    if (Input.IsActionJustPressed("act_p1") && _canAct && !IsOnFloor())
                    {
                        _isHooked = true;
                        _angularVelocity = 0;
                        _ropeLength = GlobalPosition.DistanceTo(Smoochus.GlobalPosition);
                    }
                    else if (Input.IsActionJustReleased("act_p1"))
                    {
                        _isHooked = false;
                        _canAct = false;
                    }
                    break;
                }

            case PlayerID.TWO:
                {
                    if (Input.IsActionJustPressed("act_p2") && _canAct)
                    {
                        _isStuck = true;
                    }
                    else if (Input.IsActionJustReleased("act_p2"))
                    {
                        _isStuck = false;
                        _canAct = false;
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
                return Input.IsActionJustPressed("jump_p1");
            case PlayerID.TWO:
                return Input.IsActionJustPressed("jump_p2");
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

        if (_timeSinceGround < 0.2f && WasJumpPressed())
        {
            newVelocity.Y -= JumpHeight;
        }

        return newVelocity;
    }

    public Vector2 Swing(Vector2 velocity)
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

        return velocity;
    }

    public void P2Act()
    {

    }
}
