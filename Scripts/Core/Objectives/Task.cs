namespace Stardust
{
    public abstract class Task
    {
        public Task(RoomType _roomType)
        {
            Description = GenerateDescription(_roomType);
            Room = GameLogic.RoomManager.GetRoomByType(_roomType);
        }

        public event System.Action StatusChange;

        public Room Room { get; private set; }
        public abstract string Tag { get; }
        public string Description { get; internal set; }

        public bool Completed;

        public virtual void Complete()
        {
            Completed = true;

            StatusChange?.Invoke();
        }

        public virtual void UndoComplete()
        {
            Completed = false;

            StatusChange?.Invoke();
        }

        public abstract void Activate();
        public abstract void UndoActivate();

        static string GenerateDescription(RoomType _type)
        {
            string result = "Do Objective on $";
            switch (_type)
            {
                case RoomType.Workshop:
                    result += "Workshop";
                    break;
                case RoomType.SolarPanels:
                    result += "Solar panels";
                    break;
                case RoomType.LifeSupport:
                    result += "Life Support";
                    break;
                case RoomType.Airlock:
                    result += "Airlock";
                    break;
                case RoomType.Cockpit:
                    result += "Cockpit";
                    break;
                case RoomType.Comms:
                    result += "Comms";
                    break;
                case RoomType.Habitation:
                    result += "Habitation";
                    break;
                case RoomType.Engines:
                    result += "Engine";
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}