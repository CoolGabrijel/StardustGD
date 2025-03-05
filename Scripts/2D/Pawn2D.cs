using Godot;
using Godot.Collections;
using System;

namespace Stardust.Godot
{
	public partial class Pawn2D : Node2D
	{
		public RoomGen2D RoomGen2D { get; set; }
		public Pawn Pawn { get; private set; }

		[Export] private Sprite2D charSprite;
		[Export] private Sprite2D auroraFlash;
		[Export] private ShaderMaterial[] shaders;
		[Export] private Texture2D[] textures;

		Tween moveTween;
		Tween auroraFlashTween;

		public void SetPawn(Pawn pawn)
		{
			Pawn = pawn;

			charSprite.Texture = textures[(int)Pawn.Type];
			charSprite.Material = shaders[(int)Pawn.Type];

			pawn.Moved += OnPawnMoved;
			if (pawn.Type == PawnType.Aurora)
			{
                pawn.OnDamageBlocked += OnAuroraBlock;
			}
		}

		private void OnPawnMoved()
		{
			Node2D room = RoomGen2D.GetRoomNodeByType(Pawn.Room.RoomType);
			PawnSlot2D slot = ((Room2D)room).GetVacantSlot();
			slot.AssignPawn(Pawn);

			if (moveTween != null)
			{
				moveTween.Kill();
			}

			moveTween = CreateTween();
			moveTween.TweenProperty(this, "global_position", slot.GlobalPosition, .25f).SetTrans(Tween.TransitionType.Spring);
			//GlobalPosition = room.Position;
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
