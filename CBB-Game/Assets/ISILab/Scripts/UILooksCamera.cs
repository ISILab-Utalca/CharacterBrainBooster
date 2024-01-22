using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILooksCamera : MonoBehaviour
{
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
