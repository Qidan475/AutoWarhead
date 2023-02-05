using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoWarhead.CustomWarheads
{
    internal class OmegaWarhead : CustomWarhead, IDisposable
    {
        private bool _killEverythingOnExplosion = false;
        private CoroutineHandle _omegaWarheadCoroutine;

        protected override Color WarheadColor { get; } = new Color(1f, 0, 1f);

        public OmegaWarhead()
        {
            Exiled.Events.Handlers.Warhead.Detonated += OnWarheadBoom;
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocket;
        }

        public void Dispose()
        {
            Exiled.Events.Handlers.Warhead.Detonated -= OnWarheadBoom;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocket;
            StopCoroutines();
        }

        private void OnWarheadBoom()
        {
            if (_killEverythingOnExplosion)
            {
                foreach (var ply in Player.List)
                {
                    if (ply != null && ply.IsAlive)
                    {
                        ply.IsGodModeEnabled = false;
                        ply.Kill(DamageType.Warhead);
                        if (ply.IsAlive)
                            ply.Role.Set(RoleTypeId.Spectator, SpawnReason.Died);
                    }
                }

                return;
            }

            if (!_omegaWarheadCoroutine.IsRunning)
                _omegaWarheadCoroutine = Timing.RunCoroutine(OmegaWarheadWaiter());
        }

        private IEnumerator<float> OmegaWarheadWaiter()
        {
            yield return Timing.WaitForSeconds(MainThing.Instance.Config.OmegaWarheadDelay);
            _killEverythingOnExplosion = true;
            StartWarhead(false, MainThing.Instance.Config.OmegaWarheadScenarioId);

            yield return Timing.WaitForSeconds(4f);
            foreach (var ply in Player.List)
            {
                if (ply.IsHuman)
                    ply.CurrentItem = ply.AddItem(ItemType.GunLogicer);

                if (ply.IsScp)
                {
                    ply.ChangeEffectIntensity(EffectType.Scp207, 2, float.MaxValue);
                    ply.ChangeEffectIntensity(EffectType.Burned, 1, float.MaxValue);
                }
            }
        }

        private void OnEnteringPocket(EnteringPocketDimensionEventArgs ev)
        {
            if (!_killEverythingOnExplosion)
                return;

            ev.IsAllowed = false;
            ev.Player.Kill(DamageType.Scp106);
        }

        private void OnRoundRestart()
        {
            _killEverythingOnExplosion = false;
            StopCoroutines();
        }

        private void StopCoroutines()
        {
            Timing.KillCoroutines(_omegaWarheadCoroutine);
        }
    }
}
