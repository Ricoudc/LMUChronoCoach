using GameReaderCommon;
using SimHub.Plugins;

namespace LMUChronoCoach
{
    [PluginDescription("Plugin LMU pour coaching chrono et télémétrie")]
    [PluginAuthor("ChatGPT")]
    public class LMUChronoCoachPlugin : IPlugin, IDataPlugin
    {
        private LMUSharedMemoryReader reader;

        public PluginManager PluginManager { get; set; }

        public void Init(PluginManager manager)
        {
            reader = new LMUSharedMemoryReader();
            reader.Start();
            var owner = GetType();
            manager.AddProperty("SpeedKmh", owner, 0f, "km/h");
            manager.AddProperty("Gear", owner, 0);
            manager.AddProperty("Throttle", owner, 0f);
            manager.AddProperty("Brake", owner, 0f);
            manager.AddProperty("LapDelta", owner, 0f, "m");
            manager.AddProperty("RPM", owner, 0f, "rpm");
            manager.AddProperty("CarName", owner, string.Empty);
            manager.AddProperty("TrackName", owner, string.Empty);
            manager.AddProperty("CoachMessage", owner, string.Empty);
        }

        public void End(PluginManager manager)
        {
            reader?.Stop();
        }

        public void DataUpdate(PluginManager manager, ref GameData data)
        {
            var lmu = reader?.GetCurrentData();
            if (lmu == null) return;
            var owner = GetType();
            manager.SetPropertyValue("SpeedKmh", owner, lmu.SpeedKmh);
            manager.SetPropertyValue("Gear", owner, lmu.Gear);
            manager.SetPropertyValue("Throttle", owner, lmu.Throttle);
            manager.SetPropertyValue("Brake", owner, lmu.Brake);
            manager.SetPropertyValue("LapDelta", owner, lmu.LapDelta);
            manager.SetPropertyValue("RPM", owner, lmu.RPM);
            manager.SetPropertyValue("CarName", owner, lmu.CarName ?? string.Empty);
            manager.SetPropertyValue("TrackName", owner, lmu.TrackName ?? string.Empty);
            manager.SetPropertyValue("CoachMessage", owner, lmu.CoachMessage ?? string.Empty);
        }
    }
}