using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private new Transform transform;
    private float z;
    public float speed;
    private new Camera camera;
    private void Awake()
    {
        transform = GetComponent<Transform>();
        camera = GetComponent<Camera>();
        z = camera.orthographicSize;
    }
    private void Update()
    {
        z += Input.GetAxis("Zoom")*Time.deltaTime*speed;
        z = Mathf.Max(z, 1f);
        camera.orthographicSize = z;
    }
}
