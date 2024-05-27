using Godot;
using System;

namespace Stardust.Godot
{
    public partial class Camera2DController : Camera2D
    {
        [Export] private float movementSpeed { get; set; }
        [Export] private float zoomSpeed { get; set; }
        [Export] private float maxZoom { get; set; }
        [Export] private float minZoom { get; set; }

        private Vector3 input;
        private float desiredZoom;
        private Tween tween;

        public override void _Ready()
        {
            desiredZoom = Zoom.X;
        }

        public override void _Process(double delta)
        {
            GetMovementInput();

            Vector2 movement = new(input.X, input.Y);
            Translate(movement * movementSpeed);
        }

        private void GetMovementInput()
        {
            input.X = Input.GetActionRawStrength("MoveRight") - Input.GetActionRawStrength("MoveLeft"); // Right-Left
            input.Y = Input.GetActionRawStrength("MoveDown") - Input.GetActionRawStrength("MoveUp"); // Down-Up
        }

        private void SetNewZoom(float zoom)
        {
            desiredZoom += zoom * zoomSpeed;
            desiredZoom = Mathf.Clamp(desiredZoom, minZoom, maxZoom);

            if (tween != null) tween.Kill();

            tween = CreateTween();
            tween.TweenProperty(this, "zoom", Vector2.One * desiredZoom, 0.1f).SetEase(Tween.EaseType.Out);
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
                        input.Z = 1;
                        SetNewZoom(1);
                        break;
                    case MouseButton.WheelDown:
                        input.Z = -1;
                        SetNewZoom(-1);
                        break;
                }
            }
        }
    } 
}
