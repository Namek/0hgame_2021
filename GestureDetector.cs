using System;
using Godot;

public class FlingGesture : Godot.Object {
  public Vector2 StartPos;
  public Vector2 EndPos;
  public Vector2 PosDiff;
  public uint TimeMs;
  public Vector2.Axis Axis;
  public float AmountX;
  public float AmountY;
}

public class DragGesture : Godot.Object {
  public Vector2 Pos;
  public bool IsEnd;
}

public class GestureDetector : Node2D {
  bool isPressed = false;
  Vector2 startPressPos;
  ulong startPressTime;

  [Signal] public delegate void Fling(FlingGesture gesture);
  [Signal] public delegate void DragStart();
  public delegate void DragContinue();
  [Signal] public delegate void DragEnd();



  public const int MIN_DISTANCE_TO_FLING = 20;
  public const int MAX_TIME_MS_TO_FLING = 300;


  public override void _Ready() {
    int a = 5;
    // var rect = new RectangleShape2D();
    // rect.Extents = new Vector2(1000 / 2, 500 / 2);//half the size of GameMap
    // this.Shape = rect;

  }
  public override void _Input(InputEvent evt) {
    // return;
    // if (evt is InputEventMouseButton e1) {
    //   if (e1.ButtonIndex == (int)ButtonList.Left) {
    //     if (e1.Pressed && lastMousePress == null) {
    //       startPressTime = OS.GetTicksMsec();
    //       startPressPos = e1.GlobalPosition;
    //       lastMousePress = e1;
    //       GD.Print("start");

    //       // TODO Fire start drag
    //     } else if (!e1.Pressed && lastMousePress != null) {
    //       lastMousePress = null;

    //       var timeDiffMs = OS.GetTicksMsec() - startPressTime;
    //       var posDiff = e1.GlobalPosition - startPressPos;

    //       if (timeDiffMs < MAX_TIME_MS_TO_FLING) {
    //         var x = Math.Abs(posDiff.x);
    //         var y = Math.Abs(posDiff.y);
    //         if (x != 0 && y != 0 && Math.Max(x, y) > MIN_DISTANCE_TO_FLING) {
    //           // TODO some bug occurs here: event is fired twice. Or once when debugging.
    //           if (x > y) {
    //             _fireFlingGesture(Vector2.Axis.X, posDiff.x);
    //           } else {
    //             _fireFlingGesture(Vector2.Axis.Y, posDiff.y);
    //           }
    //           GD.Print(e1);
    //         }
    //       }

    //       // TODO Fire end drag
    //     }

    //   }
    // } else if (evt is InputEventMouseMotion e2) {
    //   if (lastMousePress != null) {
    //     // TODO Fire drag continue
    //     // GD.Print(e2.Relative);
    //   }
    // }
  }

  // private void _fireFlingGesture(Vector2.Axis axis, float amountx, float amounty, Vector2 posDiff) {
  //   var flingEvt = new FlingGesture() {
  //     Axis = axis,
  //     AmountX = amountx,
  //     AmountY = amounty,
  //     PosDiff = posDiff
  //   };
  //   GD.Print("fire fling");
  //   EmitSignal(nameof(Fling), flingEvt);
  // }

  private void _on_GestureDetector_pressed() {
    if (isPressed) return;

    startPressTime = OS.GetTicksMsec();
    startPressPos = GetViewport().GetMousePosition();

    var dragEvt = new DragGesture {
      Pos = startPressPos,
      IsEnd = false
    };
    EmitSignal(nameof(DragStart), dragEvt);
  }

  private void _on_GestureDetector_released() {
    isPressed = false;

    var endPos = GetViewport().GetMousePosition();
    var timeDiffMs = OS.GetTicksMsec() - startPressTime;
    var posDiff = endPos - startPressPos;

    if (timeDiffMs < MAX_TIME_MS_TO_FLING) {
      var x = Math.Abs(posDiff.x);
      var y = Math.Abs(posDiff.y);
      if (x != 0 && y != 0 && Math.Max(x, y) > MIN_DISTANCE_TO_FLING * OS.GetScreenScale()) {
        var dragEvt = new DragGesture {
          Pos = endPos,
          IsEnd = true
        };
        EmitSignal(nameof(DragEnd), dragEvt);
        // _fireFlingGesture(x > y ? Vector2.Axis.X : Vector2.Axis.Y, posDiff.x, posDiff.y, posDiff);

        var flingEvt = new FlingGesture() {
          Axis = x > y ? Vector2.Axis.X : Vector2.Axis.Y,
          AmountX = posDiff.x,
          AmountY = posDiff.y,
          StartPos = startPressPos,
          EndPos = endPos,
          PosDiff = posDiff
        };

        EmitSignal(nameof(Fling), flingEvt);
      }
    }
  }
}
