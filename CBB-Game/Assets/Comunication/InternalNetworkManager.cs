using CBB.Comunication;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InternalNetworkManager : MonoBehaviour
{
    private static int hEADER_SIZE = 4;

    public static int HEADER_SIZE { get => hEADER_SIZE;}

    [ContextMenu("Begin internal network")]
    public void Begin()
    {
        Application.quitting += End;
        try
        {
            Server.Start();
            Debug.Log("<color=green>Server started correctly</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Server error: " + e);
        }
        Thread.Sleep(0);
        try
        {
            Client.Start();
            Debug.Log("<color=green>Internal client started correctly</color>");

        }
        catch (System.Exception e)
        {
            Debug.LogError("Internal client error: " + e);
        }
        Debug.Log("<color=green>Internal connection set</color>");
    }
    [ContextMenu("End internal network")]
    public void End()
    {
        try
        {
            Client.Stop();
            Debug.Log("<color=green>Client stopped correctly</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Internal client error: " + e);
        }
        Thread.Sleep(0);
        try
        {
            Server.Stop();
            Debug.Log("<color=green>Server stopped correctly</color>");

        }
        catch (System.Exception e)
        {
            Debug.LogError("Server error: " + e);
        }
        Debug.Log("<color=green>Internal connection stoppped</color>");
    }
}
