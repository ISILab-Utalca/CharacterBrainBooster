using CBB.Comunication;
using CBB.DataManagement;
using UnityEngine;
using CBB.Lib;
public class CameraDataSender : MonoBehaviour
{
    public RenderTexture render;

    private float currentTime = 0f;
    public float deltatime = 1f/30;

    private Camera cameraTarget;

    private void Awake()
    {
        this.cameraTarget = GetComponent<Camera>();
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

        byte[] bytes = texture2D.EncodeToPNG();
        var wrapper = new CameraWraper(bytes);

        var data = JSONDataManager.SerializeData(wrapper);
        Server.SendMessageToAllClients(data);
    }

    
}

public static class ExtensionMethod
{
    private static Rect rect = new Rect(0, 0, 300, 300);
    private static Texture2D tex = new(300, 300, TextureFormat.RGB24, false);

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