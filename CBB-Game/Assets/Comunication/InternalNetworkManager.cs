using UnityEngine;

namespace CBB.Comunication
{
    public enum InternalMessage
    {
        CLIENT_CONNECTED,
        CLIENT_STOPPED,
        SERVER_STOPPED,
    }
    public class InternalNetworkManager : MonoBehaviour
    {
        private static readonly int header_size = 4;

        public static int HEADER_SIZE { get => header_size; }

        private void Awake()
        {
            Application.quitting += StopInternalServer;
            Application.quitting += StopInternalClient;
        }
        [ContextMenu("Start internal server")]
        public void StartServer()
        {
            try
            {
                Server.Start();
                Debug.Log("<color=yellow>Server started correctly</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Server error: " + e);
            }
            Debug.Log("<color=yellow>Internal connection set</color>");
        }

        [ContextMenu("Start internal client")]
        public void StartClient()
        {
            try
            {
                Client.Start();
                Debug.Log("<color=yellow>Internal client started correctly</color>");

            }
            catch (System.Exception e)
            {
                Debug.LogError("Internal client error: " + e);
            }
        }

        [ContextMenu("End internal server")]
        public void StopInternalServer()
        {
            try
            {
                Server.Stop();
                Debug.Log("<color=yellow>Server stopped correctly</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Server error: " + e);
            }
        }

        [ContextMenu("Stop internal client")]
        public void StopInternalClient()
        {
            try
            {
                Client.Stop();
                Debug.Log("<color=yellow>Client stopped correctly</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Internal client error: " + e);
            }
        }
    }
}