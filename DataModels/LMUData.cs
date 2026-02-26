namespace LMUChronoCoach.DataModels
{
    public class LMUData
    {
        public float SpeedKmh { get; set; }
        public int Gear { get; set; }
        public float Throttle { get; set; }
        public float Brake { get; set; }
        public float RPM { get; set; }
        public float LapDelta { get; set; }
        public string CarName { get; set; }
        public string TrackName { get; set; }
        public string CoachMessage { get; set; }
    }
}
