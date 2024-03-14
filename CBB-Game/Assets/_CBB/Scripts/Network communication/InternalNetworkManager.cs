using UnityEngine;

namespace CBB.Comunication
{
    public enum InternalMessage
    {
        CLIENT_CONNECTED,
        CLIENT_STOPPED,
        SERVER_STOPPED,
    }
    /// <summary>
    /// Open or close the server when the application needs it
    /// </summary>
    public class InternalNetworkManager : MonoBehaviour
    {
        /// <summary>
        /// The size used by the Length-Prefix message framing
        /// protocol implemented in this solution
        /// </summary>
        public static int HEADER_SIZE { get; } = 4;

        /// <summary>
        /// Subscribe to process a message when it is received by the server
        /// </summary>
        public static System.Action<string> OnServerMessageDequeued { get; set; }
        private void Awake()
        {
            Application.quitting += StopInternalServer;
            StartServer();
        }
        private void Update()
        {
            if (!Server.IsRunning) return;
            if (Server.ReceivedMessages.Count == 0) return;
            var msg = Server.ReceivedMessages.Dequeue();
            OnServerMessageDequeued?.Invoke(msg);
        }
        private void StartServer()
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
    }
}