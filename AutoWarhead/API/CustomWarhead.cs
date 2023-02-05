using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoWarhead
{
    public abstract class CustomWarhead
    {
        protected abstract Color WarheadColor { get; }

        protected void StartWarhead(bool isResumeScenario, int scenarioId)
        {
            MainThing.Instance.CurrentWarhead = this;
            AlphaWarheadController.Singleton.IsLocked = false;
            AlphaWarheadController.Singleton.InstantPrepare();
            AlphaWarheadController.Singleton.StartDetonation();
            AlphaWarheadController.Singleton.NetworkInfo = new AlphaWarheadSyncInfo()
            {
                ResumeScenario = isResumeScenario,
                ScenarioId = scenarioId,
                StartTime = AlphaWarheadController.Singleton.NetworkInfo.StartTime
            };
            AlphaWarheadController.Singleton.IsLocked = true;
            SetLights();
        }

        public void SetLights()
        {
            foreach (var flick in MainThing.Instance.FlickerableLights)
            {
                flick.Network_warheadLightColor = WarheadColor;
                flick.Network_warheadLightOverride = false;
            }
        }
    }
}
