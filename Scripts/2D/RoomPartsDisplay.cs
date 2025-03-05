using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot
{
	public partial class RoomPartsDisplay : Button
	{
		[Export] private Room2D roomGraphic;
        [Export] Label partsLabel;
        [Export] PackedScene pickupPrefab;
        [Export] Vector2 pickupSpawnOffset;

        Tween colorTween;

        public override void _Ready()
        {
            Pressed += OnClick;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        public override void _Process(double delta)
        {
            if (roomGraphic.Room.Parts > 0)
            {
                Show();
                partsLabel.Text = $"x{roomGraphic.Room.Parts}";
                Disabled = false;
            }
            else
            {
                Hide();
                Disabled = true;
            }
        }

        private void OnClick()
        {
            Pawn player = GameStart.LocalPlayer;
            if (!CanPickup()) return;

            Item item = roomGraphic.Room.GetItem(ItemType.Part);
            PickUpPart PickupAction = new(player, roomGraphic.Room, item);
            PickupAction.Do();
            ActionLibrary.AddAction(PickupAction);

            Pickup pickup = pickupPrefab.Instantiate<Pickup>();
            pickup.Initialize(GameStart.GetPawnGraphic(player), roomGraphic.GlobalPosition + pickupSpawnOffset, item);
            GetTree().Root.AddChild(pickup);
            GetViewport().GuiReleaseFocus(); // Stops spacebar from activating button
        }

        private bool CanPickup()
        {
            Pawn player = GameStart.LocalPlayer;
            bool playerHasInv = player.CanPickUp;
            bool isInRoom = roomGraphic.Room.Pawns.Contains(player);
            return playerHasInv && isInRoom;
        }

        private void OnMouseEntered()
        {
            if (!CanPickup())
            {
                MouseDefaultCursorShape = CursorShape.Arrow;
                return;
            }
            else
            {
                MouseDefaultCursorShape = CursorShape.PointingHand;
            }

            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", new Color(2, 2, 2), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        private void OnMouseExited()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", Colors.White, .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        public override void _ExitTree()
        {
            Pressed -= OnClick;
        }
    } 
}
