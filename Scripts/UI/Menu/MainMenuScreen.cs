using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class MainMenuScreen : Control
	{
		public static MainMenuScreen Instance { get; private set; }

		[Export] LobbyScreen LobbyScreen { get; set; }
		[Export] Control MenuButtons { get; set; }
		[Export] Control BoxCover { get; set; }

        public override void _Ready()
        {
			Instance = this;
			ShowMainMenu();
        }

		public void ShowMainMenu()
        {
            MenuButtons.Show();
            BoxCover.Show();
            LobbyScreen.Hide();
        }

        public void SinglePlayerPressed()
        {
            MenuButtons.Hide();
            BoxCover.Hide();
			LobbyScreen.OpenSingleplayerScreen();
			MusicController.Instance.PlayLobbyMusic();
        }

		public void MultiPlayerPressed()
		{
			MenuButtons.Hide();
			BoxCover.Hide();
			LobbyScreen.OpenMultiplayerScreen();
			MusicController.Instance.PlayLobbyMusic();
		}

		public void ExitPressed()
		{
			GetTree().Quit();
		}
	} 
}
