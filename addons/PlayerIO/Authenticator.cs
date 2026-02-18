using Godot;
using PlayerIOClient;

namespace PIOMP
{
	public static class Authenticator
    {
        public static event System.Action Authenticated;

        public static void Authenticate()
        {
            PlayerIO.Authenticate("sdu-rskj8h7ak0msezy5wjio5a", "public", new() { { "userId", "GuestUser" } }, null, OnAuthenticate, OnFailed);
        }

        public static void OnFailed(PlayerIOError _error)
        {
            GD.PrintErr("PIOMP :: Failed to Authenticate to PlayerIO");
            GD.PrintErr(_error);
        }

        static void OnAuthenticate(Client _client)
        {
            MultiplayerUtils.Client = _client;
            Authenticated?.Invoke();
            GD.Print("PIOMP :: Authenticated to PlayerIO");
        }
    } 
}
