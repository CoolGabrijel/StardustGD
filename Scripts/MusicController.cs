using Godot;
using System;

namespace Stardust.Godot
{
	public partial class MusicController : Node
	{
		public static MusicController Instance { get; private set; }

		[Export] AudioStreamPlayer player;
		[Export] AudioStream menuMusic;
		[Export] AudioStream lobbyMusic;
		[Export] AudioStream readyMusic;

        public override void _Ready()
        {
			Instance = this;
			player.Stream = lobbyMusic;
			player.Play();
        }

		public void OnPlayerReady()
		{
			if (player.Stream == readyMusic) return;

			player.Stream = readyMusic;
			player.Play();
            player.Finished += LoopToLobbyMusic;
		}

        private void LoopToLobbyMusic()
        {
            player.Stream = lobbyMusic;
            player.Play();
			player.Finished -= LoopToLobbyMusic;
        }
    } 
}
