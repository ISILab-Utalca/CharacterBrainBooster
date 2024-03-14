using CBB.Lib;
using System.Collections.Generic;

namespace ArtificialIntelligence.Utility
{
    internal interface IAgentBrain
    {
        List<ISensor> Sensors { get; }
        System.Action<Option, List<Option>> OnDecisionTaken { get; set; }
        System.Action<SensorActivation> OnSensorUpdate { get; set; }
    }
}