using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class DamageIndicator : Control
	{
		[Export] Color healthyColor;
		[Export] Color damagedColor;
		[ExportCategory("References")]
		[Export] Control tickParent;

        public override void _Ready()
        {
			InstantiateTicks();
        }

        public override void _Process(double delta)
        {
			for (int i = 0; i < tickParent.GetChildCount(); i++)
			{
				ColorRect tick = tickParent.GetChild<ColorRect>(i);
				if (GetDamage() > i) tick.Color = damagedColor;
				else tick.Color = healthyColor;

            }
        }

		private int GetDamage()
		{
			int damage = 0;
			foreach (Room room in GameLogic.RoomManager.Rooms)
			{
				damage += room.DamageAmount;
			}

			return damage;
		}

		private void InstantiateTicks()
		{
			Node dupe = tickParent.GetChild(0).Duplicate();
			tickParent.FreeChildren();

			for (int i = 0; i < GameLogic.MaxDamage; i++)
			{
				Node instance = dupe.Duplicate();
				tickParent.AddChild(instance);
			}
		}
    } 
}
