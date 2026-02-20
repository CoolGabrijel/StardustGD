using Godot;
using Stardust.Actions;

namespace Stardust.Godot
{
	public partial class Pickup : Sprite2D
	{
		[Export] Area2D area;
        [Export] Texture2D sparePartTexture;
        [Export] Texture2D flagTexture;

		private Pawn2D pawn;
        private Item item;

		Tween moveTween;
        bool circleAroundGraphic = false;

		public void Initialize(Pawn2D holder, Vector2 globalPos, Item itemToPickup)
		{
			pawn = holder;
            item = itemToPickup;
			GlobalPosition = globalPos;
            SetTexture();
            pawn.Pawn.OnItemDropped += OnItemDropped;
            area.InputEvent += OnInput;
            area.MouseEntered += OnHovered;
            area.MouseExited += OnMouseExit;
            MoveToPawn();
		}

        public override void _Process(double delta)
        {
            if (!circleAroundGraphic) return;

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
            IUndoableAction action = null;

            switch (item.Type)
            {
                case ItemType.Part:
                    action = new DropItem(pawn.Pawn, pawn.Pawn.Room, item);
                    break;
                case ItemType.Sample:
                    if (pawn.Pawn.Room.RoomType == RoomType.Lander)
                        action = new CompleteMarsTask(pawn.Pawn, item);
                    else
                        action = new DropItem(pawn.Pawn, pawn.Pawn.Room, item);
                    break;
                case ItemType.Flag:
                    if (pawn.Pawn.Room.RoomType == RoomType.Peak)
                        action = new CompleteMarsTask(pawn.Pawn, item);
                    else
                        action = new DropItem(pawn.Pawn, pawn.Pawn.Room, item);
                    break;
                case ItemType.Objective:
                    break;
                default:
                    break;
            }

            action.Do();
            ActionLibrary.AddAction(action);
            
            if (PIOMP.Room.IsHost) ServerSend.Drop(GameStart.PlayerId, item.Type);
            else if (PIOMP.Room.IsInRoom) ClientSend.ReqDrop(item.Type);
        }

        private bool CanDrop()
        {
            bool isReady = circleAroundGraphic;
            bool isLocalPawn = pawn.Pawn == GameStart.LocalPlayer;

            return isReady && isLocalPawn;
        }

        private void OnItemDropped(Pawn pawn, Item droppedItem)
        {
            if (droppedItem != item) return;
            
            this.pawn.Pawn.OnItemDropped -= OnItemDropped;
            QueueFree();
        }

        private void OnHovered()
        {
            if (!CanDrop()) return;

            Modulate = new Color(2, 2, 2);
        }

        private void OnMouseExit()
        {
            if (!CanDrop()) return;

            if (item.Type == ItemType.Part) Modulate = new Color("374666");
            else Modulate = Colors.White;
        }

        private void SetTexture()
        {
            switch (item.Type)
            {
                case ItemType.Part:
                    break;
                case ItemType.Sample:
                    break;
                case ItemType.Flag:
                    Texture = flagTexture;
                    break;
                case ItemType.Objective:
                    break;
                default:
                    break;
            }
        }
    } 
}
