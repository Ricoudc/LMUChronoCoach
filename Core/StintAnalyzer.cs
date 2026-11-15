using System;
using System.Collections.Generic;
using System.Linq;
namespace LMUChronoCoach.Core
{
    public sealed class CornerLapMetrics
    {
        public string CornerId { get; set; }
        public int LapNumber { get; set; }
        public double? MinimumSpeedKmh { get; set; }
        public double? EntrySpeedKmh { get; set; }
        public double? ExitSpeedKmh { get; set; }
        public double? BrakeStartMeters { get; set; }
        public double? ThrottlePickupMeters { get; set; }
        public double? ApexDistanceMeters { get; set; }
        public double? TimeInCornerSeconds { get; set; }
        public bool TrafficAffected { get; set; }
        public int SampleCount { get; set; }
    }
    public sealed class LapMetrics
    {
        public int LapNumber { get; set; }
        public double? MaxSpeedKmh { get; set; }
        public double? AverageSpeedKmh { get; set; }
        public double? FuelStartLitres { get; set; }
        public double? FuelEndLitres { get; set; }
        public int SampleCount { get; set; }
        public bool TrafficAffected { get; set; }
        public IList<CornerLapMetrics> Corners { get; set; } = new List<CornerLapMetrics>();
    }
    /// <summary>Deterministic stint aggregation; missing channels remain missing.</summary>
    public sealed class StintAnalyzer
    {
        public IList<LapMetrics> Analyze(IEnumerable<TelemetrySample> source, IEnumerable<CornerDefinition> cornerDefinitions)
        {
            var samples = (source ?? Enumerable.Empty<TelemetrySample>()).Where(x => x != null && x.IsValid).OrderBy(x => x.TimestampUtc).ToList();
            var corners = (cornerDefinitions ?? Enumerable.Empty<CornerDefinition>()).ToList();
            return samples.GroupBy(x => x.LapNumber).OrderBy(g => g.Key)
                .Select(g => AnalyzeLap(g.Key, g.ToList(), corners)).ToList();
        }
        private static LapMetrics AnalyzeLap(int lapNumber, IList<TelemetrySample> samples, IList<CornerDefinition> definitions)
        {
            var speeds = samples.Where(x => x.SpeedMetersPerSecond >= 0).Select(x => x.SpeedKmh).ToList();
            var fuel = samples.Where(x => x.FuelLitres.HasValue).ToList();
            return new LapMetrics {
                LapNumber = lapNumber,
                MaxSpeedKmh = speeds.Count == 0 ? (double?)null : speeds.Max(),
                AverageSpeedKmh = speeds.Count == 0 ? (double?)null : speeds.Average(),
                FuelStartLitres = fuel.Count == 0 ? (double?)null : fuel.First().FuelLitres,
                FuelEndLitres = fuel.Count == 0 ? (double?)null : fuel.Last().FuelLitres,
                SampleCount = samples.Count,
                TrafficAffected = samples.Any(x => x.InTraffic),
                Corners = definitions.Select(d => AnalyzeCorner(lapNumber, samples, d)).ToList()
            };
        }
        private static CornerLapMetrics AnalyzeCorner(int lapNumber, IList<TelemetrySample> lap, CornerDefinition definition)
        {
            var samples = lap.Where(x => definition.Contains(x.LapDistanceMeters)).OrderBy(x => x.TimestampUtc).ToList();
            if (samples.Count == 0) return new CornerLapMetrics { CornerId = definition.Id, LapNumber = lapNumber };
            var braking = samples.Where(x => x.Brake > 0.05).ToList();
            var throttle = samples.Where(x => x.Throttle > 0.10).ToList();
            var min = samples.OrderBy(x => x.SpeedMetersPerSecond).First();
            var first = samples.First(); var last = samples.Last();
            return new CornerLapMetrics {
                CornerId = definition.Id, LapNumber = lapNumber, EntrySpeedKmh = first.SpeedKmh,
                ExitSpeedKmh = last.SpeedKmh, MinimumSpeedKmh = min.SpeedKmh,
                BrakeStartMeters = braking.Count == 0 ? (double?)null : braking.First().LapDistanceMeters,
                ThrottlePickupMeters = throttle.Count == 0 ? (double?)null : throttle.First().LapDistanceMeters,
                ApexDistanceMeters = min.LapDistanceMeters,
                TimeInCornerSeconds = (last.TimestampUtc - first.TimestampUtc).TotalSeconds,
                TrafficAffected = samples.Any(x => x.InTraffic), SampleCount = samples.Count
            };
        }
    }
}