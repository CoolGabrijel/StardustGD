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
		[Export] private AudioStreamPlayer undoAudioPlayer;

		public override void _Ready()
		{
			nextTurnButton.Pressed += OnNextTurnPressed;
			undoButton.Pressed += OnUndoPressed;
		}

        public override void _Process(double delta)
        {
            nextTurnButton.Disabled = !CanNextTurn;
            
            if (Input.IsActionJustPressed("Undo"))
            {
	            OnUndoPressed();
            }
        }

		public static void AttemptNextTurn()
        {
			if (!CanNextTurn) return;

			IUndoableAction action;
			if (GameStart.LocalPlayer.Type == PawnType.Zambuko) action = new ZambukoEndTurn();
			else action = new EndTurn();
			action.Do();
            ActionLibrary.AddAction(action);
        }

		private void OnNextTurnPressed()
		{
			ReleaseFocus();
			AttemptNextTurn();
        }

		private void OnUndoPressed()
        {
	        ReleaseFocus();
	        
            if (GameLogic.TurnQueue.CurrentPawn != GameStart.LocalPlayer) return;

            ActionLibrary.UndoAction();
            undoAudioPlayer?.Play();
        }
	} 
}
