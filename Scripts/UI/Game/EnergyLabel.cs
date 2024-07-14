using Godot;
using System;

namespace Stardust.Godot
{
	public partial class EnergyLabel : Label
	{
        public override void _Process(double delta)
        {
            Text = GameLogic.EnergyExpended.ToString();
        }
    } 
}
