using Godot;
using System;

public enum PlayerType {
    HOOKY, SMOOCHUS
}

public partial class Player : CharacterBody2D
{
    [Export]
    public PlayerType Type;

    public override void _Ready()
    {
        base._Ready();

        GD.Print("HELLO!");
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
