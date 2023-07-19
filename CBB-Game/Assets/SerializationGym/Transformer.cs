using ArtificialIntelligence.Utility;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    /// <summary>
    /// Utility function for serializing data from a prefab to a Json.
    /// It looks for the prefab's name (npc type), it's associated actions
    /// and considerations for each action.
    /// </summary>
    public class Transformer : MonoBehaviour
    {
        public static int tests = 0;
        public enum Mode { Brain,Prefab,SingleClass}
        public Mode mode = Mode.SingleClass;
        public Object brain;

        [ContextMenu("Save data to JSON")]
        public void BrainToJson()
        {
            string path = Application.dataPath + "/Git-Ignore/Test";
            tests++;
            JSONDataManager.SaveData(path, $"brain_{tests}", "json", brain);
        }
    }
}

