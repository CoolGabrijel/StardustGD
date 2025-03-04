using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot.UI
{
	public partial class TurnButtons : Control
	{
		[Export] Button nextTurnButton;
		[Export] Button undoButton;

		public override void _Ready()
		{
			nextTurnButton.Pressed += OnNextTurnPressed;
			undoButton.Pressed += OnUndoPressed;
		}

		private void OnNextTurnPressed()
		{
			if (GameLogic.TurnQueue.CurrentPawn != GameStart.LocalPlayer) return;

            new EndTurn().Do();
        }

		private void OnUndoPressed()
        {
            if (GameLogic.TurnQueue.CurrentPawn != GameStart.LocalPlayer) return;

            ActionLibrary.UndoAction();
        }
	} 
}
