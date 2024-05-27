using Godot;
using System;
using System.Collections.Generic;

namespace Stardust
{
	public static class ActionLibrary
	{
		public static List<IUndoableAction> Actions { get; private set; } = new List<IUndoableAction>();

        public static void AddAction(IUndoableAction action) => Actions.Add(action);

		public static void UndoAction()
		{
			if (Actions.Count <= 0) return;

			Actions[^1].Undo();
			Actions.RemoveAt(Actions.Count - 1);
		}
    } 
}
