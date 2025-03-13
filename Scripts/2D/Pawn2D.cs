using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Godot
{
	public partial class Pawn2D : Node2D
	{
		public RoomGen2D RoomGen2D { get; set; }
		public Pawn Pawn { get; private set; }

        [Export] Area2D area;
        [Export] private Sprite2D charSprite;
		[Export] private Node2D energyCardContainer;
		[Export] private Sprite2D auroraFlash;
		[Export] private Node2D wolframAbilityNode;
		[Export] private ShaderMaterial[] shaders;
		[Export] private Texture2D[] textures;
        [Export] PackedScene pickupPrefab;

		Vector2 wolframNodePosition;
		Dictionary<EnergyCard, Node2D> cardToNode = new();

        Tween moveTween;
		Tween auroraFlashTween;
		Tween wolframTween;

		public void SetPawn(Pawn pawn)
		{
			Pawn = pawn;

			charSprite.Texture = textures[(int)Pawn.Type];
			charSprite.Material = shaders[(int)Pawn.Type];

			pawn.Moved += OnPawnMoved;
			pawn.OnItemPickedUp += OnItemPickedUp;

			if (pawn.Type == PawnType.Aurora)
			{
                pawn.OnDamageBlocked += OnAuroraBlock;
			}

			SetupEnergyCards();

			wolframNodePosition = wolframAbilityNode.Position;
			wolframAbilityNode.Position = Vector2.Zero;
			wolframAbilityNode.Scale = Vector2.Zero;
			wolframAbilityNode.Modulate = Colors.Transparent;
			energyCardContainer.Scale = Vector2.Zero;
			energyCardContainer.Modulate = Colors.Transparent;

            area.MouseEntered += OnMouseEnter;
            area.MouseExited += OnMouseExited;
		}

        public Vector2 GetFreeItemSlotPosition(Item item)
		{
			float segment = Mathf.Tau / Pawn.Inventory.Count;
			float segmentPos = segment;
			for (int i = 0; i < Pawn.Inventory.Count; i++)
			{
				if (Pawn.Inventory[i] == item)
				{
					segmentPos = (i+1) * segment;
					break;
				}
			}
			float timeOffset = Time.GetTicksMsec() / 4000f;

			Vector2 direction = Vector2.Right.Rotated(segmentPos + timeOffset);

            return GlobalPosition + direction * 150;
        }

		private void SetupEnergyCards()
		{
			Node energyCardPrefab = energyCardContainer.GetChild(0).Duplicate();
			energyCardContainer.FreeChildren();

			float angle = -60f;
			float angleSegment = 120f / (Pawn.EnergyCards.Length - 1);

			foreach (var card in Pawn.EnergyCards)
			{
				Node2D dupe = energyCardPrefab.Duplicate() as Node2D;
				dupe.GetNode<Label>("Label").Text = card.Energy.ToString();
				energyCardContainer.AddChild(dupe);
				dupe.RotationDegrees = angle;
				angle += angleSegment;
				cardToNode.Add(card, dupe);
			}
		}

        private void OnMouseEnter()
        {
			wolframTween?.Kill();

			wolframTween = wolframAbilityNode.CreateTween();
			wolframTween.SetEase(Tween.EaseType.Out);
			wolframTween.SetTrans(Tween.TransitionType.Circ);
			wolframTween.TweenProperty(wolframAbilityNode, "position", wolframNodePosition, .5f);
			wolframTween.Parallel().TweenProperty(wolframAbilityNode, "scale", Vector2.One, .5f);
			wolframTween.Parallel().TweenProperty(wolframAbilityNode, "modulate", Colors.White, .5f);

			foreach (KeyValuePair<EnergyCard, Node2D> pair in cardToNode)
			{
				if (pair.Key.Exhausted) pair.Value.Modulate = Colors.White / 2;
				else pair.Value.Modulate = Colors.White;
			}

            wolframTween.Parallel().TweenProperty(energyCardContainer, "scale", Vector2.One, .5f);
            wolframTween.Parallel().TweenProperty(energyCardContainer, "modulate", Colors.White, .5f);
        }

        private void OnMouseExited()
        {
            wolframTween?.Kill();

            wolframTween = wolframAbilityNode.CreateTween();
            wolframTween.SetEase(Tween.EaseType.Out);
            wolframTween.SetTrans(Tween.TransitionType.Circ);
            wolframTween.TweenProperty(wolframAbilityNode, "position", Vector2.Zero, .75f);
            wolframTween.Parallel().TweenProperty(wolframAbilityNode, "scale", Vector2.Zero, .75f);
            wolframTween.Parallel().TweenProperty(wolframAbilityNode, "modulate", Colors.Transparent, .75f);

            wolframTween.Parallel().TweenProperty(energyCardContainer, "scale", Vector2.Zero, .75f);
            wolframTween.Parallel().TweenProperty(energyCardContainer, "modulate", Colors.Transparent, .75f);
        }

        private void OnPawnMoved()
		{
			Node2D room = RoomGen2D.GetRoomNodeByType(Pawn.Room.RoomType);
			PawnSlot2D slot = ((Room2D)room).GetVacantSlot();
			slot.AssignPawn(Pawn);

			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.TweenProperty(this, "global_position", slot.GlobalPosition, .25f).SetTrans(Tween.TransitionType.Spring);
		}

		private void OnItemPickedUp(Item item)
        {
            Pickup pickup = pickupPrefab.Instantiate<Pickup>();
			Vector2 mousePos = GetGlobalMousePosition();
            pickup.Initialize(this, mousePos, item);
            GetTree().Root.AddChild(pickup);
        }

		private void OnAuroraBlock()
		{
			auroraFlashTween?.Kill();

			auroraFlash.Scale = Vector2.One;
			auroraFlash.SelfModulate = Colors.White;

			auroraFlashTween = auroraFlash.CreateTween();
			auroraFlashTween.TweenProperty(auroraFlash, "scale", Vector2.One * 10, 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            auroraFlashTween.Parallel().TweenProperty(auroraFlash, "self_modulate", Colors.Transparent, 1.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

            GD.Print($"Aurora blocked damage in {Pawn.Room.Name}");
        }
	} 
}
