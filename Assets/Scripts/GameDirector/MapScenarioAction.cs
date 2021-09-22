using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class MapScenarioAction : ScenarioAction
    {
        public MapScenarioAction(MapAction previous) : base(previous)
        {
            
        }

        protected override void BackAction()
        {
            MapAction action = previous as MapAction;
            action.ScenarioDone();
        }

        
    }
}