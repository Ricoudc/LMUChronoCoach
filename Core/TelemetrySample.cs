using System;
namespace LMUChronoCoach.Core
{
    /// <summary>Normalized telemetry frame. SI units are used internally.</summary>
    public sealed class TelemetrySample
    {
        public DateTime TimestampUtc { get; set; }
        public int LapNumber { get; set; }
        public double LapDistanceMeters { get; set; }
        public double SpeedMetersPerSecond { get; set; }
        public double Throttle { get; set; }
        public double Brake { get; set; }
        public double Steering { get; set; }
        public int Gear { get; set; }
        public double EngineRpm { get; set; }
        public double? FuelLitres { get; set; }
        public double? FrontLeftTyreTemperatureC { get; set; }
        public double? FrontRightTyreTemperatureC { get; set; }
        public double? RearLeftTyreTemperatureC { get; set; }
        public double? RearRightTyreTemperatureC { get; set; }
        public double? FrontLeftTyrePressureKpa { get; set; }
        public double? FrontRightTyrePressureKpa { get; set; }
        public double? RearLeftTyrePressureKpa { get; set; }
        public double? RearRightTyrePressureKpa { get; set; }
        public bool InTraffic { get; set; }
        public bool IsValid { get; set; }
        public string CarName { get; set; }
        public string Category { get; set; }
        public string TrackName { get; set; }
        public double SpeedKmh { get { return SpeedMetersPerSecond * 3.6; } }
    }
}