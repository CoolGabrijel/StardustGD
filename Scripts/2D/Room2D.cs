using Godot;
using Stardust.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		[Export] Label roomNameLabel;
		[Export] Control slot1;
        [Export] Control slot2;
		[ExportCategory("Buttons")]
		[Export] Button activationButton;

        Tween colorTween;
		Tween activationTween;
		Tween headerTween;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			area.InputEvent += OnInput;
			area.MouseEntered += OnHovered;
			area.MouseExited += OnMouseExit;
			Room.OnDamage += OnDamaged;
			Room.OnBreak += OnBroken;
			activationButton.Pressed += OnActivationClicked;

			activationGraphic.Position = new Vector2(-1125f, 0f);
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
			else activationGraphic.Hide();

            if (GameLogic.TurnQueue == null) return;
            
			if (room.Capacity == 1) slot1.Show();
			else slot2.Show();

			roomNameLabel.Text = room.Name;
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

			if (mouseEvent.ButtonMask == MouseButtonMask.Left)
			{
				//GD.Print($"{Room.Name}: Left click");
				if (GameLogic.TurnQueue.CurrentPawn.Room == Room) return; // We don't want to move to the Room we're already in.

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
			}
		}

		private void OnActivationClicked()
		{
			Pawn player = GameStart.LocalPlayer;

			if (player.Room != Room || !Room.CanBeActivated) return;

            Room.ActivateAbility(player);
        }

        private void OnHovered()
		{
			RoomSelection2D.Instance.SetPos(GlobalPosition);

			activationTween?.Kill();
			headerTween?.Kill();

			activationTween = activationGraphic.CreateTween();
			activationTween.TweenProperty(activationGraphic, "position", Vector2.Zero, .25f).SetTrans(Tween.TransitionType.Expo);

			headerTween = header.CreateTween();
            headerTween.TweenProperty(header, "position", Vector2.Zero, .25f).SetTrans(Tween.TransitionType.Expo);
        }

		private void OnMouseExit()
		{
            activationTween?.Kill();
            headerTween?.Kill();

            activationTween = activationGraphic.CreateTween();
            activationTween.TweenProperty(activationGraphic, "position", new Vector2(-1125f, 0f), 1f);

            headerTween = header.CreateTween();
            headerTween.TweenProperty(header, "position", new Vector2(-687f, 0f), 1f);
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

	public static class Pathfinding
	{
		public static Room[] GetPath(Room start, Pawn pawn)
		{
			var dfs = DepthFirstTraversal(start);
			PathNode pawnNode = null;

			foreach (var node in dfs)
			{
				if (node.Room == pawn.Room)
				{
					pawnNode = node;
					break;
				}
			}

			List<Room> result = new();
			PathNode current = pawnNode;
			while (current.Parent != null)
			{
				result.Add(current.Parent);
				foreach (var node in dfs)
				{
					if (node.Room == current.Parent) current = node;
				}
			}

			return result.ToArray();
		}

		public static IEnumerable<PathNode> DepthFirstTraversal(Room start)
		{
            var visited = new HashSet<PathNode>();
            var stack = new Stack<PathNode>();

            stack.Push(new(start, null));

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                if (!visited.Add(current))
                    continue;

                yield return current;

                List<PathNode> neighbours = new();

                foreach (var neighbour in current.Room.Neighbours)
                {
                    if (!visited.Contains(new(neighbour.Item2, current.Room))) neighbours.Add(new(neighbour.Item2, current.Room));
                }

                // If you don't care about the left-to-right order, remove the Reverse
                foreach (var neighbour in neighbours)
					stack.Push(neighbour);
			}
        }

		public class PathNode : IEquatable<PathNode>
        {
            public PathNode(Room room, Room parent)
            {
                Room = room;
                Parent = parent;
            }

            public Room Room;
			public Room Parent;

            public override bool Equals(object obj)
            {
                return Equals(obj as PathNode);
            }

            public bool Equals(PathNode other)
            {
                return other is not null &&
                       EqualityComparer<Room>.Default.Equals(Room, other.Room);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Room);
            }
        }
	}
}
