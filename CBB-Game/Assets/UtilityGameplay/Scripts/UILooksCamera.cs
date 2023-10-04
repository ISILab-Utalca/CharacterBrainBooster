using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILooksCamera : MonoBehaviour
{
    new Camera camera;
    private void Start()
    {
        camera = Camera.main;
    }
    void Update()
    {
        
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
