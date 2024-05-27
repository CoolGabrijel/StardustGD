namespace Stardust
{
    public class FlagTask : MarsTask
    {
        public FlagTask(RoomType _roomType) : base(_roomType)
        {
            Description = "Plant a Flag on $Peak";
        }

        Item flag;

        public override void Activate()
        {
            Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
            flag = new Item(ItemType.Flag);
            lander.AddItem(flag);

            Room peak = GameLogic.RoomManager.GetRoomByType(RoomType.Peak);
            peak.OnItemDrop += OnItemDropped;
        }

        public override void UndoActivate()
        {
            foreach (Room room in GameLogic.RoomManager.Rooms)
            {
                if (room.GetItem(ItemType.Flag) != null)
                {
                    room.RemoveItem(flag);
                    Room peak = GameLogic.RoomManager.GetRoomByType(RoomType.Peak);
                    peak.OnItemDrop -= OnItemDropped;
                }
            }
        }

        void OnItemDropped()
        {
            Room peak = GameLogic.RoomManager.GetRoomByType(RoomType.Peak);
            Item flag = peak.GetItem(ItemType.Flag);

            if (flag == null) return;

            peak.RemoveItem(flag);
            Complete();
        }

        //void OnFlagPlanted(Item _flag)
        //{
        //    Room peak = GameLogic.RoomManager.GetRoomByType(RoomType.Peak);
        //    peak.OnFlagDrop -= OnFlagPlanted;
        //    peak.RemoveItem(_flag);
        //    Complete();
        //}
    }
}