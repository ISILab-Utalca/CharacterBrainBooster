using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
namespace CBB.Comunication
{
    public class NewClientNotifier : MonoBehaviour
    {
        private void Update()
        {
            if(Server.GetNewClientConnected(out TcpClient client)) 
            {
                Server.NotifyNewClienConnection(client);
            }

        }
    }

}
