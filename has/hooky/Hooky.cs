using Godot;
using System;

public partial class Hooky : CharacterBody2D
{
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
