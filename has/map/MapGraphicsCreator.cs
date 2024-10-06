using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class MapGraphicsCreator : Node
{
    [Export]
    public StringName MapPolyGroupName = "map_poly";

    private List<CanvasItem> _visualPolys = new();

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
                float hue = (float)GD.RandRange(0.0, 1.0);

                Polygon2D newVisualPoly = new()
                {
                    Polygon = collisionPoly.Polygon,
                    Color = Color.FromHsv(hue, 0.6f, 0.7f, 1.0f),
                    Transform = collisionPoly.Transform,
                    Antialiased = true,
                    ZIndex = collisionPoly.ZIndex
                };
                AddChild(newVisualPoly);
                _visualPolys.Add(newVisualPoly);

                // Create randomized curve
                Curve sillyCurve = new Curve();
                int numPoints = 20;
                for (uint i = 0; i < numPoints; i++) {
                    sillyCurve.AddPoint(new Vector2(
                        i / (numPoints - 1),
                        (float)GD.RandRange(0.5, 1)
                    ));
                }

                Line2D newVisualLinePoly = new()
                {
                    Points = collisionPoly.Polygon,
                    DefaultColor = Color.FromHsv(hue, 0.8f, 0.5f, 1.0f),
                    Transform = collisionPoly.Transform,
                    Closed = true,
                    WidthCurve = sillyCurve,
                    Width = 0.5f,
                    ZIndex = collisionPoly.ZIndex + 1
                };
                AddChild(newVisualLinePoly);
                _visualPolys.Add(newVisualLinePoly);
            }
        }
    }
}
