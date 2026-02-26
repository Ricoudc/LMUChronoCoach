using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace LMUChronoCoach
{
    [PluginDescription("Plugin LMU pour coaching chrono et télémétrie")]
    [PluginAuthor("ChatGPT")]
    public class LMUChronoCoachPlugin : IPlugin, IDataPlugin, IWpfSettingsV2
    {
        private LMUSharedMemoryReader reader;

        public void Init(PluginManager manager)
        {
            reader = new LMUSharedMemoryReader();
            reader.Start();
        }

        public void End(PluginManager manager)
        {
            reader?.Stop();
        }

        public void DataUpdate(PluginManager manager, ref GameData data)
        {
            var lmu = reader.GetCurrentData();
            if (lmu == null) return;

            this.SetProperty("SpeedKmh", lmu.SpeedKmh);
            this.SetProperty("Gear", lmu.Gear);
            this.SetProperty("Throttle", lmu.Throttle);
            this.SetProperty("Brake", lmu.Brake);
            this.SetProperty("LapDelta", lmu.LapDelta);
            this.SetProperty("RPM", lmu.RPM);
            this.SetProperty("CarName", lmu.CarName);
            this.SetProperty("TrackName", lmu.TrackName);
            this.SetProperty("CoachMessage", lmu.CoachMessage);
        }

        public System.Windows.Controls.Control GetWpfSettingsControl(PluginManager manager)
        {
            return new System.Windows.Controls.TextBlock
            {
                Text = "LMU Chrono Coach - Aucun réglage pour le moment."
            };
        }
    }
}
