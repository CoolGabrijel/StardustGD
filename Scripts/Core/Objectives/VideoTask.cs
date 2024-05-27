namespace Stardust
{
    public class VideoTask : MarsTask
    {
        public VideoTask(RoomType _roomType) : base(_roomType)
        {
            room = GameLogic.RoomManager.GetRoomByType(_roomType);

            Description = $"Get Objectives to ${_roomType}";
        }

        Room room;

        public override void Activate()
        {
            Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
            lander.AddItem(new Item(ItemType.Objective));
            lander.AddItem(new Item(ItemType.Objective));

            room.OnItemDrop += OnRoomGetItem;
        }

        public override void UndoActivate()
        {
            throw new System.NotImplementedException();
        }

        void OnRoomGetItem()
        {
            int sampleAmount = 0;
            foreach (Item item in room.Items)
            {
                if (item.Type == ItemType.Objective) sampleAmount++;
            }

            if (sampleAmount >= 2)
            {
                Item sample1 = room.GetItem(ItemType.Objective);
                room.RemoveItem(sample1);
                Item sample2 = room.GetItem(ItemType.Objective);
                room.RemoveItem(sample2);

                room.OnItemDrop -= OnRoomGetItem;

                Complete();
            }
        }
    }
}