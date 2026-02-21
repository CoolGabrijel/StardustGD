using Godot;
using PIOMP;
using PlayerIOClient;
using Stardust.Actions;

namespace Stardust.Godot
{
	public static partial class ServerHandle
    {
        [MessageHandler("ReqCharChange")]
        public static void ReceiveChangeChar(Message _msg)
        {
            int id = _msg.GetInt(0);
            string newChar = _msg.GetString(1);

            UI.LobbyScreen.Lobby.ChangePlayer(id, newChar);

            ServerSend.ChangeChar(id, newChar);
        }

        [MessageHandler("SetReady")]
        public static void GetReady(Message _msg)
        {
            int id = _msg.GetInt(0);
            bool ready = _msg.GetBoolean(1);

            UI.LobbyScreen.Lobby.Ready(id, ready);

            ServerSend.SetReady(id, ready);
        }
        
        [MessageHandler("ReqEndTurn")]
        public static void HandleEndTurn(Message _msg)
        {
            int id = _msg.GetInt(0);

            UI.TurnButtons.NextTurn();

            Room damagedRoom = GameLogic.DamageManager.PreviouslyDamagedRoom;

            ServerSend.EndTurn(id, damagedRoom.RoomType);
        }
        
        [MessageHandler("ReqUndo")]
        public static void HandleUndo(Message _msg)
        {
            int id = _msg.GetInt(0);

            UI.TurnButtons.Undo();

            ServerSend.Undo(id);
        }
        
        [MessageHandler("ReqMove")]
        public static void ReceiveMove(Message _msg)
        {
            uint msgIndex = 0;
            int id = _msg.GetInt(msgIndex++);
            
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
            
            ServerSend.Move(id, pawnType, cost, fromType, toType, movDir);
        }
        
        [MessageHandler("ReqActivateRoom")]
        public static void ReceiveActivateRoom(Message _msg)
        {
            int id = _msg.GetInt(0);

            Pawn pawn = GameLogic.TurnQueue.CurrentPawn;
            pawn.Room.ActivateAbility(pawn);

            if (pawn.Room.RoomType == RoomType.Habitation) id = GameStart.PlayerId;
            ServerSend.ActivateRoom(id, GameLogic.DamageManager.PreviouslyDamagedRoom.RoomType);
        }
        
        [MessageHandler("ReqPickup")]
        public static void ReceivePickup(Message _msg)
        {
            int id = _msg.GetInt(0);
            ItemType itemType = (ItemType)_msg.GetInt(1);

            Pawn pawn = GameLogic.TurnQueue.CurrentPawn;
            Item item = pawn.Room.GetItem(itemType);
            PickUpPart pickupAction = new(pawn, pawn.Room, item);
            pickupAction.Do();
            ActionLibrary.AddAction(pickupAction);

            ServerSend.Pickup(id, itemType);
        }
        
        [MessageHandler("ReqDrop")]
        public static void ReceiveDrop(Message _msg)
        {
            int id = _msg.GetInt(0);
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
            
            ServerSend.Drop(id, itemType);
        }
        
        [MessageHandler("ReqRepair")]
        public static void ReceiveRepair(Message _msg)
        {
            int id = _msg.GetInt(0);

            Pawn pawn = GameLogic.TurnQueue.CurrentPawn;
            new RepairRoom(pawn.Room, pawn).Do();

            ServerSend.Repair(id);
        }
        
        [MessageHandler("ReqCompleteTask")]
        public static void ReceiveCompleteTask(Message _msg)
        {
            int id = _msg.GetInt(0);
            
            Pawn pawn = GameLogic.TurnQueue.CurrentPawn;
            IUndoableAction action = null;
            if (ObjectiveHandler.CurrentObjective.Tasks[0].Tag == "FirstSteps")
            {
                action = new CompleteMarsTask(pawn);
            }
            else
            {
                action = new CompleteBaseTask(pawn.Room.RoomType);
            }

            action.Do();
            ActionLibrary.AddAction(action);
            
            ServerSend.CompleteTask(id);

            ObjectiveHandler.CheckAllObjectivesCompleted();
        }
    } 
}
