using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.HelloAR;

public class ARViewer : MonoBehaviour
{
    public float scale;
    public HelloARController helloARContorller;
    private new Transform transform;
    private PanAndZoom panAndZoom;
    private void Awake()
    {
        transform = GetComponent<Transform>();
        panAndZoom = gameObject.AddComponent<PanAndZoom>();
        panAndZoom.onTap += OnTap;
        panAndZoom.onSwipe += OnSwipe;
        panAndZoom.onPinch += OnPinch;
    }

    private void OnSwipe(Vector2 swipe)
    {
        float avg = (Camera.main.pixelHeight + Camera.main.pixelWidth) * 0.5f;
        transform.Rotate(new Vector3(swipe.y/ avg * 360, -swipe.x/ avg * 360, 0), Space.World);
    }

    private void OnPinch(float start,float end)
    {
        scale = Mathf.Max(0.01f, (end - start)/Camera.main.scaledPixelWidth+scale);
    }

    private void OnTap(Vector2 pos)
    {
        helloARContorller.Place(pos);
        Debug.Log("tap");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * scale;
        transform.localPosition = new Vector3(0, scale * 0.5f, 0f);
    }
}
