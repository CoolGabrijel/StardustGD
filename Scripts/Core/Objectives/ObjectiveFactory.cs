namespace Stardust
{
    public static class ObjectiveFactory
    {
        public static Objective CreateObjective(string _type)
        {
            Task[] tasks = new Task[2];

            switch (_type)
            {
                case "CourseCorrection":
                    tasks[0] = new BaseTask(RoomType.Cockpit);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                case "CockpitUpdate":
                    tasks[0] = new BaseTask(RoomType.Cockpit);
                    tasks[1] = new BaseTask(RoomType.Comms);
                    break;
                case "WorkshopUpdate":
                    tasks[0] = new BaseTask(RoomType.Workshop);
                    tasks[1] = new BaseTask(RoomType.Comms);
                    break;
                case "AirFilters":
                    tasks[0] = new BaseTask(RoomType.LifeSupport);
                    tasks[1] = new BaseTask(RoomType.Airlock);
                    break;
                case "DebrisFilters":
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case "Experiment":
                    tasks[0] = new BaseTask(RoomType.LifeSupport);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case "Pressure":
                    tasks[0] = new BaseTask(RoomType.Airlock);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                case "ClearDebris":
                    tasks[0] = new BaseTask(RoomType.SolarPanels);
                    tasks[1] = new BaseTask(RoomType.SolarPanels);
                    break;
                case "Recalibrations":
                    tasks[0] = new BaseTask(RoomType.SolarPanels);
                    tasks[1] = new BaseTask(RoomType.Cockpit);
                    break;
                case "Laundry":
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.LifeSupport);
                    break;
                case "Recycling":
                    tasks[0] = new BaseTask(RoomType.Habitation);
                    tasks[1] = new BaseTask(RoomType.Workshop);
                    break;
                case "Diagnostics":
                    tasks[0] = new BaseTask(RoomType.Comms);
                    tasks[1] = new BaseTask(RoomType.Engines);
                    break;
                case "Flag":
                    tasks = new Task[1];
                    tasks[0] = new FlagTask(RoomType.Lander);
                    break;
                case "GullySample":
                    tasks = new Task[1];
                    tasks[0] = new SampleTask(RoomType.Gully);
                    break;
                case "CraterSample":
                    tasks = new Task[1];
                    tasks[0] = new SampleTask(RoomType.Crater);
                    break;
                case "PlainsSample":
                    tasks = new Task[1];
                    tasks[0] = new SampleTask(RoomType.Plains);
                    break;
                case "RidgeVideo":
                    tasks = new Task[1];
                    tasks[0] = new VideoTask(RoomType.Ridge);
                    break;
                case "CraterVideo":
                    tasks = new Task[1];
                    tasks[0] = new VideoTask(RoomType.Crater);
                    break;
                case "Golf":
                    tasks = new Task[1];
                    tasks[0] = new GolfTask(RoomType.Plains);
                    break;
                default:
                    break;
            }

            Objective objective = new(tasks);
            objective.Name = _type;
            return objective;
        }

        public static Objective CreateObjective(ObjectiveHandler.ObjectiveCardType type)
        {
            Task[] tasks = new Task[2];

            switch (type)
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
            objective.Name = type.ToString();
            return objective;
        }
    }
}