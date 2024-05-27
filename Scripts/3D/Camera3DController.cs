using Godot;
using System;

namespace Stardust.Godot
{
    public partial class Camera3DController : Node3D
    {
        [Export] private Node3D camNode;
        [Export] private float speed;
        [Export] private float rotationSpeed;
        [Export] private float zoomSpeed;

        private Vector3 input;
        private float desiredZoom;
        private float degToRotate;
        private Tween zoomTween;

        public override void _Ready()
        {
            desiredZoom = (Vector3.One * camNode.Position).Y;
        }

        public override void _Process(double delta)
        {
            GetMovementInput();

            Vector3 movement = new Vector3(input.X, 0f, input.Y) * speed;

            Translate(movement);

            RotateY(Mathf.DegToRad(degToRotate * (float)delta) * rotationSpeed);
            degToRotate -= degToRotate * (float)delta * rotationSpeed;
        }

        private void GetMovementInput()
        {
            input.X = Input.GetActionRawStrength("MoveRight") - Input.GetActionRawStrength("MoveLeft"); // Right-Left
            input.Y = Input.GetActionRawStrength("MoveDown") - Input.GetActionRawStrength("MoveUp"); // Down-Up

            bool cwRot = Input.IsActionJustPressed("RotateCW");
            bool ccwRot = Input.IsActionJustPressed("RotateCCW");

            if (cwRot) BeginRotate(-45);
            if (ccwRot) BeginRotate(45);
        }

        private void BeginRotate(int rot)
        {
            degToRotate += rot;
        }

        private void SetNewZoom(float zoom)
        {
            desiredZoom += zoom * zoomSpeed;
            desiredZoom = Mathf.Clamp(desiredZoom, 0.2f, 5f);

            if (zoomTween != null) zoomTween.Kill();

            zoomTween = CreateTween();
            zoomTween.TweenProperty(camNode, "position", (Vector3.One * camNode.Position).Normalized() * desiredZoom, 0.1f).SetEase(Tween.EaseType.Out);
        }

        // This is used for Zoom.
        // Apparently Input.GetAction can't quite handle mouse wheels.
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                switch (mouseEvent.ButtonIndex)
                {
                    case MouseButton.WheelUp:
                        SetNewZoom(-1);
                        break;
                    case MouseButton.WheelDown:
                        SetNewZoom(1);
                        break;
                }
            }
        }
    } 
}
