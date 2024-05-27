using Godot;
using System;

namespace Stardust.Godot
{
	public partial class EnergyLabel : Label
	{
        public override void _Process(double delta)
        {
            Text = $"Energy Expended: {GameLogic.EnergyExpended}";
        }
    } 
}
