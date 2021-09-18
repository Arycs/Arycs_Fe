using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [Serializable]
    public class MapEventInfo
    {
        public string nextScene;

        public string scenarioName;

        public MouseCursorInfo mouseCursor;

        public string cursor;
        
        public struct MouseCursorInfo
        {
            public string prefab;
            public int x;
            public int y;
        }
    }
}