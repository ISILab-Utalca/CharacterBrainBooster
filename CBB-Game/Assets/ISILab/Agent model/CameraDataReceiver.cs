using CBB.ExternalTool;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static CameraDataSender;

public class CameraDataReceiver : VisualElement
{
    readonly JsonSerializerSettings settings = new()
    {
        NullValueHandling = NullValueHandling.Include,
        MissingMemberHandling = MissingMemberHandling.Error,
        TypeNameHandling = TypeNameHandling.Auto,
    };

    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CameraDataReceiver, UxmlTraits> { }
    #endregion

    public Image image;

    public CameraDataReceiver()
    {
        image = new Image();
        this.Add(image);

        ExternalMonitor.OnMessageReceived += HandleMessage;
    }


    private void HandleMessage(string message)
    {
        CameraWraper pack = null;
        try
        {
            pack = JsonConvert.DeserializeObject<CameraWraper>(message, settings);
            var image = pack.image;

            var texture = new Texture2D(1024, 1024);
            texture.LoadImage(image);

            this.image.image = texture;
        }
        catch (Exception) { }
    }
}
