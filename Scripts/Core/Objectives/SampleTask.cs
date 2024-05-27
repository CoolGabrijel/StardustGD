namespace Stardust
{
    public class SampleTask : MarsTask
    {
        public SampleTask(RoomType _roomType) : base(_roomType)
        {
            roomType = _roomType;

            Description = "Gather samples to the $Lander";

            lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
        }

        RoomType roomType;
        Room lander;

        public override void Activate()
        {
            Room room = GameLogic.RoomManager.GetRoomByType(roomType);

            room.AddItem(new Item(ItemType.Sample));
            room.AddItem(new Item(ItemType.Sample));

            lander.OnItemDrop += OnLanderGetItem;
        }

        public override void UndoActivate()
        {
            throw new System.NotImplementedException();
        }

        void OnLanderGetItem()
        {
            int sampleAmount = 0;
            foreach (Item item in lander.Items)
            {
                if (item.Type == ItemType.Sample) sampleAmount++;
            }

            if (sampleAmount >= 2)
            {
                Item sample1 = lander.GetItem(ItemType.Sample);
                lander.RemoveItem(sample1);
                Item sample2 = lander.GetItem(ItemType.Sample);
                lander.RemoveItem(sample2);

                Room room = GameLogic.RoomManager.GetRoomByType(roomType);
                lander.OnItemDrop -= OnLanderGetItem;

                Complete();
            }
        }
    }
}