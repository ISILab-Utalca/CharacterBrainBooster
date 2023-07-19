using System;
using System.Collections.Generic;
/*
* This file defines the plain classes that hold the information about
* an agent.
*/

namespace CBB.Lib
{
    /// <summary>
    /// Data used to identify this Agent Instance in the CBB tool
    /// </summary>
    [Serializable]
    public class AgentBasicData
    {
        public Type agentType;
        public string agentName;
        public AgentBasicData(Type agentType, string agentName)
        {
            this.agentType = agentType;
            this.agentName = agentName;
        }
    }
    
}
