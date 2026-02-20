using Godot;
using Stardust.Actions;
using System;
using System.Linq;

namespace Stardust.Godot
{
	public partial class RoomPartsDisplay : Button
	{
		[Export] private Room2D roomGraphic;
        [Export] Label partsLabel;
        [Export] PackedScene pickupPrefab;
        [Export] Vector2 pickupSpawnOffset;
        [Export] ItemType pickupType;

        Tween colorTween;

        public override void _Ready()
        {
            Pressed += OnClick;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        public override void _Process(double delta)
        {
            int itemAmount = roomGraphic.Room.Items.Where((i) => i.Type == pickupType).Count();

            if (itemAmount > 0)
            {
                Show();
                partsLabel.Text = $"x{itemAmount}";
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

            Item item = roomGraphic.Room.GetItem(pickupType);
            PickUpPart PickupAction = new(player, roomGraphic.Room, item);
            PickupAction.Do();
            ActionLibrary.AddAction(PickupAction);
            
            if (PIOMP.Room.IsHost) ServerSend.Pickup(GameStart.PlayerId, item.Type);
            else if (PIOMP.Room.IsInRoom) ClientSend.ReqPickup(item.Type);

            //Pickup pickup = pickupPrefab.Instantiate<Pickup>();
            //pickup.Initialize(GameStart.GetPawnGraphic(player), roomGraphic.GlobalPosition + pickupSpawnOffset, item);
            //GetTree().Root.AddChild(pickup);
            //GetViewport().GuiReleaseFocus(); // Stops spacebar from activating button
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
