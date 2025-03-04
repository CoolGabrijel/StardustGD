using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot.UI
{
	public partial class TurnButtons : Control
	{
		public static bool CanNextTurn
		{
			get
			{
				bool currentPawn = GameLogic.TurnQueue.CurrentPawn == GameStart.LocalPlayer;
				bool roomHasCapacity = GameStart.LocalPlayer.Room.Pawns.Count <= GameStart.LocalPlayer.Room.Capacity;

				return currentPawn && roomHasCapacity;
            }
		}

		[Export] Button nextTurnButton;
		[Export] Button undoButton;

		public override void _Ready()
		{
			nextTurnButton.Pressed += OnNextTurnPressed;
			undoButton.Pressed += OnUndoPressed;
		}

        public override void _Process(double delta)
        {
            nextTurnButton.Disabled = !CanNextTurn;
        }

		public static void AttemptNextTurn()
        {
			if (!CanNextTurn) return;

            new EndTurn().Do();
        }

		private void OnNextTurnPressed()
		{
			AttemptNextTurn();
        }

		private void OnUndoPressed()
        {
            if (GameLogic.TurnQueue.CurrentPawn != GameStart.LocalPlayer) return;

            ActionLibrary.UndoAction();
        }
	} 
}
