using Godot;
using Stardust.Actions;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace Stardust.Godot
{
	public partial class Pickup : Sprite2D
	{
		[Export] Area2D area;

		private Pawn2D pawn;
        private Item item;

		Tween moveTween;
        bool circleAroundGraphic = false;

		public void Initialize(Pawn2D holder, Vector2 globalPos, Item itemToPickup)
		{
			pawn = holder;
            item = itemToPickup;
			GlobalPosition = globalPos;
            area.InputEvent += OnInput;
            area.MouseEntered += OnHovered;
            area.MouseExited += OnMouseExit;
            MoveToPawn();
		}

        public override void _Process(double delta)
        {
            if (!circleAroundGraphic) return;

            //GlobalPosition = pawn.GetFreeItemSlotPosition(item);
            Vector2 destination = pawn.GetFreeItemSlotPosition(item);
            float distance = GlobalPosition.DistanceTo(destination);
            GlobalPosition = GlobalPosition.MoveToward(destination, .05f * distance);
        }

        private void MoveToPawn()
		{
            Scale = Vector2.Zero;
			moveTween?.Kill();
			moveTween = CreateTween();
            moveTween.TweenProperty(this, "scale", Vector2.One, .25f).SetTrans(Tween.TransitionType.Bounce);
			//moveTween.TweenProperty(this, "global_position", pawn.GlobalPosition, 1f);
            moveTween.TweenCallback(Callable.From(() => circleAroundGraphic = true));
        }

        private void OnInput(Node viewport, InputEvent @event, long shapeIdx)
        {
            if (@event is not InputEventMouseButton mouseEvent) return;

            if (mouseEvent.ButtonMask == MouseButtonMask.Left)
            {
                if (!CanDrop()) return;

                DropItem();
            }
        }

        private void DropItem()
        {
            IUndoableAction action = new DropItem(pawn.Pawn, pawn.Pawn.Room, item);
            action.Do();
            ActionLibrary.AddAction(action);

            QueueFree();
        }

        private bool CanDrop()
        {
            bool isReady = circleAroundGraphic;
            bool isLocalPawn = pawn.Pawn == GameStart.LocalPlayer;

            return isReady && isLocalPawn;
        }

        private void OnHovered()
        {
            if (!CanDrop()) return;

            Modulate = new Color(2, 2, 2);
        }

        private void OnMouseExit()
        {
            if (!CanDrop()) return;

            Modulate = new Color("374666");
        }
    } 
}
