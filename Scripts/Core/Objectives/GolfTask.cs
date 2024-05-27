namespace Stardust
{
    public class GolfTask : MarsTask
    {
        public GolfTask(RoomType _roomType) : base(_roomType)
        {
            room = GameLogic.RoomManager.GetRoomByType(_roomType);

            Description = $"Get Objectives to ${_roomType}";
        }

        Room room;

        public override void Activate()
        {
            Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
            lander.AddItem(new Item(ItemType.Objective));

            room.OnItemDrop += OnRoomGetItem;
        }

        public override void UndoActivate()
        {
            throw new System.NotImplementedException();
        }

        void OnRoomGetItem()
        {
            foreach (Item item in room.Items)
            {
                if (item.Type == ItemType.Objective)
                {
                    room.RemoveItem(item);

                    room.OnItemDrop -= OnRoomGetItem;

                    Complete();

                    return;
                }
            }
        }
    }
}