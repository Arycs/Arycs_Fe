using System;

namespace Arycs_Fe.ScriptManagement
{
    public class MapEventNoCondition : MapEvent
    {
        public override bool CanTrigger(MapAction action)
        {

            if (!onlyonce)
            {
                onlyonce = true;
            }

            return true;
        }
    }
}