using System.Collections.Generic;
using System.Linq;
using Godot;
using static MoreLinq.Extensions.MinByExtension;

public class Game : Node2D {
  private Penguin penguin = null;
  private List<Fish> allFish = new List<Fish>();
  private RichTextLabel pointsLabel;

  [Export] public int Points = 0;


  public override void _Ready() {
    pointsLabel = GetNode<RichTextLabel>("./PointsLabel");
    penguin = GetNode<Penguin>("./Entities/Penguin");
    foreach (var child in GetNode<Node2D>("./Entities").GetChildren()) {
      if (child is Fish f) {
        allFish.Add(f);
      }
    }
  }
  public override void _Input(InputEvent evt) {
    if (evt.IsActionPressed("ui_cancel")) {
      GetTree().Quit(0);
      return;
    }
  }

  public void _on_GestureDetector_DragStart(DragGesture drag) {

  }

  public void _on_GestureDetector_DragEnd() {

  }

  public void _on_GestureDetector_Fling(FlingGesture fling) {
    var fish = allFish.MinBy(f => f.Position.DistanceTo(fling.StartPos)).First();

    GD.Print($"throw {fling.PosDiff.x} {fling.PosDiff.y}");

    if (fish != null) {
      fish.Velocity = fling.PosDiff * 3.5f;
      fish.RotationSpeed = 0.2f + GD.Randf() * 2f;
      if (GD.Randf() > 0.5f) {
        fish.RotationSpeed *= -1;
      }
    }
  }

  private void _on_Area2D_body_entered(Node2D body) {
    if (!allFish.Contains(body as Fish)) {
      return;
    }

    GD.Print("+1 point");
    Points += 1;
    pointsLabel.Text = Points.ToString();
    penguin.AnimateEat();
    allFish.Remove(body as Fish);
    body.QueueFree();
  }

  public override void _Process(float delta) {

  }
}



public static class Res {
  public static string Uri(string path) => "res:///" + path;
  public static class Prefab {
    public static string Uri(string name) => Res.Uri("Prefabs/" + name + ".tscn");

  }
}