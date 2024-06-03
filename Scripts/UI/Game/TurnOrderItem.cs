using Godot;
using System;

namespace Stardust.Godot
{
	public partial class TurnOrderItem : Node
	{
		[Export] private TextureRect portrait;
		[Export] private Label pawnNameLabel;
		[Export] private GpuParticles2D particles;

		private PawnType pawnType;

        public override void _Process(double delta)
        {
			if (GameLogic.TurnQueue.CurrentPawn.Type == pawnType) particles.Emitting = true;
			else particles.Emitting = false;
        }

        public void Init(Texture2D tex, ShaderMaterial shader, Color color, PawnType type)
		{
			portrait.Texture = tex;
			portrait.Material = shader;

			particles.SelfModulate = color;
			pawnType = type;
			pawnNameLabel.Text = type.ToString();
		}
	} 
}
