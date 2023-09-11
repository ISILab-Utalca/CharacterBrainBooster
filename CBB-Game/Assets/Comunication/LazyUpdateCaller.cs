using System;
using UnityEngine;
namespace CBB.Comunication
{
    public class LazyUpdateCaller : MonoBehaviour
    {
        private static LazyUpdateCaller instance;
        public static void AddUpdateCallback(Action updateMethod)
        {
            if (instance == null)
            {
                instance = new GameObject("[Update Caller]").AddComponent<LazyUpdateCaller>();
            }
            instance.updateCallback += updateMethod;
        }

        private Action updateCallback;

        private void Update()
        {
            updateCallback?.Invoke();
        }
    }

}
