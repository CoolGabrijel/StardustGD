using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Godot.UI
{
	public partial class TaskDisplay : Control
	{
		[Export] Control taskContainer;
		[Export] PackedScene taskPrefab;
        [Export] RoomIcon[] roomIcons;
		[Export] Control separator;

        public override void _Ready()
		{
			Task[] tasks = GetAllTasks();

			foreach (Task task in tasks)
			{
				TaskItem taskInstance = taskPrefab.Instantiate<TaskItem>();
				taskContainer.AddChild(taskInstance);
				Texture2D icon = GetIcon(task);
				taskInstance.Initialize(task, icon);
			}
		}

        public override void _Process(double delta)
        {
			int taskAmount = 0;
			for (int i = 0; i < taskContainer.GetChildCount(); i++)
			{
				Control child = taskContainer.GetChild<Control>(i);
				if (!child.Visible) continue;

				if (child.GetType() != typeof(HSeparator))
				{
					taskContainer.MoveChild(separator, i-1);
					taskAmount++;
				}
			}

			separator.Visible = taskAmount >= 2;
			Visible = taskAmount != 0;
        }

        private Task[] GetAllTasks()
		{
			List<Task> tasks = new List<Task>();
			foreach (Objective objective in ObjectiveHandler.Objectives)
			{
				tasks.AddRange(objective.Tasks);
			}
			return tasks.ToArray();
		}

        private Texture2D GetIcon(Task task)
        {
			foreach (RoomIcon icon in roomIcons)
			{
				if (icon.RoomType == task.Room.RoomType) return icon.Icon;
			}

			return null;
        }
    } 
}
