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
