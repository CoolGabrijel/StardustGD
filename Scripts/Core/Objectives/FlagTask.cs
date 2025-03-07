namespace Stardust
{
    public class FlagTask : MarsTask
    {
        public FlagTask(RoomType _roomType) : base(_roomType)
        {
            Description = "Plant a Flag on $Peak";
        }

        Item flag = new(ItemType.Flag);
        Room peak = GameLogic.RoomManager.GetRoomByType(RoomType.Peak);
        Pawn[] pawns;

        public override void Activate()
        {
            pawns = GameLogic.TurnQueue.Pawns;
            Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
            lander.AddItem(flag);

            foreach (Pawn pawn in pawns)
            {
                pawn.OnItemDropped += OnItemDropped;
            }
        }

        public override void UndoActivate()
        {
            foreach (Room room in GameLogic.RoomManager.Rooms)
            {
                if (room.GetItem(ItemType.Flag) != null)
                {
                    room.RemoveItem(flag);
                    foreach (Pawn pawn in pawns)
                    {
                        pawn.OnItemDropped -= OnItemDropped;
                    }
                }
            }
        }

        void OnItemDropped(Pawn dropper, Item item)
        {
            if (item.Type != ItemType.Flag && dropper.Room.RoomType != RoomType.Peak) return;

            peak.RemoveItem(item);

            foreach (Pawn pawn in pawns)
            {
                pawn.OnItemDropped -= OnItemDropped;
                pawn.OnItemPickedUp += OnItemPickedUp;
            }

            Complete();
        }

        void OnItemPickedUp(Item item)
        {
            if (item.Type != ItemType.Flag) return;

            foreach (Pawn pawn in pawns)
            {
                pawn.OnItemPickedUp -= OnItemPickedUp;
                pawn.OnItemDropped += OnItemDropped;
            }

            UndoComplete();
        }

        void OnItemDropped()
        {
            Item flag = peak.GetItem(ItemType.Flag);

            if (flag == null) return;

            peak.RemoveItem(flag);
            Complete();
        }
    }
}