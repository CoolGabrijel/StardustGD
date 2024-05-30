using Godot;
using System;

namespace Stardust
{
    public class TurnQueue
    {
        public TurnQueue(Pawn[] pawns)
        {
            Pawns = pawns;
        }

        public Pawn[] Pawns { get; private set; }
        public Pawn CurrentPawn { get { return Pawns[current]; } }

        private int current;

        public void Next()
        {
            current++;

            if (current >= Pawns.Length) current = 0;
        }

        public void Previous()
        {
            current--;

            if (current < 0) current = 0;
        }
    } 
}
