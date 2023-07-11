using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for Sensors. A Sensor is a way to emulate a sense
/// for an NPC. Every class that implements this interface should be
/// prefixed with "Sensor".
/// </summary>
public interface ISensible
{
    public void CheckForValue();
    public bool CheckForParentBrain();
}
