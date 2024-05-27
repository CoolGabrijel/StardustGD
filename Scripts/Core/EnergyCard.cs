using Godot;
using System;

namespace Stardust
{
	public class EnergyCard
	{
        public EnergyCard(int energy)
        {
            Energy = energy;
        }

        public int Energy { get; init; }
        public bool Exhausted { get; set; } = false;
	} 
}
