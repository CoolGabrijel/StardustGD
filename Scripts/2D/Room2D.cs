using Godot;
using Godot.Collections;
using Stardust.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Godot
{
	[Tool]
	public partial class Room2D : Sprite2D
	{
		public Room Room { get; private set; }

		[Export] Area2D area;
		[ExportCategory("Pawn Sots")]
		[Export] PawnSlot2D middleSlot;
        [Export] PawnSlot2D topSlot;
        [Export] PawnSlot2D bottomSlot;
        [Export] PawnSlot2D extraSlot;
		[ExportCategory("Overlay")]
		[Export] TextureRect activationGraphic;
		[Export] Control header;
		[Export] TextureRect roomIconGraphic;
		[Export] Array<RoomIcon> roomIcons;
		[Export] Label roomNameLabel;
		[Export] Control slot1;
        [Export] Control slot2;
		[ExportCategory("Buttons")]
		[Export] Button activationButton;
		[ExportCategory("Sound")]
		[Export] AudioStreamPlayer clickSound;
		[ExportCategory("First Steps Extra")]
		[Export] Texture2D landerOrbit;
		[Export] Texture2D landerMars;

        Tween colorTween;
		Tween activationMoveTween;
		Tween activationHoverTween;
		Tween headerTween;
		Tween movementTween;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			area.InputEvent += OnInput;
			area.MouseEntered += OnHovered;
			area.MouseExited += OnMouseExit;
			Room.OnDamage += OnDamaged;
			Room.OnBreak += OnBroken;
			activationButton.Pressed += OnActivationClicked;
			activationButton.MouseEntered += OnActivationMouseEntered;
			activationButton.MouseExited += OnActivationMouseExited;

			activationButton.Position = new Vector2(-1125f, 897f);
			header.Position = new Vector2(-687f, 0f);
        }

        public void Initialize(Room room, Texture2D tex, Texture2D texActivation = null)
		{
			if (room == null)
			{
				GD.PrintErr("Room2D :: Attempted to generate null room.");
				return;
			}

			Room = room;
			Texture = tex;
			if (texActivation != null) activationGraphic.Texture = texActivation;
			else activationButton.Hide();

            if (GameLogic.TurnQueue == null) return;
            
			if (room.Capacity == 1) slot1.Show();
			else slot2.Show();

			roomNameLabel.Text = room.Name;

			if (roomIconGraphic != null)
			{
				for (int i = 0; i < roomIcons.Count; i++)
				{
					if (roomIcons[i].RoomType == room.RoomType)
                    {
                        roomIconGraphic.Texture = roomIcons[i].Icon;
						break;
                    }
                }
            }

            if (Room is Lander lander) lander.LanderMoved += OnLanderMoved;
            else if (Room is MarsTile)
            {
				slot1.Hide();
				slot2.Hide();
				header.Hide();
            }
        }

		public PawnSlot2D GetVacantSlot()
		{
			if (Room.Capacity > 1)
			{
				if (!topSlot.Occupied) return topSlot;
				if (!bottomSlot.Occupied) return bottomSlot;
			}
			else
			{
				if (!middleSlot.Occupied) return middleSlot;
			}

			return extraSlot;
		}

		private void OnInput(Node viewport, InputEvent @event, long shapeIdx)
		{
			if (@event is not InputEventMouseButton mouseEvent) return;

			if (GameLogic.TurnQueue.CurrentPawn != GameStart.LocalPlayer) return;

            if (mouseEvent.ButtonMask == MouseButtonMask.Left)
			{
				//GD.Print($"{Room.Name}: Left click");
				if (GameLogic.TurnQueue.CurrentPawn.Room == Room) return; // We don't want to move to the Room we're already in.

				// If Airlock is broken, we shouldn't be able to get to or leave solar panels.
				if (Room.RoomType == RoomType.SolarPanels || GameStart.LocalPlayer.Room.RoomType == RoomType.SolarPanels)
                {
                    Room airlock = GameLogic.RoomManager.GetRoomByType(RoomType.Airlock);
					if (airlock.Broken) return;
                }

				Direction movDir = GameStart.LocalPlayer.LastMovementDirection;
				Room[] path = Pathfinding.GetPath(Room, GameStart.LocalPlayer);
				int cost = GameStart.LocalPlayer.CalculateMoveCost(path);

				int localMaxEnergy = GameStart.LocalPlayer.EnergyCards.Where((c) => !c.Exhausted).Max(x => x.Energy);

                // No need to move anywhere when we don't have the energy.
                // TODO: Concorde's cost calculation breaks a bit when moving for free, attempting to go another direction midway, then trying to go the correct direction.
                if (GameLogic.EnergyExpended + cost > localMaxEnergy) return;
				// If the move will put us to max energy cost, then we have to make sure it's not a room at or over capacity.
				// If we move when we're at max energy, we won't be able to end the turn there or move anywhere else. So no point in going there.
				if (GameLogic.EnergyExpended + cost == localMaxEnergy)
				{
					if (Room.Pawns.Count >= Room.Capacity)
					{
						return;
					}
				}

				IUndoableAction moveAction = null;

				if (GameLogic.TurnQueue.CurrentPawn.Type == PawnType.Concorde)
                {
					Direction finalMoveDirection = Direction.None;
					Room previousRoom = null;
					if (path.Length > 1) previousRoom = path[^2];
					else previousRoom = GameLogic.TurnQueue.CurrentPawn.Room;

					finalMoveDirection = previousRoom.Neighbours.Where(n => n.Item2 == path[^1]).FirstOrDefault().Item1;

                    moveAction = new ConcordeMove(GameStart.LocalPlayer, cost, Room, movDir, finalMoveDirection);
                }
				else moveAction = new MoveAction(GameStart.LocalPlayer, cost, GameStart.LocalPlayer.Room, Room, movDir);

                //moveAction = new MoveAction(GameStart.LocalPlayer, cost, GameStart.LocalPlayer.Room, Room, movDir);
				moveAction.Do();
				ActionLibrary.AddAction(moveAction);
				
				if (PIOMP.Room.IsHost) ServerSend.Move(GameStart.PlayerId, new MoveAction(GameStart.LocalPlayer, cost, GameStart.LocalPlayer.Room, Room, movDir));
				
				clickSound?.Play();
			}
		}

		private void OnActivationClicked()
		{
			if (!CanActivate()) return;

			Pawn player = GameStart.LocalPlayer;

            Room.ActivateAbility(player);

            // If you click on the button, you "focus" it and then space bar (the end turn button) pushes it again. Release focus to fix.
            GetViewport().GuiReleaseFocus();
        }

		private bool CanActivate()
        {
            Pawn player = GameStart.LocalPlayer;
			bool isInRoom = player.Room == Room;
			bool canBeActivated = Room.CanBeActivated;
			bool hasEnergy = GameLogic.EnergyExpended < player.EnergyCards.Where((e) => !e.Exhausted).Max((e) => e.Energy);
			return isInRoom && canBeActivated && hasEnergy;
        }

        private void OnHovered()
		{
			RoomSelection2D.Instance.SetPos(GlobalPosition);

			activationMoveTween?.Kill();
			headerTween?.Kill();

			activationMoveTween = activationButton.CreateTween();
			activationMoveTween.TweenProperty(activationButton, "position", new Vector2(0f, 897f), .25f).SetTrans(Tween.TransitionType.Expo);

			headerTween = header.CreateTween();
            headerTween.TweenProperty(header, "position", Vector2.Zero, .25f).SetTrans(Tween.TransitionType.Expo);
        }

		private void OnMouseExit()
		{
            activationMoveTween?.Kill();
            headerTween?.Kill();

            activationMoveTween = activationButton.CreateTween();
            activationMoveTween.TweenProperty(activationButton, "position", new Vector2(-1125f, 897f), 1f);

            headerTween = header.CreateTween();
            headerTween.TweenProperty(header, "position", new Vector2(-687f, 0f), 1f);
        }

        private void OnActivationMouseEntered()
        {
            if (!CanActivate())
            {
	            activationButton.Disabled = true;
                activationButton.MouseDefaultCursorShape = Control.CursorShape.Arrow;
                return;
            }
            else
            {
	            activationButton.Disabled = false;
                activationButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            }

            activationHoverTween?.Kill();

            activationHoverTween = activationButton.CreateTween();
            activationHoverTween.TweenProperty(activationButton, "modulate", new Color(2, 2, 2), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        private void OnActivationMouseExited()
        {
            activationHoverTween?.Kill();

            activationHoverTween = activationButton.CreateTween();
            activationHoverTween.TweenProperty(activationButton, "modulate", Colors.White, .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        private void OnLanderMoved()
        {
			Lander lander = Room as Lander;
			movementTween?.Kill();

			Vector2 endPosition;
			if (lander.HasLanded)
            {
                endPosition = new Vector2(GlobalPosition.X, 1125 * 2);
				Texture = landerMars;
            }
            else
            {
                endPosition = new Vector2(GlobalPosition.X, 1125);
                Texture = landerOrbit;
            }

            movementTween = CreateTween();
			movementTween.SetEase(Tween.EaseType.Out);
			movementTween.SetTrans(Tween.TransitionType.Quad);
			movementTween.TweenProperty(this, "global_position", endPosition, 1f);
        }

        private void OnDamaged()
		{
			colorTween?.Kill();

			colorTween = CreateTween();

			colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#ff0000"), .25f);
			if (Room.Broken) colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#555555"), .5f);
            else colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#ffffff"), .5f);

			GD.Print($"{Room.Name}: Damaged");
        }

		private void OnBroken()
		{
            colorTween?.Kill();

            colorTween = CreateTween();

			// Creates a flashing effect
            colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#555555"), .2f);
            colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#ffffff"), .2f);
            colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#555555"), .2f);
            colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#ffffff"), .2f);
            colorTween.TweenProperty(this, "self_modulate", Color.FromHtml("#555555"), .2f);

            GD.Print($"{Room.Name}: Damaged and broke");
        }
	} 
}
