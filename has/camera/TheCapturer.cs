using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class TheCapturer : Camera2D
{
    [Export]
    public float MaxZoom = 1f;

    private float Aspect = 0f;
    private Vector2 Size;

    private Rect2 IdealRect;

    public override void _EnterTree()
    {
        base._Ready();

        Size = GetViewportRect().Size;
        Aspect = Size.Aspect();
    }

    public void FitNodes(Node2D[] nodes)
    {
        if (nodes.Length == 0)
        {
            GD.PrintErr("No nodes to focus!");
            return;
        }

        // Find extremes
        Rect2 containmentSize = new(nodes[0].GlobalPosition, Vector2.One);
        Vector2 center = Vector2.Zero;
        foreach (Node2D node in nodes)
        {
            containmentSize = containmentSize.Expand(node.GlobalPosition);
            center += node.GlobalPosition;
        }
        center /= nodes.Length;

        GD.Print("Containment size: " + containmentSize.ToString());

        // Determine ideal size
        float verticalRange = containmentSize.Size.Y;
        float horizontalRange = containmentSize.Size.X;

        Vector2 idealSize = new();

        if (verticalRange * Aspect > horizontalRange) {
            idealSize.X = verticalRange * Aspect;
            idealSize.Y = verticalRange;
        }
        if (horizontalRange * (1.0f / Aspect) > verticalRange) {
            idealSize.X = horizontalRange;
            idealSize.Y = horizontalRange * (1.0f / Aspect);
        }

        const float paddingPixels = 200.0f;
        idealSize += Vector2.One * paddingPixels;

        IdealRect = new Rect2(center - idealSize / 2, idealSize);

        // Put camera in right place
        GlobalPosition = IdealRect.GetCenter();

        Vector2 idealZoom = new Vector2(
            Size.X / IdealRect.Size.X,
            Size.Y / IdealRect.Size.Y
        );
        idealZoom = idealZoom.Clamp(1.0f, MaxZoom);
        Zoom = Zoom.Lerp(idealZoom, 0.1f);

        // QueueRedraw();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // Find nodes to fit
        List<Node2D> taggedNodes = new();
        foreach (Node node in GetTree().GetNodesInGroup("player")) {
            if (node is Node2D) {
                taggedNodes.Add((Node2D)node);
            }
        }
        FitNodes(taggedNodes.ToArray());
    }

    public override void _Draw()
    {
        base._Draw();

        // DrawString(ThemeDB.FallbackFont, new Vector2(10, 10), IdealRect.ToString());
        // DrawString(ThemeDB.FallbackFont, new Vector2(10, 20), IdealRect.Size.Aspect().ToString());
        // DrawRect(IdealRect, new Color(Colors.Cyan, 0.2f));
    }
}
