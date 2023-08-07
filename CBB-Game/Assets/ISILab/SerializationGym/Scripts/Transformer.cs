using ArtificialIntelligence.Utility;
using UnityEngine;
using Utility;

namespace CBB.Lib
{
    /// <summary>
    /// Helper class inherited from Monobehaviour. It's serialized using the
    /// JSON Convert approach
    /// </summary>
    public class Transformer : MonoBehaviour
    {
        public int tests = 0;
        //public enum Mode { Brain,Prefab,SingleClass}
        //public Mode mode = Mode.SingleClass;
        //public Object brain;

        [ContextMenu("save data to json")]
        public void BrainToJson()
        {
            // The next line fails because "this" (the Transformer instance) derives from Monobehavior
            // thus, an error is raised on runtime because of a "rigidbody" property no longer supported
            // by the engine. 
            //string s = JSONDataManager.SerializeData(this);
            //This line works because a custom converter is added to the underlying JsonSerializer settings
            string s = JSONDataManager.SerializeData(this,new TransformClassConverter());
            Debug.Log(s);
        }
    }
}

