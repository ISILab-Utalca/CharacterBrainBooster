using CBB.Lib;
using System;
using System.Collections.Generic;

public class AgentDataGenerators
{
    private static System.Random random = new System.Random();
    private static List<Type> AgentTypes = new() { typeof(Goblin), typeof(Villager) };
    // Random names to identify the agent instance
    private static string[] firstNames = { "John", "Emma", "Michael", "Olivia", "William", "Ava", "James", "Isabella", "Benjamin", "Sophia" };
    private static string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Wilson", "Taylor" };
    private static List<Type> sensorTypes = new() { typeof(SensorFieldOfView), typeof(SensorAuditoryField) };

    private static string RandomName()
    {
        string firstName = firstNames[random.Next(firstNames.Length)];
        string lastName = lastNames[random.Next(lastNames.Length)];

        return firstName + " " + lastName;
    }

    private static Type RandomAgentType()
    {
        var randomIndex = random.Next(AgentTypes.Count);
        return AgentTypes[randomIndex];
    }

    private static Type RandomSensorType()
    {
        var randomIndex = random.Next(sensorTypes.Count);
        return sensorTypes[randomIndex];
    }

    public static AgentBrainData New_Brain_Data()
    {
        return new AgentBrainData(RandomAgentType(), RandomName());
    }

    public static SensorStatus New_Sensor_Data()
    {
        Dictionary<string, object> configurationDictionary = new() {
            { "param_a", 1 }, {"param_b", 4 }
        };
        return new SensorStatus(RandomSensorType(), configurationDictionary, null);
    }

    public static AgentData New_Agent_Data(Agent agent)
    {
        int numberOfSensors = agent.Sensors.Count;
        List<SensorStatus> sensorsData = new();
        for (int i = 0; i < numberOfSensors; i++)
        {
            sensorsData.Add(New_Sensor_Data());
        }
        AgentData ad = new()
        {
            AgentType = RandomAgentType(),
            BrainData = New_Brain_Data(),
            SensorsData = sensorsData
        };

        return ad;
    }
}
