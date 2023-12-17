namespace HeartRateZoneService.Domain;

public class HeartRateZoneReached
{
    public Guid DeviceId { get; set; }

    public HeartRateZone Zone { get; set; }

    public DateTime DateTime { get; set; }

    public int HeartRate { get; set; }

    public int MaxHeartRate { get; set; }

    public HeartRateZoneReached(Guid deviceId, HeartRateZone zone, DateTime dateTime, int heartRate, int maxHeartRate)
    {
        DeviceId = deviceId;
        Zone = zone;
        DateTime = dateTime;
        HeartRate = heartRate;
        MaxHeartRate = maxHeartRate;
    }
}
