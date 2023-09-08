using System.Collections.Generic;

namespace ArtificialIntelligence.Utility
{
    internal interface IAgentBrain
    {
        List<ISensor> Sensors { get; }
        System.Action<Option, List<Option>> OnDecisionTaken { get; set; }
        System.Action OnSetupDone { get; set; }
    }
}