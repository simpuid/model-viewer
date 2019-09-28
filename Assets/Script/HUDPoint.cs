using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPoint : MonoBehaviour
{
    public RectTransform point;
    public bool enable;
    public void SetPoint(Vector3 worldPoint)
    {
        point.gameObject.SetActive(true);
        Camera main = Camera.main;
        Vector2 pos = main.WorldToScreenPoint(worldPoint);
        point.anchorMin = point.anchorMax = new Vector2(pos.x / main.scaledPixelWidth, pos.y / main.scaledPixelHeight);
    }

    public void Hide()
    {
        point.gameObject.SetActive(false);
        enable = false;
    }

    public void Show()
    {
        point.gameObject.SetActive(true);
        enable = true;
    }

    public void Update()
    {
        point.localScale = Vector3.one * (1f+0.4f*Mathf.Sin(Time.time*8));
    }
}
