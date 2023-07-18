using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generators
{
    private static System.Random random = new System.Random();
    private static List<Type> AgentTypes = new() { typeof(Goblin), typeof(Villager) };
    // Random names to identify the agent instance
    private static string[] firstNames = { "John", "Emma", "Michael", "Olivia", "William", "Ava", "James", "Isabella", "Benjamin", "Sophia" };
    private static string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Wilson", "Taylor" };
    /// <summary>
    /// Creates a new instance of AgentBasicData with random information
    /// </summary>
    /// <returns></returns>
    public static AgentBasicData New_Agent_Basic_Data()
    {
        int typeIndex = random.Next(AgentTypes.Count);
        return new AgentBasicData(AgentTypes[typeIndex],GenerateRandomName());
    }
    private static string GenerateRandomName()
    {
        string firstName = firstNames[random.Next(firstNames.Length)];
        string lastName = lastNames[random.Next(lastNames.Length)];

        return firstName + " " + lastName;
    }
}
