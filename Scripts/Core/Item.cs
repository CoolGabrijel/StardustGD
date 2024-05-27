using System.Collections;
using System.Collections.Generic;

namespace Stardust
{
    public class Item
    {
        public Item(ItemType _type)
        {
            Type = _type;
        }

        public ItemType Type { get; set; }
    }

    public enum ItemType { Part, Sample, Flag, Objective }
}