using CBB.Lib;
using Newtonsoft.Json;
using UnityEngine;

namespace CBB.ExternalTool
{
    public class HistoryPanelController : MonoBehaviour, IMessageHandler
    {
        // TODO:
        // Deserialize a Decision Package
        // Update the view after receiving a Decision package
        // Handle OnChange event when selecting a history
        readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };
        public void HandleMessage(string message)
        {
            try
            {
                var decisionPack = JsonConvert.DeserializeObject<DecisionPackage>(message, settings);
                GameData.HandleDecisionPackage(decisionPack);
                return;
            }
            catch(System.Exception ex)
            {
                Debug.Log("<color=red>[HISTORY PANEL] Error: </color>" + ex);
            }
        }
    }
}

