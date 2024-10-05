using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class MapGraphicsCreator : Node
{
    [Export]
    public StringName MapPolyGroupName = "map_poly";

    private List<Polygon2D> _visualPolys = new();

    public override void _Ready()
    {
        base._Ready();

        GenerateGraphics(GetTree().GetNodesInGroup(MapPolyGroupName));
    }

    public void GenerateGraphics(Array<Node> nodes) {
        // Clean old polys
        foreach (Polygon2D visualPoly in _visualPolys) {
            visualPoly.QueueFree();
        }
        _visualPolys.Clear();

        // Generate new ones!
        GD.Randomize();
        foreach (Node node in nodes) {
            if (node is CollisionPolygon2D collisionPoly) {
                Polygon2D newVisualPoly = new()
                {
                    Polygon = collisionPoly.Polygon,
                    Color = Color.FromHsv((float)GD.RandRange(0.0, 1.0), 0.6f, 0.7f, 1.0f),
                    Transform = collisionPoly.Transform
                };
                AddChild(newVisualPoly);
            }
        }
    }
}
