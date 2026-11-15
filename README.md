# LMU Chrono Coach

Windows/SimHub-based LMU telemetry and race-engineer project.

## Current repository status
The repository currently contains a small SimHub plugin prototype. It is not yet a complete standalone race engineer. This increment adds a telemetry-neutral analysis core so live telemetry, replay/import and UI can share the same domain model.

## Analysis core
- Normalized telemetry sample model (SI units, optional channels and explicit validity).
- Track-specific corner windows. Corner locations must come from verified track data or user calibration; they are never guessed.
- Per-lap and per-corner stint aggregation, including speed, braking point, throttle pickup, fuel when available and traffic flags.
- Conservative coaching against the driver's own comparable reference. Traffic-affected or undersampled corners are excluded.
- Read-only setup document loading that preserves unknown lines. It does not alter setup values until verified per-car ranges and increments exist.

## Important limitations
- The previous shared-memory reader attempted to marshal an undefined telemetry structure. That unsafe/incomplete reader has been replaced by a publication boundary. A real adapter must use a verified version of the official rFactor 2/LMU shared-memory structures before it can publish live samples. Do not infer memory offsets or marshal layouts.
- The prototype does not yet provide full tyre wear, steering angle, wheel loads, aero platform, weather or opponent telemetry unless a verified source supplies those channels.
- Automatic setup modification is disabled until exact LMU setup schemas, allowed ranges and increments are validated per car.
- Voice output, desktop UI, overlay packaging, replay analysis and installer are subsequent increments.

## Target architecture
LMU shared memory / telemetry source -> validated adapter -> normalized telemetry stream -> stint/corner analysis -> prioritized coach events -> speech and desktop/overlay UI.

The same analysis core will later accept imported telemetry files, allowing offline analysis and live coaching to use identical calculations.
