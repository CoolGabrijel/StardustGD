using System.Collections.Generic;

namespace Stardust;

public class Lobby
{
	public Lobby(bool multiplayer)
	{
		IsMultiplayer = multiplayer;
	}
	
	public event System.Action OnPlayerAdded;
	
	public bool IsMultiplayer { get; private set; }
	public bool IsHost { get; private set; }

	public List<LobbyPlayer> Players { get; private set; } = new();

	public void AddPlayer(LobbyPlayer player)
	{
		Players.Add(player);
		OnPlayerAdded?.Invoke();
	}

	public void RemovePlayer(LobbyPlayer player)
	{
		Players.Remove(player);
		OnPlayerAdded?.Invoke();
	}

	public void Ready(int id, bool ready)
	{
		foreach (LobbyPlayer player in Players)
		{
			if (player.PlayerId == id) player.SetReady(ready);
		}
	}
	
	public class LobbyPlayer
	{
		public LobbyPlayer(int id)
		{
			PlayerId = id;
			CharacterName = "Random";
			Ready = false;
		}
		
		public event System.Action<bool> OnReady;
		
		public int PlayerId { get; private set; }
		public string CharacterName { get; private set; }
		public bool Ready { get; private set; }

		public void SetReady(bool ready)
		{
			Ready = ready;
			OnReady?.Invoke(ready);
		}

		public void SetCharacter(string characterName)
		{
			CharacterName = characterName;
		}
	}
}
