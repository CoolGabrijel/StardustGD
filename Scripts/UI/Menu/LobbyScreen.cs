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
		[Export] private Button firstStepsExpansionBtn;

		private List<PlayerSlot> vacantSlots = new();
		private Tween moveTween;
		public static Lobby.LobbyPlayer localPlayer;
		private RandomNumberGenerator rng = new();

        public override void _Ready()
        {
			PIOMP.Room.OnNewClientJoined += OnPlayerJoined;
			PIOMP.Room.OnClientLeft += OnPlayerLeft;
        }

        public override void _Process(double delta)
		{
			ValidateReadyButton();

			if (StardustGameConfig.CurrentConfig != null)
			{
				firstStepsExpansionBtn.SetPressedNoSignal(StardustGameConfig.CurrentConfig.FirstStepsEnabled);
			}
		}

        public void OpenSingleplayerScreen()
        {
	        StardustGameConfig.CurrentConfig = new();
			GameStart.PlayerId = 1;
			Lobby = new Lobby(false);
			readyBtn.Text = "Start";
			readyBtn.ButtonPressed = false;
			Show();
		}

		public void OpenMultiplayerScreen()
		{
			StardustGameConfig.CurrentConfig = new();
			readyBtn.Text = Lobby.IsHost ? "Start" : "Ready";
			readyBtn.ButtonPressed = false;
			
			Show();
		}

		public void Reset(bool multiplayer)
        {
			if (Lobby != null)
            {
                Lobby.OnPlayerChanged -= OnPlayerChanged;
            }

            Lobby = new Lobby(multiplayer);
            Lobby.OnPlayerChanged += OnPlayerChanged;

            vacantSlots.Clear();
            vacantSlots.AddRange(playerSlots);
            vacantSlots.Remove(localSlot);
        }

		private void OnPlayerJoined(PIOMP.RoomClient client)
		{
			if (client.Id == PIOMP.Room.PlayerId)
            {
                GameStart.PlayerId = PIOMP.Room.PlayerId;
                localPlayer = new(PIOMP.Room.PlayerId);
                Lobby.AddPlayer(localPlayer);
                localSlot.SetPlayer(localPlayer);
            }
			else
            {
				PlayerSlot slot = vacantSlots[0];
				vacantSlots.RemoveAt(0);
                Lobby.LobbyPlayer player = new(client.Id);
				player.SetCharacter(client.GetData("Character"));
				bool ready = bool.Parse(client.GetData("Ready"));
				GD.Print($"LobbyScreen :: {client.Id}: {ready}");
                player.SetReady(ready);
                Lobby.AddPlayer(player);
                slot.SetPlayer(player);
            }

            GD.Print($"LobbyScreen :: Player {client.Id} joined");
        }

		private void OnPlayerLeft(PIOMP.RoomClient client)
        {
			foreach (PlayerSlot slot in playerSlots)
			{
				if (slot.LobbyPlayer.PlayerId == client.Id)
				{
					slot.Reset();
					vacantSlots.Insert(0, slot);
					break;
				}
			}

            GD.Print($"LobbyScreen :: Player {client.Id} left");
        }

		private void OnPlayerChanged(Lobby.LobbyPlayer player)
		{
			foreach (PlayerSlot slot in playerSlots)
			{
				if (slot.LobbyPlayer == player)
				{
					slot.OnCharacterChanged(player.CharacterName);
					return;
				}
			}
		}
		
		private void OnReadyClicked()
		{
			//GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
			if (readyBtn.ButtonPressed)
				MusicController.Instance.OnPlayerReady();
			
			//OnReady?.Invoke(readyBtn.ButtonPressed);

			List<PawnType> pawnPool = Enum.GetValues(typeof(PawnType)).Cast<PawnType>().ToList();
			List<PawnType> pawnsToSpawn = new();
			Dictionary<int, PawnType> players = new();
			
			foreach (Lobby.LobbyPlayer player in Lobby.Players)
			{
				if (player.CharacterName == "Random") continue;
				
				PawnType.TryParse(player.CharacterName, out PawnType pawnType);
				pawnsToSpawn.Add(pawnType);
				pawnPool.Remove(pawnType);

                if (Lobby.IsMultiplayer && Lobby.IsHost)
                {
					players.Add(player.PlayerId, pawnType);
                }
            }
			
			foreach (Lobby.LobbyPlayer player in Lobby.Players)
			{
				if (player.CharacterName != "Random") continue;
				
				PawnType randType = pawnPool[rng.RandiRange(0, pawnPool.Count-1)];
				
				pawnsToSpawn.Add(randType);
				pawnPool.Remove(randType);

				if (Lobby.IsMultiplayer && Lobby.IsHost)
                {
					players.Add(player.PlayerId, randType);
                }
			}

			foreach (PawnType pawn in pawnsToSpawn)
			{
				GD.Print("LobbyScreen :: OnReadyClicked " + pawn);
			}

			GameStart.PawnsToSpawn = pawnsToSpawn.ToArray();
			GameStart.PlayerList = players;
			
			if (!Lobby.IsMultiplayer) GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
			else
			{
				localPlayer.SetReady(readyBtn.ButtonPressed);

				if (!Lobby.IsHost) ClientSend.SetReady(readyBtn.ButtonPressed);
				else
				{
                    GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
                }
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

		private void OnFirstStepsToggled()
		{
			StardustGameConfig.CurrentConfig.FirstStepsEnabled = firstStepsExpansionBtn.ButtonPressed;
			
			if (PIOMP.Room.IsHost)
			{
				ServerSend.SetConfigVar("FirstSteps", StardustGameConfig.CurrentConfig.FirstStepsEnabled.ToString());
			}
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
			
			if (PIOMP.Room.IsInRoom)
			{
				PIOMP.Room.LeaveRoom();
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

		public override void _ExitTree()
		{
			PIOMP.Room.OnNewClientJoined -= OnPlayerJoined;
			PIOMP.Room.OnClientLeft -= OnPlayerLeft;
		}
	}
}
