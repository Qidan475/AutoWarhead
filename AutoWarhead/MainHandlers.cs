using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using PlayerRoles;
using Respawning;
using UnityEngine;
using static PlayerList;

namespace AutoWarhead
{
    internal class MainHandlers: IDisposable
    {
        public MainHandlers()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
            Exiled.Events.Handlers.Warhead.Stopping += OnWarheadStop;
        }

        public void Dispose()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
            Exiled.Events.Handlers.Warhead.Stopping -= OnWarheadStop;
        }

        private void OnRoundStart()
        {
            MainThing.Instance.FlickerableLights = UnityEngine.Object.FindObjectsOfType<FlickerableLightController>();
        }

        private void OnWarheadStop(StoppingEventArgs ev)
        {
            if (MainThing.Instance.CurrentWarhead is CustomWarhead cWarhead)
            {
                Timing.CallDelayed(Timing.WaitForOneFrame, () => cWarhead.SetLights());
                if (ev.Player != null && ev.Player != Server.Host)
                {
                    ev.Player.IsGodModeEnabled = false;
                    ev.Player.Kill(DamageType.Warhead);
                }
            }
        }

        private void OnRoundEnd(RoundEndedEventArgs ev)
        {
            AlphaWarheadController.Singleton.IsLocked = false;
            AlphaWarheadController.Singleton.ForceTime(90);
            AlphaWarheadController.Singleton.CancelDetonation();
        }

        private void OnRoundRestart()
        {
            MainThing.Instance.CurrentWarhead = null;
        }
    }
}