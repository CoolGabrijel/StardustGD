using Godot;
using System;

namespace Stardust.Godot
{
	public partial class VictoryScreen : Control
	{
		[Export] private Control popup;
		[Export] private Control screen;

        private Vector2 originalPopupSize;
        private Vector4 originalOffsets;
        private Tween tweenPopup;

        public override void _Ready()
        {
            originalOffsets.X = popup.OffsetBottom;
            originalOffsets.Y = popup.OffsetRight;
            originalOffsets.Z = popup.OffsetTop;
            originalOffsets.W = popup.OffsetLeft;
            originalPopupSize = popup.Size;
            screen.Hide();

            GameLogic.OnGameFinished += Popup;
        }

        public override void _Process(double delta)
        {
            if (Input.IsKeyPressed(Key.M)) Popup(true);
        }

        public override void _UnhandledKeyInput(InputEvent @event)
        {
            if (!Visible) return;

            if (@event is InputEventKey eventKey)
            {
                if (eventKey.Keycode == Key.Escape)
                {
                    Hide();
                }
            }
        }

        private void QuitToMainMenu()
        {
            if (PIOMP.Room.IsInRoom) PIOMP.Room.LeaveRoom();
            
            GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
        }

        private void Popup(bool v)
        {
            GD.Print($"Game End State: {v}");

            Show();

            tweenPopup?.Kill();

            screen.Hide();

            popup.OffsetBottom = originalOffsets.X;
            popup.OffsetRight = originalOffsets.Y;
            popup.OffsetTop = originalOffsets.Z;
            popup.OffsetLeft = originalOffsets.W;
            popup.Size = originalPopupSize;

            popup.Show();

            tweenPopup = popup.CreateTween();
            tweenPopup.TweenProperty(popup, "position", screen.Position, .25f);
            tweenPopup.TweenProperty(popup, "size", screen.Size, .15f);
            tweenPopup.TweenCallback(Callable.From(HidePopupShowScreen));
        }

        private void HidePopupShowScreen()
        {
            popup.Hide();
            screen.Show();
        }
    } 
}
