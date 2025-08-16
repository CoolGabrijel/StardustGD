using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Godot.UI
{
	public partial class LobbyScreen : Control
	{
		public static event Action<bool> OnReady;
		
		public static Lobby Lobby { get; private set; }
		
		[Export] private Button backBtn;
		[Export] private Button readyBtn;
		[Export] private Button[] lobbyBtns;
		[Export] private PlayerSlot[] playerSlots;
		[Export] private PlayerSlot localSlot;
		
		private Tween moveTween;
		private Lobby.LobbyPlayer localPlayer;
		private RandomNumberGenerator rng = new();

		public override void _Process(double delta)
		{
			ValidateReadyButton();
		}

		public void OpenSingleplayerScreen()
		{
			GameStart.PlayerId = 1;
			Lobby = new Lobby(false);
			readyBtn.Text = "Start";
			readyBtn.ButtonPressed = false;
			Show();
		}

		public void OpenMultiplayerScreen()
		{
			GameStart.PlayerId = 1;
			Lobby = new Lobby(true);
			int iterator = 2;

			foreach (PlayerSlot slot in playerSlots)
			{
				if (slot.IsLocal)
				{
					Lobby.LobbyPlayer player = new(1);
					Lobby.AddPlayer(player);
					slot.SetPlayer(player);
					localPlayer = player;
				}
				else
				{
					Lobby.LobbyPlayer player = new(iterator++);
					Lobby.AddPlayer(player);
					slot.SetPlayer(player);
				}
			}
			
			readyBtn.Text = Lobby.IsHost ? "Start" : "Ready";
			readyBtn.ButtonPressed = false;
			
			Show();
		}
		
		private void OnReadyClicked()
		{
			//GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
			if (readyBtn.ButtonPressed)
				MusicController.Instance.OnPlayerReady();
			
			//OnReady?.Invoke(readyBtn.ButtonPressed);

			List<PawnType> pawnPool = Enum.GetValues(typeof(PawnType)).Cast<PawnType>().ToList();
			List<PawnType> pawnsToSpawn = new();
			
			foreach (Lobby.LobbyPlayer player in Lobby.Players)
			{
				if (player.CharacterName == "Random") continue;
				
				PawnType.TryParse(player.CharacterName, out PawnType pawnType);
				pawnsToSpawn.Add(pawnType);
				pawnPool.Remove(pawnType);
			}
			
			foreach (Lobby.LobbyPlayer player in Lobby.Players)
			{
				if (player.CharacterName != "Random") continue;
				
				PawnType randType = pawnPool[rng.RandiRange(0, pawnPool.Count-1)];
				
				pawnsToSpawn.Add(randType);
				pawnPool.Remove(randType);
			}

			foreach (PawnType pawn in pawnsToSpawn)
			{
				GD.Print("LobbyScreen::OnReadyClicked " + pawn);
			}

			GameStart.PawnsToSpawn = pawnsToSpawn.ToArray();
			
			if (!Lobby.IsMultiplayer) GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
			else
			{
				localPlayer.SetReady(readyBtn.ButtonPressed);
			}
		}
		
		private void OnSettingsClicked()
		{
			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", new Vector2(0, -450), .25f);

			foreach (Button btn in lobbyBtns)
			{
				btn.Hide();
			}
			backBtn.Show();
		}

		private void OnBackClicked()
		{
			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", Vector2.Zero, .25f);

			foreach (Button btn in lobbyBtns)
			{
				btn.Show();
			}
			backBtn.Hide();
		}

		private void OnLeaveClicked()
		{
			foreach (PlayerSlot slot in playerSlots)
			{
				slot.Reset();
			}
			
			MainMenuScreen.Instance.ShowMainMenu();
		}

		private void ValidateReadyButton()
		{
			if (Lobby == null || Lobby.Players.Count <= 0)
			{
				readyBtn.TooltipText = "You must select at least one character.\nIf you already did, then something went completely horribly wrong.";
				readyBtn.Disabled = true;
				return;
			}

			readyBtn.TooltipText = "Click when ready.";
			readyBtn.Disabled = false;
			List<string> charTypes = new();

			foreach (Lobby.LobbyPlayer player in Lobby.Players)
			{
				if (player.CharacterName == "Random") continue;

				if (charTypes.Contains(player.CharacterName))
				{
					readyBtn.TooltipText = "Possible duplicate characters detected. Unable to start game.";
					readyBtn.Disabled = true;
					break;
				}

				charTypes.Add(player.CharacterName);
			}
		}
	}
}
