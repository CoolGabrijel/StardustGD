using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stardust
{
    public static class ObjectiveHandler
    {
        public enum ObjectiveCardType { CourseCorrection, CockpitUpdate, WorkshopUpdate, AirFilters, DebrisFilters, Experiment, Pressure, ClearDebris, Recalibrations, Laundry, Recycling, Diagnostics }
        public enum MarsObjCardType { Flag, GullySample, CraterSample, PlainsSample, RidgeVideo, CraterVideo, Golf }
        public static ReadOnlyCollection<Objective> Objectives => objectives.AsReadOnly();
        public static Objective CurrentObjective { get { return objectives[objectiveIndex]; } }
        public static Objective NextObjective
        {
            get
            {
                if (objectiveIndex + 1 < objectives.Count)
                {
                    return objectives[objectiveIndex + 1];
                }
                else return null;
            }
        }
        public static bool ObjectiveComplete
        {
            get
            {
                return CurrentObjective.Completed;
            }
        }
        public static int ObjectivesRemaining
        {
            get
            {
                int objRemaining = 0;

                foreach (Objective objective in objectives)
                {
                    if (!objective.Completed) objRemaining++;
                }

                return objRemaining;
            }
        }

        public static event Action<Objective> OnNewObjective;

        static List<ObjectiveCardType> objectiveCards;

        static List<Objective> objectives;
        static int objectiveIndex;

        public static void RevealNewObjective()
        {
            objectiveIndex++;

            CurrentObjective.ActivateTasks();

            OnNewObjective?.Invoke(CurrentObjective);
        }

        public static void UndoRevealNewObjective()
        {
            CurrentObjective.UndoActivateTasks();

            objectiveIndex--;

            OnNewObjective?.Invoke(CurrentObjective);
        }

        public static void SetObjectives(string[] _objectives)
        {
            objectiveCards = new List<ObjectiveCardType>();
            objectives = new List<Objective>();

            foreach (string objective in _objectives)
            {
                objectives.Add(ObjectiveFactory.CreateObjective(objective));
            }

            objectiveIndex = 0;

            CurrentObjective.ActivateTasks();

            OnNewObjective?.Invoke(CurrentObjective);
        }

        public static void Initialize(int _amount)
        {
            // Get random objectives
            List<ObjectiveCardType> allObjectives = new List<ObjectiveCardType>();
            allObjectives.AddRange((IEnumerable<ObjectiveCardType>)Enum.GetValues(typeof(ObjectiveCardType)));
            List<ObjectiveCardType> randomObjectives = new List<ObjectiveCardType>();

            System.Random rng = new System.Random();

            for (int i = 0; i < _amount; i++)
            {
                int randNum = rng.Next(allObjectives.Count);
                ObjectiveCardType randObjective = allObjectives[randNum];
                allObjectives.Remove(randObjective);
                randomObjectives.Add(randObjective);
            }

            objectiveCards = randomObjectives;

            objectives = new List<Objective>();
            foreach (ObjectiveCardType type in objectiveCards)
            {
                objectives.Add(Objective.CreateObjective(type));
            }

            //If we have the First Steps Expansion
            if (StardustGameConfig.CurrentConfig.FirstStepsEnabled)
            {
                // Get Mars Objectives
                List<MarsObjCardType> allMarsObj = new();
                allMarsObj.AddRange((IEnumerable<MarsObjCardType>)Enum.GetValues(typeof(MarsObjCardType)));
                List<MarsObjCardType> marsObjectives = new();

                int marsObjAmount = 3;
                if (StardustGameConfig.CurrentConfig.Difficulty == StardustGameConfig.GameDifficulty.Hard) marsObjAmount = 4;

                for (int i = 0; i < marsObjAmount; i++)
                {
                    int randNum = rng.Next(allMarsObj.Count);
                    MarsObjCardType randObjective = allMarsObj[randNum];
                    allMarsObj.Remove(randObjective);
                    marsObjectives.Add(randObjective);
                }

                marsObjectives = new() { MarsObjCardType.GullySample, MarsObjCardType.Flag }; // testing only.

                foreach (MarsObjCardType objType in marsObjectives)
                {
                    Objective objective = Objective.CreateObjective(objType);
                    //objectives.Insert(objectives.Count / 2, objective);
                    objectives.Insert(1, objective); // Testing only
                }
            }

            objectiveIndex = 0;

            CurrentObjective.ActivateTasks();

            OnNewObjective?.Invoke(CurrentObjective);
        }

        public static bool CheckAllObjectivesCompleted()
        {
            foreach (Objective objective in objectives)
            {
                if (!objective.Completed) return false;
            }

            GameLogic.AnnounceGameEnd(true);

            return true;
        }
    }
}