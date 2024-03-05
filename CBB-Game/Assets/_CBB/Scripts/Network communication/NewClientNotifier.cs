using System.Collections;
using System.Net.Sockets;
using UnityEngine;
namespace CBB.Comunication
{
    public class NewClientNotifier : MonoBehaviour
    {
        [SerializeField, Tooltip("How often this scripts checks for new connected clients")]
        private float checkTime = 1;

        private WaitForSeconds wfs = new(1);
        private IEnumerator Start()
        {
            wfs = new WaitForSeconds(checkTime);
            while (true)
            {
                if (Server.GetNewClientConnected(out TcpClient client))
                {
                    Server.NotifyNewClienConnection(client);
                }
                yield return wfs;
            }
        }
    }

}
