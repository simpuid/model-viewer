using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerController : MonoBehaviour
{
    public float horizontalSpeed;
    public float verticalSpeed;

    private new Transform transform;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Vertical")*verticalSpeed*Time.deltaTime, Input.GetAxis("Horizontal")*horizontalSpeed*Time.deltaTime, 0), Space.World);
    }
}
