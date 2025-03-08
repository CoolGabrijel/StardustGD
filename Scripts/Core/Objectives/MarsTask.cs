using Stardust.Actions;
using System.Collections;
using System.Collections.Generic;

namespace Stardust
{
    public abstract class MarsTask : Task
    {
        public MarsTask(RoomType _roomType) : base(_roomType)
        {
        }

        public override string Tag => "FirstSteps";
    }
}