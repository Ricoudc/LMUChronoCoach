namespace LMUChronoCoach.Core
{
    /// <summary>Track-specific corner window; values must be verified or user-calibrated.</summary>
    public sealed class CornerDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double StartDistanceMeters { get; set; }
        public double EndDistanceMeters { get; set; }
        public double? ReferenceBrakeStartMeters { get; set; }
        public double? ReferenceApexDistanceMeters { get; set; }
        public double? ReferenceThrottlePickupMeters { get; set; }

        public bool Contains(double distanceMeters)
        {
            if (EndDistanceMeters >= StartDistanceMeters)
                return distanceMeters >= StartDistanceMeters && distanceMeters <= EndDistanceMeters;
            return distanceMeters >= StartDistanceMeters || distanceMeters <= EndDistanceMeters;
        }
    }
}