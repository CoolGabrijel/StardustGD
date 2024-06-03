using Godot;
using System;

namespace Stardust.Godot
{
	public partial class TurnOrderDisplay : Control
	{
		[Export] private Control portraitParent;
		[Export] private PackedScene portraitPrefab;
        [ExportCategory("Shader Materials")]
        [Export] private ShaderMaterial concordeMat;
        [Export] private ShaderMaterial auroraMat;
        [Export] private ShaderMaterial zambukoMat;
        [Export] private ShaderMaterial rosettaMat;
        [Export] private ShaderMaterial wolframMat;
        [ExportCategory("Textures")]
		[Export] private Texture2D concordeTex;
		[Export] private Texture2D auroraTex;
		[Export] private Texture2D zambukoTex;
		[Export] private Texture2D rosettaTex;
		[Export] private Texture2D wolframTex;
        [ExportCategory("Colors")]
        [Export] private Color concordeColor;
        [Export] private Color auroraColor;
        [Export] private Color zambukoColor;
        [Export] private Color rosettaColor;
        [Export] private Color wolframColor;

        public override void _Ready()
        {
			Show();

			DestroyChildren(portraitParent);

            foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
            {
                TurnOrderItem portrait = null;

                portrait = portraitPrefab.Instantiate<TurnOrderItem>();

                switch (pawn.Type)
                {
                    case PawnType.Concorde:
                        portrait.Init(concordeTex, concordeMat, Color.FromHtml("#ffffff"), pawn.Type);
                        break;
                    case PawnType.Aurora:
                        portrait.Init(auroraTex, auroraMat, Color.FromHtml("#f0b924"), pawn.Type);
                        break;
                    case PawnType.Zambuko:
                        portrait.Init(zambukoTex, zambukoMat, Color.FromHtml("#333333"), pawn.Type);
                        break;
                    case PawnType.Rosetta:
                        portrait.Init(rosettaTex, rosettaMat, Color.FromHtml("#ca64ff"), pawn.Type);
                        break;
                    case PawnType.Wolfram:
                        portrait.Init(wolframTex, wolframMat, Color.FromHtml("#81bbff"), pawn.Type);
                        break;
                    default:
                        break;
                }

                portraitParent.AddChild(portrait);
            }
        }

		private void DestroyChildren(Node node)
		{
			for (int i = node.GetChildCount() - 1; i >= 0; i--)
			{
				node.GetChild(i).QueueFree();
			}
		}
    } 
}
