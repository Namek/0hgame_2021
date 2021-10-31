using System;
using Godot;

public class Fish : KinematicBody2D {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  [Export] public Vector2 Velocity = Vector2.Zero;
  [Export] public float RotationSpeed = 1f;


  public override void _Ready() {

  }


  public override void _Process(float delta) {

    if (Velocity.Length() > 0) {
      MoveAndCollide(Velocity * delta);
      RotationDegrees = Mathf.Clamp(this.RotationDegrees + RotationSpeed * delta * 90, -360, 360);
    }
  }
}
