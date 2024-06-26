using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot
{
    public partial class GameStart : Node
    {
        public static Pawn LocalPlayer;

        [Export] private RoomGen2D roomGen;
        [Export] private Node pawnParent;
        [Export] private PackedScene pawnPrefab;

        public override void _Ready()
        {
            GameLogic.BeginGame();
            roomGen.Generate(GameLogic.RoomManager);

            foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
            {
                SpawnPawnGraphic(pawn);
            }

            LocalPlayer = GameLogic.TurnQueue.CurrentPawn;
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("Undo"))
            {
                ActionLibrary.UndoAction();
            }
            if (Input.IsActionJustPressed("ActivateRoom"))
            {
                if (LocalPlayer.Room.CanBeActivated) LocalPlayer.Room.ActivateAbility(LocalPlayer);
            }
            if (Input.IsActionJustPressed("NextTurn"))
            {
                //GameLogic.EndTurn();
                new EndTurn().Do();
            }

            LocalPlayer = GameLogic.TurnQueue.CurrentPawn; // TODO: Delete later.
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
        }
    } 
}
