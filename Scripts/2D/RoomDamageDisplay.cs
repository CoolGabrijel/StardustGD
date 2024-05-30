using Godot;
using Stardust.Actions;
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
                damageLabel.Text = $"Damage: {roomGraphic.Room.DamageAmount}";
                //damageLabel.Show();
                Show();
                Disabled = false;
            }
            else
            {
                //damageLabel.Hide();
                Hide();
                Disabled = true;
            }
        }

		private void OnClicked()
		{
            new RepairRoom(roomGraphic.Room).Do();
		}

        public override void _ExitTree()
        {
			Pressed -= OnClicked;
        }
    } 
}
