using Godot;
using PIOMP;
using PlayerIOClient;
using System.Collections.Generic;

namespace Stardust.Godot.UI
{
	public partial class OnlineMenu : Control
	{
		[Export] private LobbyScreen lobbyScreen;
		[Export] private Control authWidget;
		[Export] private Label authLabel;
		[Export] private LineEdit lobbyNameField;
		[Export] private Button roomButtonPrefab;
		[Export] private Control roomContainer;

        public override void _Ready()
        {
			roomContainer.RemoveChild(roomButtonPrefab);
            VisibilityChanged += OnVisibilityChanged;
            PIOMP.Room.OnRoomJoined += OnRoomJoined;
			Authenticator.Authenticated += OnAuthenticated;
        }

        public override void _PhysicsProcess(double delta)
        {
			authWidget.Visible = !MultiplayerUtils.HasAuthenticated;
        }

        private void OnVisibilityChanged()
        {
			roomContainer.FreeChildren();

			if (!MultiplayerUtils.HasAuthenticated) return;

			RoomInfo[] rooms = MultiplayerUtils.Client.Multiplayer.ListRooms(null, null, 10, 0);

			foreach (RoomInfo room in rooms)
			{
				string roomName = "New Lobby";
				if (room.RoomData.TryGetValue("LobbyName", out string name))
				{
					roomName = name;
				}

				Button roomButton = roomButtonPrefab.Duplicate() as Button;
				roomButton.GetNode<Label>("LobbyName").Text = roomName;
				roomButton.Pressed += () => JoinRoom(room.Id);
				roomContainer.AddChild(roomButton);
			}
        }

        public void CreateLobby()
		{
			string lobbyName = "New Lobby";
			if (!string.IsNullOrEmpty(lobbyNameField.Text))
			{
				lobbyName = lobbyNameField.Text;
			}

			Dictionary<string, string> roomData = new();
			roomData.Add("LobbyName", lobbyName);
			Dictionary<string, string> joinData = new()
			{
				{"Character", "Random" },
                {"Ready", "False" }
            };

			lobbyScreen.Reset(true);
			PIOMP.Room.CreateRoom(roomData, joinData);
		}

		private void JoinRoom(string id)
        {
            lobbyScreen.Reset(true);
            Dictionary<string, string> joinData = new()
            {
                {"Character", "Random" },
				{"Ready", "False" }
            };
            PIOMP.Room.JoinRoom(id, joinData);
        }

        public void OpenMenu()
		{
			if (!MultiplayerUtils.HasAuthenticated)
			{
				authWidget.Show();
				Authenticator.Authenticate();
			}
			else
			{
				authWidget.Hide();
			}

			Show();
		}

		private void OpenLobby()
        {
            Hide();
            lobbyScreen.OpenMultiplayerScreen();
        }

		private void CloseMenu()
		{
			MainMenuScreen.Instance.ShowMainMenu();
		}

		private void OnRoomJoined()
		{
			CallDeferred("OpenLobby");
		}

		private void OnAuthenticated()
		{
			CallDeferred("OnVisibilityChanged");
		}

		public override void _ExitTree()
		{
			PIOMP.Room.OnRoomJoined -= OnRoomJoined;
			Authenticator.Authenticated -= OnAuthenticated;
		}
	}
}
