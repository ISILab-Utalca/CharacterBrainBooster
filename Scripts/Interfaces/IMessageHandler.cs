using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CBB.ExternalTool
{

    public interface IMessageHandler
    {
        void HandleMessage(string message);
    }
}
