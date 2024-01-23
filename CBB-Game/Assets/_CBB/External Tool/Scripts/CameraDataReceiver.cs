using CBB.ExternalTool;
using Newtonsoft.Json;
using System;
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

    private VisualElement content;
    private Foldout foldout;

    private Image image;

    private Texture2D texture = new Texture2D(256, 256);

    public CameraDataReceiver()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("CameraDataReciver");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        foldout = this.Q<Foldout>();
        foldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            if(e.newValue)
            {
                this.style.height = 256 + 32;
                content.style.display = DisplayStyle.Flex;
            }
            else
            {
                this.style.height = 32;
                content.style.display = DisplayStyle.None;
            }
        });
        this.style.height = 256 + 32;

        image = new Image();
        image.style.display = DisplayStyle.Flex;
        image.style.flexGrow = 1;
        content.Add(image);

        ExternalMonitor.OnMessageReceived += HandleMessage;
    }

    
    private void HandleMessage(string message)
    {
        CameraWraper pack = null;
        try
        {
            pack = JsonConvert.DeserializeObject<CameraWraper>(message, settings);
            var image = pack.image;

            texture.LoadImage(image);
            this.image.image = texture;
        }
        catch (Exception) { }
    }
}
