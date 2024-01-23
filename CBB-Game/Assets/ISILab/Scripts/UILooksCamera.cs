using UnityEngine;

namespace CBB.InternalTool.DebugTools
{
    public class UILooksCamera : MonoBehaviour
    {
        Camera cam;
        private void Start()
        {
            cam = Camera.main;
        }
        void Update()
        {

            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}