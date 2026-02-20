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

			if (PIOMP.Room.IsInRoom && !PIOMP.Room.IsHost)
			{
				ClientSend.ReqEndTurn();
				return;
			}
			
			NextTurn();
			
			GD.Print(GameLogic.DamageManager.PreviouslyDamagedRoom.RoomType);
			if (PIOMP.Room.IsHost) ServerSend.EndTurn(GameStart.PlayerId, GameLogic.DamageManager.PreviouslyDamagedRoom.RoomType);
			//else if (PIOMP.Room.IsInRoom) ClientSend.ReqEndTurn();
        }

		public static void NextTurn()
		{
			IUndoableAction action;
			if (GameLogic.TurnQueue.CurrentPawn.Type == PawnType.Zambuko) action = new ZambukoEndTurn();
			else action = new EndTurn();
			action.Do();
			ActionLibrary.AddAction(action);
		}
		
		public static void NextTurn(Room damagedRoom)
		{
			IUndoableAction action;
			if (GameLogic.TurnQueue.CurrentPawn.Type == PawnType.Zambuko)
			{
				ZambukoEndTurn endTurn = new ZambukoEndTurn();
				endTurn.DamagedRoom = damagedRoom;
				action = endTurn;
			}
			else
			{
				EndTurn endTurn = new();
				endTurn.DamagedRoom = damagedRoom;
				action = endTurn;
			}

			action.Do();
			ActionLibrary.AddAction(action);
		}

		public static void Undo()
		{
			ActionLibrary.UndoAction();
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
            
            Undo();
            undoAudioPlayer?.Play();
            
            if (PIOMP.Room.IsHost) ServerSend.Undo(GameStart.PlayerId);
            else if (PIOMP.Room.IsInRoom) ClientSend.ReqUndo();
        }
	} 
}
