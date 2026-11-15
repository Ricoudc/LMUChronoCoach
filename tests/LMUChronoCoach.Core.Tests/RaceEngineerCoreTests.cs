using System;
using System.Collections.Generic;
using LMUChronoCoach.Core;
using Xunit;

public class RaceEngineerCoreTests
{
    [Fact]
    public void Analyzer_ReportsLapAndCornerMetrics()
    {
        var start = DateTime.UtcNow;
        var samples = new List<TelemetrySample>();
        for (int i = 0; i < 12; i++)
        {
            samples.Add(new TelemetrySample {
                TimestampUtc = start.AddMilliseconds(i * 100),
                LapNumber = 1, LapDistanceMeters = 100 + i * 5,
                SpeedMetersPerSecond = 30 - Math.Abs(5 - i),
                Throttle = i > 7 ? 0.7 : 0.0, Brake = i < 3 ? 0.8 : 0.0,
                FuelLitres = 80 - i * 0.01, IsValid = true
            });
        }
        var result = new StintAnalyzer().Analyze(samples, new[] {
            new CornerDefinition { Id = "T1", StartDistanceMeters = 100, EndDistanceMeters = 160 }
        });
        Assert.Single(result);
        Assert.Equal(1, result[0].LapNumber);
        Assert.True(result[0].MaxSpeedKmh > 0);
        Assert.Single(result[0].Corners);
        Assert.True(result[0].Corners[0].MinimumSpeedKmh.HasValue);
    }

    [Fact]
    public void CornerDefinition_SupportsStartFinishWraparound()
    {
        var corner = new CornerDefinition { StartDistanceMeters = 4900, EndDistanceMeters = 100 };
        Assert.True(corner.Contains(4950));
        Assert.True(corner.Contains(50));
        Assert.False(corner.Contains(250));
    }

    [Fact]
    public void Coach_DoesNotCoachOnTrafficAffectedPass()
    {
        var current = new CornerLapMetrics { CornerId = "T4", SampleCount = 10, TrafficAffected = true, MinimumSpeedKmh = 90 };
        var reference = new CornerLapMetrics { CornerId = "T4", SampleCount = 10, MinimumSpeedKmh = 100 };
        Assert.Null(new LiveCoach().EvaluateCorner(current, reference));
    }

    [Fact]
    public void SetupDocument_PreservesUnknownLines()
    {
        const string setup = "[GENERAL]\r\nKnown=1\r\nVendorSpecific=abc\r\n";
        var parsed = SetupFileDocument.Parse(setup);
        Assert.True(parsed.TryGetValue("Known", out var value));
        Assert.Equal("1", value);
        Assert.Contains("VendorSpecific=abc", parsed.ToText());
    }
}