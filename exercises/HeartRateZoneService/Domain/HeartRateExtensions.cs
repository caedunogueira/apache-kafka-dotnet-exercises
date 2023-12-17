namespace HeartRateZoneService.Domain;

public enum HeartRateZone
{
    None,
    Zone1,
    Zone2,
    Zone3,
    Zone4,
    Zone5
}

public static class HeartRateExtensions
{
    public static HeartRateZone GetHeartRateZone(this HeartRate heartRate, int maximumHeartRate)
    {
        var percentage = (Double)heartRate.Value / (Double)maximumHeartRate;

        return percentage switch
        {
            < 0.5 => HeartRateZone.None,
            <= 0.59 => HeartRateZone.Zone1,
            <= 0.69 => HeartRateZone.Zone2,
            <= 0.79 => HeartRateZone.Zone3,
            <= 0.89 => HeartRateZone.Zone4,
            _ => HeartRateZone.Zone5
        };
    }
}
