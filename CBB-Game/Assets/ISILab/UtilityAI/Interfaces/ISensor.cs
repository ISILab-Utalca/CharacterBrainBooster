using CBB.Lib;

/// <summary>
/// Interface for Sensors. A Sensor is a way to emulate a sense
/// for an NPC. Every class that implements this interface should be
/// prefixed with "Sensor".
/// </summary>
public interface ISensor
{
    System.Action OnSensorUpdate { get; set; }
    public SensorStatus GetSensorData();
}
