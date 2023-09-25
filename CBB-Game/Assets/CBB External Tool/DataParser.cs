using CBB.Api;
using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    public Action<InternalMessage> OnInternalMessageReceived { get; set; }

    // Set initial settings for detecting errors on deserialization
    readonly JsonSerializerSettings settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Error
    };
    #region Events
    public static Action OnClientConnected { get; set; }
    #endregion
    public void HandleMessage(string msg)
    {
        
        
    }
    
}
