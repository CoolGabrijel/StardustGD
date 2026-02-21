using Godot;
using System;
using Stardust.Actions;

namespace Stardust.Godot.UI;

public partial class StatTracker : Control
{
	[Export] private Label dmgLabel;

	public void UpdateDamageStat()
	{
		int curDamage = 0;
		foreach (Room room in GameLogic.RoomManager.Rooms)
		{
			curDamage += room.DamageAmount;
		}

		int repairedDamage = 0;
		foreach (IUndoableAction action in ActionLibrary.Actions)
		{
			if (action is RepairRoom)
			{
				repairedDamage++;
			}
		}
		
		int totalDamage = curDamage + repairedDamage;

		dmgLabel.Text = $"Total Damage: {totalDamage}\nRepaired Damage: {repairedDamage}";
	}
}
