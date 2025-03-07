using Stardust.Actions;
using System;
using System.Collections.Generic;

namespace Stardust
{
    public abstract class Room
    {
        public event Action OnItemDrop;
        public event Action OnDamage;
        public event Action OnBreak;
        public event Action OnRepair;

        public virtual string Name
        {
            get
            {
                return RoomType switch
                {
                    RoomType.LifeSupport => "Life Support",
                    RoomType.SolarPanels => "Solar Panels",
                    _ => RoomType.ToString(),
                };
            }
        }
        public abstract RoomType RoomType { get; }
        public int Capacity
        {
            get
            {
                // Hab, Comms and Workshop have a higher capacity when player count is 3 or more
                if (RoomType == RoomType.Habitation || RoomType == RoomType.Comms || RoomType == RoomType.Workshop && GameLogic.TurnQueue.Pawns.Length > 2)
                    return 2;
                return 1;
            }
        }
        public List<(Direction, Room)> Neighbours { get; set; } = new();
        public virtual bool CanBeActivated { get; }
        public List<Pawn> Pawns { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public int Parts
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items == null) return 0;
                    if (Items[i].Type == ItemType.Part) counter++;
                }

                return counter;
            }
        }
        public int DamageAmount { get; set; }
        public bool Broken => DamageAmount >= 3;

        public void ActivateAbility(Pawn pawn)
        {
            IUndoableAction action = null;

            switch (RoomType)
            {
                case RoomType.Workshop:
                    action = new CreatePart(pawn, 1, this, new(ItemType.Part));
                    break;
                case RoomType.Habitation:
                    action = new Sleep(pawn);
                    break;
                case RoomType.Comms:
                    action = new RevealObjective(pawn);
                    break;
                case RoomType.Airlock:
                    action = new DropLander();
                    break;
                case RoomType.Lander:
                    action = new LaunchLander();
                    break;
                default:
                    break;
            }
            if (action == null) return;

            action.Do();
            ActionLibrary.AddAction(action);
        }

        public void Damage()
        {
            bool wasBroken = Broken;

            DamageAmount++;

            if (wasBroken != Broken) OnBreak?.Invoke();
            else OnDamage?.Invoke();
        }

        public void AddItem(Item item)
        {
            if (item == null) throw new Exception($"Attempted to add null item to {Name}.");

            Items.Add(item);
            OnItemDrop?.Invoke();
        }

        public Item GetItem(ItemType type)
        {
            foreach (Item item in Items)
            {
                if (item.Type == type) return item;
            }

            return null;
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
    } 
}
