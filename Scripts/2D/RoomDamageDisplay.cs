using Godot;
using System;

namespace Stardust.Godot
{
	public partial class RoomDamageDisplay : Button
    {
        [Export] private Room2D roomGraphic;
        [Export] Label damageLabel;

        public override void _Ready()
		{
			Pressed += OnClicked;
		}

        public override void _PhysicsProcess(double delta)
        {
            if (roomGraphic.Room.DamageAmount > 0)
            {
                damageLabel.Show();
                damageLabel.Text = $"Damage: {roomGraphic.Room.DamageAmount}";
            }
            else damageLabel.Hide();
        }

		private void OnClicked()
		{
            roomGraphic.Room.DamageAmount--;
		}

        public override void _ExitTree()
        {
			Pressed -= OnClicked;
        }
    } 
}
