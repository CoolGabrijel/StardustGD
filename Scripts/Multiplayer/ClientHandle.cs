using Godot;
using PIOMP;
using PlayerIOClient;
using System.Collections.Generic;
using Stardust.Actions;

namespace Stardust.Godot
{
    public static partial class ClientHandle
    {
        [MessageHandler("ChangeChar")]
        public static void ReceiveCharChange(Message _msg)
        {
            int id = _msg.GetInt(0);
            string charName = _msg.GetString(1);

            UI.LobbyScreen.Lobby.ChangePlayer(id, charName);
        }


        [MessageHandler("SendReady")]
        public static void GetReady(Message _msg)
        {
            int id = _msg.GetInt(0);
            bool ready = _msg.GetBoolean(1);

            UI.LobbyScreen.Lobby.Ready(id, ready);
        }

        [MessageHandler("StartGame")]
        public static void StartGame(Message _msg)
        {
            uint msgIndex = 0;
            int pawnCount = _msg.GetInt(msgIndex++);

            // Get the Players and Pawns

            PawnType[] pawns = new PawnType[pawnCount];
            Dictionary<int, PawnType> players = new();

            for (int i = 0; i < pawnCount; i++)
            {
                int id = _msg.GetInt(msgIndex++);
                PawnType type = (PawnType)_msg.GetInt(msgIndex++);
                pawns[i] = type;
                players.Add(id, type);
            }

            // Get the Rooms

            int roomCount = _msg.GetInt(msgIndex++);
            RoomType[] rooms = new RoomType[roomCount];

            for(int i = 0; i < roomCount; i++)
            {
                rooms[i] = (RoomType)_msg.GetInt(msgIndex++);
            }

            // Get the Objectives

            int objCount = _msg.GetInt(msgIndex++);
            string[] objectives = new string[objCount];

            for(int i = 0; i < objCount; i++)
            {
                objectives[i] = _msg.GetString(msgIndex++);
            }

            // Set it all up

            GameStart.PawnsToSpawn = pawns;
            GameStart.RoomsToSpawn = rooms;
            GameStart.ObjectivesToSpawn = objectives;
            GameStart.PlayerList = players;

            UI.MainMenuScreen.Instance.GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
        }
        
        [MessageHandler("EndTurn")]
        public static void ReceiveEndTurn(Message _msg)
        {
            RoomType damagedRoomType = (RoomType)_msg.GetInt(0);
            
            Room damagedRoom = GameLogic.RoomManager.GetRoomByType(damagedRoomType);
            
            UI.TurnButtons.NextTurn(damagedRoom);
        }
        
        [MessageHandler("Undo")]
        public static void ReceiveUndo(Message _msg)
        {
            UI.TurnButtons.Undo();
        }
        
        [MessageHandler("Move")]
        public static void ReceiveMove(Message _msg)
        {
            uint msgIndex = 0;
            PawnType pawnType = (PawnType)_msg.GetInt(msgIndex++);
            int cost =  _msg.GetInt(msgIndex++);
            RoomType toType = (RoomType)_msg.GetInt(msgIndex++);
            RoomType fromType = (RoomType)_msg.GetInt(msgIndex++);
            Direction movDir = (Direction)_msg.GetInt(msgIndex);

            Pawn pawn = null;
            foreach (Pawn turnQueuePawn in GameLogic.TurnQueue.Pawns)
            {
                if (pawnType == turnQueuePawn.Type)
                {
                    pawn = turnQueuePawn;
                    break;
                }
            }

            Room toRoom = GameLogic.RoomManager.GetRoomByType(toType);
            Room fromRoom = GameLogic.RoomManager.GetRoomByType(fromType);
            
            IUndoableAction action = new MoveAction(pawn, cost, fromRoom, toRoom, movDir);

            action.Do();
            ActionLibrary.AddAction(action);
        }
        
        [MessageHandler("ActivateRoom")]
        public static void ReceiveActivateRoom(Message _msg)
        {
            int id =  _msg.GetInt(0);

            Pawn currentPawn = GameLogic.TurnQueue.CurrentPawn;
            currentPawn.Room.ActivateAbility(currentPawn);
        }
        
        [MessageHandler("Pickup")]
        public static void ReceivePickup(Message _msg)
        {
            int id =  _msg.GetInt(0);
            ItemType itemType = (ItemType)_msg.GetInt(1);

            Pawn currentPawn = GameLogic.TurnQueue.CurrentPawn;
            Item item = currentPawn.Room.GetItem(itemType);
            PickUpPart pickupAction = new(currentPawn, currentPawn.Room, item);
            pickupAction.Do();
            ActionLibrary.AddAction(pickupAction);
        }
        
        [MessageHandler("Drop")]
        public static void ReceiveDrop(Message _msg)
        {
            int id =  _msg.GetInt(0);
            ItemType itemType = (ItemType)_msg.GetInt(1);

            Pawn pawn = GameLogic.TurnQueue.CurrentPawn;
            Item item = null;

            foreach (Item itemInInventory in pawn.Inventory)
            {
                if (itemInInventory.Type == itemType)
                {
                    item = itemInInventory;
                    break;
                }
            }
            
            IUndoableAction action = null;

            switch (item.Type)
            {
                case ItemType.Part:
                    action = new DropItem(pawn, pawn.Room, item);
                    break;
                case ItemType.Sample:
                    if (pawn.Room.RoomType == RoomType.Lander)
                        action = new CompleteMarsTask(pawn, item);
                    else
                        action = new DropItem(pawn, pawn.Room, item);
                    break;
                case ItemType.Flag:
                    if (pawn.Room.RoomType == RoomType.Peak)
                        action = new CompleteMarsTask(pawn, item);
                    else
                        action = new DropItem(pawn, pawn.Room, item);
                    break;
                case ItemType.Objective:
                    break;
                default:
                    break;
            }

            action.Do();
            ActionLibrary.AddAction(action);
        }
        
        [MessageHandler("Repair")]
        public static void ReceiveRepair(Message _msg)
        {
            int id =  _msg.GetInt(0);

            Pawn currentPawn = GameLogic.TurnQueue.CurrentPawn;
            new RepairRoom(currentPawn.Room, currentPawn).Do();
        }
    }
}
