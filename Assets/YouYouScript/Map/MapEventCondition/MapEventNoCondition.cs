using System;

namespace Arycs_Fe.ScriptManagement
{
    [Serializable]
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