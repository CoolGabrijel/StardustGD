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

        public override void Complete()
        {
            base.Complete();

            if (ObjectiveHandler.NextObjective != null && ObjectiveHandler.NextObjective.Tasks[0].Tag == "FirstSteps")
                ObjectiveHandler.RevealNewObjective();
        }
    }
}