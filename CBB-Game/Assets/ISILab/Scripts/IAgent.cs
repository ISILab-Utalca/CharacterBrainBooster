using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    public IAgentInternalState GetInternalState();
}
