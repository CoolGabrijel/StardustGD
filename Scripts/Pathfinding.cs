using System;
using System.Collections.Generic;

namespace Stardust.Godot
{
    public static class Pathfinding
    {
        public static Room[] GetPath(Room start, Pawn pawn)
        {
            var dfs = DepthFirstTraversal(start);
            PathNode pawnNode = null;

            foreach (var node in dfs)
            {
                if (node.Room == pawn.Room)
                {
                    pawnNode = node;
                    break;
                }
            }

            List<Room> result = new();
            PathNode current = pawnNode;
            while (current.Parent != null)
            {
                result.Add(current.Parent);
                foreach (var node in dfs)
                {
                    if (node.Room == current.Parent) current = node;
                }
            }

            return result.ToArray();
        }

        public static IEnumerable<PathNode> DepthFirstTraversal(Room start)
        {
            var visited = new HashSet<PathNode>();
            var stack = new Stack<PathNode>();

            stack.Push(new(start, null));

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                if (!visited.Add(current))
                    continue;

                yield return current;

                List<PathNode> neighbours = new();

                foreach (var neighbour in current.Room.Neighbours)
                {
                    if (!visited.Contains(new(neighbour.Item2, current.Room))) neighbours.Add(new(neighbour.Item2, current.Room));
                }

                // If you don't care about the left-to-right order, remove the Reverse
                foreach (var neighbour in neighbours)
                    stack.Push(neighbour);
            }
        }

        public class PathNode : IEquatable<PathNode>
        {
            public PathNode(Room room, Room parent)
            {
                Room = room;
                Parent = parent;
            }

            public Room Room;
            public Room Parent;

            public override bool Equals(object obj)
            {
                return Equals(obj as PathNode);
            }

            public bool Equals(PathNode other)
            {
                return other is not null &&
                       EqualityComparer<Room>.Default.Equals(Room, other.Room);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Room);
            }
        }
    }
}
