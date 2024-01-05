using CBB.Comunication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class CameraDataSender : MonoBehaviour
{
    public RenderTexture render;

    private float currentTime = 0f;
    public float deltatime = 0.5f;

    private Camera cameraTarget;

    private void Awake()
    {
        this.cameraTarget = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime >= deltatime)
        {
            currentTime = 0f;
            SendImage();
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    private void SendImage()
    {
        // get the image from the camera
        cameraTarget.targetTexture = render;
        cameraTarget.Render();
        var texture2D = render.toTexture2D();
        cameraTarget.targetTexture = null;

        Texture texture = texture2D;

        byte[] bytes = texture2D.EncodeToPNG();
        var wrapper = new CameraWraper(bytes);

        var data = JSONDataManager.SerializeData(wrapper);
        Server.SendMessageToAllClients(data);
    }

    [System.Serializable]
    public class CameraWraper
    {
        [SerializeField]
        public byte[] image;

        public CameraWraper(byte[] image)
        {
            this.image = image;
        }
    }
}

public static class ExtensionMethod
{
    private static Rect rect = new Rect(0, 0, 300, 300);
    private static Texture2D tex = new Texture2D(300, 300, TextureFormat.RGB24, false);

    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(rect, 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }
}