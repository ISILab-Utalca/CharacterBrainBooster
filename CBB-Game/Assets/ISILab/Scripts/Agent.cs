using UnityEngine;
namespace CBB.Lib
{
    [System.Serializable]
    public class Agent : MonoBehaviour
    {
        /// <summary>
        /// The data structure that is going to be serialized, then sent to CBB
        /// </summary>
        public AgentBasicData BasicData;

        [ContextMenu("Invoke Start")]
        private void Start()
        {
            BasicData = Generators.New_Agent_Basic_Data();
            Debug.Log($"{this} data: name = {BasicData.agentName}; type = {BasicData.agentType}");
        }
    }
}