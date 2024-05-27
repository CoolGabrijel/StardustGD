using Godot;
using Stardust;
using System;

namespace Stardust.Actions
{
    public class CreatePart : IUndoableAction
    {
        public CreatePart(Pawn pawn, int cost, Room workshop, Item part)
        {
            Pawn = pawn;
            EnergyCost = cost;
            Workshop = workshop;
            Part = part;
        }

        public Pawn Pawn { get; private set; }
        public int EnergyCost { get; private set; }
        public Room Workshop { get; private set; }
        public Item Part { get; private set; }

        public void Do()
        {
            Workshop.AddItem(Part);
            GD.Print($"{Workshop.Name}: Created Part");
        }

        public void Undo()
        {
            Workshop.RemoveItem(Part);
            GD.Print($"{Workshop.Name}: Undone Part");
        }
    }
}
