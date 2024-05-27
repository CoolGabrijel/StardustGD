using Godot;
using System;

namespace Stardust.Godot
{
	[Tool]
	public partial class PawnSlot2D : Node2D
	{
		public bool Occupied => assignedPawn != null;

		private Pawn assignedPawn;

		public void AssignPawn(Pawn pawn)
		{
			assignedPawn = pawn;
			assignedPawn.Moved += OnMoved;
		}

		private void OnMoved()
		{
			assignedPawn.Moved -= OnMoved;
			assignedPawn = null;
		}
	} 
}
