namespace Stardust
{
    public class Objective
    {
        public Objective(Task[] _tasks)
        {
            Tasks = _tasks;
        }

        public string Name { get; set; }
        public readonly Task[] Tasks;
        public bool Completed
        {
            get
            {
                foreach (Task task in Tasks)
                {
                    if (!task.Completed) return false;
                }

                return true;
            }
        }

        public static Objective CreateObjective(ObjectiveHandler.ObjectiveCardType _type)
        {
            Task[] tasks = new Task[2];

            switch (_type)
            {
                case ObjectiveHandler.ObjectiveCardType.CourseCorrection:
                    tasks[0] = new BaseTask(RoomType.Cockpit);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                case ObjectiveHandler.ObjectiveCardType.CockpitUpdate:
                    tasks[0] = new BaseTask(RoomType.Cockpit);
                    tasks[1] = new BaseTask(RoomType.Comms);
                    break;
                case ObjectiveHandler.ObjectiveCardType.WorkshopUpdate:
                    tasks[0] = new BaseTask(RoomType.Workshop);
                    tasks[1] = new BaseTask(RoomType.Comms);
                    break;
                case ObjectiveHandler.ObjectiveCardType.AirFilters:
                    tasks[0] = new BaseTask(RoomType.LifeSupport);
                    tasks[1] = new BaseTask(RoomType.Airlock);
                    break;
                case ObjectiveHandler.ObjectiveCardType.DebrisFilters:
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Experiment:
                    tasks[0] = new BaseTask(RoomType.LifeSupport);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Pressure:
                    tasks[0] = new BaseTask(RoomType.Airlock);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                case ObjectiveHandler.ObjectiveCardType.ClearDebris:
                    tasks[0] = new BaseTask(RoomType.SolarPanels);
                    tasks[1] = new BaseTask(RoomType.SolarPanels);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Recalibrations:
                    tasks[0] = new BaseTask(RoomType.SolarPanels);
                    tasks[1] = new BaseTask(RoomType.Cockpit);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Laundry:
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.LifeSupport);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Recycling:
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case ObjectiveHandler.ObjectiveCardType.Diagnostics:
                    tasks[0] = new BaseTask(RoomType.Comms);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                default:
                    break;
            }

            Objective objective = new(tasks);
            objective.Name = _type.ToString();
            return objective;
        }

        public static Objective CreateObjective(ObjectiveHandler.MarsObjCardType _type)
        {
            Task[] tasks = new Task[1];

            switch (_type)
            {
                case ObjectiveHandler.MarsObjCardType.Flag:
                    tasks[0] = new FlagTask(RoomType.Lander);
                    break;
                case ObjectiveHandler.MarsObjCardType.GullySample:
                    tasks[0] = new SampleTask(RoomType.Gully);
                    break;
                case ObjectiveHandler.MarsObjCardType.CraterSample:
                    tasks[0] = new SampleTask(RoomType.Crater);
                    break;
                case ObjectiveHandler.MarsObjCardType.PlainsSample:
                    tasks[0] = new SampleTask(RoomType.Plains);
                    break;
                case ObjectiveHandler.MarsObjCardType.RidgeVideo:
                    tasks[0] = new VideoTask(RoomType.Ridge);
                    break;
                case ObjectiveHandler.MarsObjCardType.CraterVideo:
                    tasks[0] = new VideoTask(RoomType.Crater);
                    break;
                case ObjectiveHandler.MarsObjCardType.Golf:
                    tasks[0] = new GolfTask(RoomType.Plains);
                    break;
                default:
                    break;
            }

            Objective objective = new(tasks);
            objective.Name = _type.ToString();
            return objective;
        }

        public void ActivateTasks()
        {
            foreach (Task task in Tasks)
            {
                task.Activate();
            }
        }

        public void UndoActivateTasks()
        {
            foreach (Task task in Tasks)
            {
                task.UndoActivate();
            }
        }
    }
}