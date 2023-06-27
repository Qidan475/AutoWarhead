using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoWarhead.CustomWarheads
{
    internal class AutoWarhead : CustomWarhead, IDisposable
    {
        private CoroutineHandle _autoWarheadCoroutine;

        protected override Color WarheadColor { get; } = new Color(1f, 1f, 0);

        public AutoWarhead()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
            Exiled.Events.Handlers.Warhead.Detonated += OnWarheadBoom;
        }

        public void Dispose()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
            Exiled.Events.Handlers.Warhead.Detonated -= OnWarheadBoom;
            StopCoroutines();
        }

        private void OnRoundStart()
        {
            int delay = -1;
            int playersCount = Player.Dictionary.Count;
            foreach (var item in MainThing.Instance.Config.AutoWarheadDelay)
            {
                if (playersCount <= item.Key)
                {
                    delay = item.Value;
                    break;
                }
            }

            if (delay == -1)
            {
                Log.Warn("Сломанный конфиг - авто-боеголовка не будет запущена");
                return;
            }

            _autoWarheadCoroutine = Timing.RunCoroutine(AutoWarheadWaiter(delay));
        }

        private IEnumerator<float> AutoWarheadWaiter(float delay)
        {
            yield return Timing.WaitForSeconds(delay);
            foreach (var ply in Player.List)
            {
                ply.ShowHint(MainThing.Instance.Config.AutoWarheadText, 20f);
            }

            yield return Timing.WaitForSeconds(MainThing.Instance.Config.AutoWarheadWarningDelay);
            if (AlphaWarheadController.InProgress) 
            { 
                AlphaWarheadController.Singleton.IsLocked = true;
            }
            else
            {
                StartWarhead(true, MainThing.Instance.Config.AutoWarheadScenarioId);
            }
        }

        private void OnWarheadBoom()
        {
            StopCoroutines();
        }

        private void OnRoundRestart()
        {
            StopCoroutines();
        }

        private void StopCoroutines()
        {
            Timing.KillCoroutines(_autoWarheadCoroutine);
        }
    }
}
