using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Godot
{
    public partial class GameStart : Node
    {
        public static Pawn LocalPlayer;
        public static int PlayerId;
        public static PawnType[] PawnsToSpawn;
        public static RoomType[] RoomsToSpawn;
        public static string[] ObjectivesToSpawn;
        public static Dictionary<int, PawnType> PlayerList;

        [Export] private RoomGen2D roomGen;
        [Export] private Node pawnParent;
        [Export] private PackedScene pawnPrefab;

        private static Dictionary<Pawn, Pawn2D> pawnToGraphic;

        public override void _Ready()
        {
            StardustGameConfig config = new()
            {
                // For testing purposes feel free to place anything in here
                DamageDisabled = false,
                FirstStepsEnabled = false,
            };
            StardustGameConfig.CurrentConfig = config;

            if (PawnsToSpawn == null)
            {
                PawnsToSpawn = new PawnType[]
                {
                    PawnType.Concorde,
                    PawnType.Aurora,
                };
            }

            pawnToGraphic = new();

            if (RoomsToSpawn != null)
            {
                if (ObjectivesToSpawn != null)
                    GameLogic.BeginGame(PawnsToSpawn, RoomsToSpawn, ObjectivesToSpawn);
                else
                    GameLogic.BeginGame(PawnsToSpawn, RoomsToSpawn);
            }
            else
                GameLogic.BeginGame(PawnsToSpawn);

            roomGen.Generate(GameLogic.RoomManager);

            foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
            {
                SpawnPawnGraphic(pawn);
            }

            if (UI.LobbyScreen.Lobby.IsMultiplayer)
            {
                if (PlayerList != null)
                {
                    foreach (var player in PlayerList)
                    {
                        if (player.Key == PlayerId)
                        {
                            foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
                            {
                                if (pawn.Type == player.Value)
                                {
                                    LocalPlayer = pawn;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                if (UI.LobbyScreen.Lobby.IsHost)
                {
                    ServerSend.StartGame();
                }
            }
            else
            {
                LocalPlayer = GameLogic.TurnQueue.CurrentPawn;
            }
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("ActivateRoom"))
            {
                if (LocalPlayer.Room.CanBeActivated)
                {
                    bool canBeActivated = LocalPlayer.Room.CanBeActivated;
                    bool hasEnergy = GameLogic.EnergyExpended < LocalPlayer.EnergyCards.Where((e) => !e.Exhausted).Max((e) => e.Energy);

                    if (!canBeActivated || !hasEnergy) return;

                    if (LocalPlayer.Room.RoomType == RoomType.Habitation && PIOMP.Room.IsInRoom && !PIOMP.Room.IsHost)
                    {
                        ClientSend.ReqActivateRoom();
                        return;
                    }

                    LocalPlayer.Room.ActivateAbility(LocalPlayer);
                    
                    if (PIOMP.Room.IsHost) ServerSend.ActivateRoom(PlayerId, GameLogic.DamageManager.PreviouslyDamagedRoom.RoomType);
                    else if (PIOMP.Room.IsInRoom) ClientSend.ReqActivateRoom();
                }
            }
            if (Input.IsActionJustPressed("NextTurn"))
            {
                UI.TurnButtons.AttemptNextTurn();
            }

            if (!UI.LobbyScreen.Lobby.IsMultiplayer)
            {
                LocalPlayer = GameLogic.TurnQueue.CurrentPawn; // TODO: Refactor.
            }
        }

        public static Pawn2D GetPawnGraphic(Pawn pawn)
        {
            return pawnToGraphic[pawn];
        }

        private void SpawnPawnGraphic(Pawn pawn)
        {
            Pawn2D graphic = pawnPrefab.Instantiate<Pawn2D>();
            pawnParent.AddChild(graphic);
            graphic.SetPawn(pawn);
            graphic.RoomGen2D = roomGen;

            RoomType spawnRoom = RoomType.Cockpit;

            switch (pawn.Type)
            {
                case PawnType.Concorde:
                    break;
                case PawnType.Aurora:
                    spawnRoom = RoomType.Engines;
                    break;
                case PawnType.Zambuko:
                    spawnRoom = RoomType.LifeSupport;
                    break;
                case PawnType.Rosetta:
                    spawnRoom = RoomType.Airlock;
                    break;
                case PawnType.Wolfram:
                    spawnRoom = RoomType.Habitation;
                    break;
                default:
                    break;
            }

            graphic.Pawn.MoveTo(GameLogic.RoomManager.GetRoomByType(spawnRoom));
            pawnToGraphic.Add(pawn, graphic);
        }
    } 
}
