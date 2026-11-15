using System;
namespace LMUChronoCoach.Core
{
    public enum CoachPriority { Critical = 0, Important = 1, Coaching = 2, Informational = 3 }
    public sealed class CoachMessage
    {
        public CoachPriority Priority { get; set; }
        public string Text { get; set; }
        public string CornerId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Evidence { get; set; }
    }
    /// <summary>Conservative coaching against the driver's own comparable reference.</summary>
    public sealed class LiveCoach
    {
        private DateTime _lastMessageUtc = DateTime.MinValue;
        private string _lastKey;
        public TimeSpan MinimumMessageInterval { get; set; } = TimeSpan.FromSeconds(5);

        public CoachMessage EvaluateCorner(CornerLapMetrics current, CornerLapMetrics reference, double minimumMeaningfulSpeedDeltaKmh = 2.0)
        {
            if (current == null || reference == null || current.TrafficAffected || reference.TrafficAffected ||
                current.SampleCount < 5 || reference.SampleCount < 5) return null;
            CoachMessage candidate = null;
            if (current.MinimumSpeedKmh.HasValue && reference.MinimumSpeedKmh.HasValue)
            {
                var delta = current.MinimumSpeedKmh.Value - reference.MinimumSpeedKmh.Value;
                if (delta <= -minimumMeaningfulSpeedDeltaKmh)
                    candidate = Build(current, "Vitesse minimale inférieure de " + Math.Abs(delta).ToString("0.0") +
                        " km/h à ton meilleur passage comparable.", "Minimum speed delta=" + delta.ToString("0.0") + " km/h");
            }
            if (candidate == null && current.ThrottlePickupMeters.HasValue && reference.ThrottlePickupMeters.HasValue)
            {
                var delta = current.ThrottlePickupMeters.Value - reference.ThrottlePickupMeters.Value;
                if (delta > 5.0)
                    candidate = Build(current, "Réaccélération plus tardive de " + delta.ToString("0") +
                        " m que sur ton meilleur passage comparable. Vérifie la motricité avant de corriger.",
                        "Throttle pickup delta=" + delta.ToString("0.0") + " m");
            }
            return Throttle(candidate);
        }
        public CoachMessage EvaluateTopSpeed(double currentKmh, double referenceKmh, bool comparableConditions, bool trafficAffected)
        {
            if (!comparableConditions || trafficAffected) return null;
            var delta = currentKmh - referenceKmh;
            if (delta < 1.0) return null;
            return Throttle(new CoachMessage {
                Priority = CoachPriority.Informational,
                Text = "Nouvelle vitesse de pointe : " + currentKmh.ToString("0.0") + " km/h, +" + delta.ToString("0.0") + " km/h par rapport à la référence.",
                CreatedUtc = DateTime.UtcNow, Evidence = "Vmax delta=" + delta.ToString("0.0") + " km/h"
            });
        }
        private static CoachMessage Build(CornerLapMetrics m, string text, string evidence)
        {
            return new CoachMessage { Priority = CoachPriority.Coaching, CornerId = m.CornerId,
                Text = m.CornerId + " : " + text, CreatedUtc = DateTime.UtcNow, Evidence = evidence };
        }
        private CoachMessage Throttle(CoachMessage candidate)
        {
            if (candidate == null) return null;
            var key = candidate.CornerId + "|" + candidate.Text;
            if (key == _lastKey || DateTime.UtcNow - _lastMessageUtc < MinimumMessageInterval) return null;
            _lastKey = key; _lastMessageUtc = DateTime.UtcNow; return candidate;
        }
    }
}