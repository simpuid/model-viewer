using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private new Transform transform;
    private float z;
    public float speed;
    private void Awake()
    {
        transform = GetComponent<Transform>();
        z = transform.position.z;
    }
    private void Update()
    {
        z += Input.GetAxis("Zoom")*Time.deltaTime*speed;
        z = Mathf.Min(z, 0f);
        Debug.Log(Input.GetAxis("Zoom"));
        transform.position = new Vector3(0, 0, z);
    }
}
