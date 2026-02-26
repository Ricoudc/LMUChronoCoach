using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using LMUChronoCoach.DataModels;

namespace LMUChronoCoach
{
    public class LMUSharedMemoryReader
    {
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor accessor;
        private Thread thread;
        private bool runThread;
        private LMUData lastData;

        const string MAP_NAME = "$rFactor2$"; 

        public void Start()
        {
            runThread = true;
            thread = new Thread(ReadLoop);
            thread.Start();
        }

        public void Stop()
        {
            runThread = false;
            thread?.Join();
            accessor?.Dispose();
            mmf?.Dispose();
        }

        public LMUData GetCurrentData() => lastData;

        private void ReadLoop()
        {
            try
            {
                mmf = MemoryMappedFile.OpenExisting(MAP_NAME);
                accessor = mmf.CreateViewAccessor();

                while (runThread)
                {
                    var buffer = new byte[Marshal.SizeOf(typeof(rF2Telemetry))];
                    accessor.ReadArray(0, buffer, 0, buffer.Length);

                    var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var raw = (rF2Telemetry)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(rF2Telemetry));
                    handle.Free();

                    string coach = "";
                    if (raw.mUnfilteredBrake > 0.9 && raw.mSpeed > 80)
                        coach = "Freine un peu plus tard";
                    else if (raw.mUnfilteredThrottle < 0.2 && raw.mSpeed < raw.mBestLapSpeed - 5)
                        coach = "Réaccélère plus tôt";

                    lastData = new LMUData
                    {
                        SpeedKmh = raw.mSpeed * 3.6f,
                        Gear = raw.mGear,
                        Throttle = raw.mUnfilteredThrottle,
                        Brake = raw.mUnfilteredBrake,
                        RPM = raw.mEngineRPM,
                        LapDelta = raw.mLapDistance - raw.mBestLapDistance,
                        CarName = raw.mVehicleName,
                        TrackName = raw.mTrackName,
                        CoachMessage = coach
                    };

                    Thread.Sleep(10);
                }
            }
            catch { }
        }
    }
}
