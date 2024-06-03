using Godot;
using System;

namespace Stardust.Godot
{
	public partial class TurnOrderDisplay : Control
	{
		[Export] private Control portraitParent;
		[Export] private PackedScene concordePrefab;
		[Export] private PackedScene auroraPrefab;
		[Export] private PackedScene zambukoPrefab;
		[Export] private PackedScene rosettaPrefab;
		[Export] private PackedScene wolframPrefab;

        public override void _Ready()
        {
			Show();

			DestroyChildren(portraitParent);

			foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
			{
				Node portrait = null;

				switch (pawn.Type)
				{
					case PawnType.Concorde:
						portrait = concordePrefab.Instantiate();
						break;
					case PawnType.Aurora:
                        portrait = auroraPrefab.Instantiate();
                        break;
					case PawnType.Zambuko:
                        portrait = zambukoPrefab.Instantiate();
                        break;
					case PawnType.Rosetta:
                        portrait = rosettaPrefab.Instantiate();
                        break;
					case PawnType.Wolfram:
                        portrait = wolframPrefab.Instantiate();
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
