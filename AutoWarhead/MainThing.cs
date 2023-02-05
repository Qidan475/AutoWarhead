using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace AutoWarhead
{
    public class MainThing: Plugin<PluginConfig>
    {
        public override string Name => "AutoWarhead";

        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);

        public static MainThing Instance { get; private set; }

        internal CustomWarhead CurrentWarhead;
        internal FlickerableLightController[] FlickerableLights;

        private MainHandlers _ev;
        private CustomWarheads.AutoWarhead _autoWarhead;
        private CustomWarheads.OmegaWarhead _omegaWarhead;

        public override void OnEnabled()
        {
            Instance = this;
            _ev = new MainHandlers();
            _autoWarhead = new CustomWarheads.AutoWarhead();
            _omegaWarhead = new CustomWarheads.OmegaWarhead();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _autoWarhead.Dispose();
            _omegaWarhead.Dispose();
            _ev.Dispose();

            Instance = null;
            base.OnDisabled();
        }
    }

    public class PluginConfig: IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public Dictionary<int, int> AutoWarheadDelay { get; set; } = new Dictionary<int, int>()
        {
            [3] = 420,
            [7] = 720,
            [14] = 1050,
            [1337] = 1260
        };

        public int AutoWarheadWarningDelay { get; set; } = 50;

        public int AutoWarheadScenarioId { get; set; } = 3;

        public string AutoWarheadText { get; set; } = "<size=80%>Автоматическая неотключаемая боеголовка будет скоро <color=red>запущена</color>. Немедленно покиньте комплекс</size>";

        public int OmegaWarheadDelay { get; set; } = 50;

        public int OmegaWarheadScenarioId { get; set; } = 0;
    }
}
