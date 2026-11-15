using LMUChronoCoach.DataModels;

namespace LMUChronoCoach
{
    /// <summary>
    /// Boundary between a verified LMU telemetry adapter and the SimHub plugin.
    /// The old implementation attempted to marshal an undefined rF2Telemetry
    /// structure from shared memory. Reading an unknown ABI is unsafe, so this
    /// class deliberately does not open or interpret the memory map.
    ///
    /// A future adapter must validate the exact LMU shared-memory version and
    /// publish normalized data through Publish().
    /// </summary>
    public class LMUSharedMemoryReader
    {
        private volatile LMUData lastData;

        public void Start()
        {
            // No reader is started until the shared-memory ABI is verified.
        }

        public void Stop()
        {
            lastData = null;
        }

        public LMUData GetCurrentData()
        {
            return lastData;
        }

        public void Publish(LMUData data)
        {
            lastData = data;
        }
    }
}
