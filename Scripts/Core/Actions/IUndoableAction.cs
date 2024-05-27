using Godot;
using System;

namespace Stardust
{
    // Doing the actions and undo system like this was revealed to me in a dream. Or a nightmare. Depends how well it works.
    public interface IUndoableAction
    {
        public void Do();
        public void Undo();
    } 
}
