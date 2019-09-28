using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Bundle bundle;
    public static Telepathy.Client client;
    public string startScene;
    public float requiredSize;
    public HUDPoint hudPoint;
    private Vector3 localPos;
    private Transform currentTransform;

    public Transform viewer;
    GameObject[] objects;

    private void DestroyCamera(Transform t)
    {
        Camera c = t.GetComponent<Camera>();
        if (c != null)
        {
            Destroy(c);
        }
        for (int i = 0; i < t.childCount; i++)
        {
            DestroyCamera(t.GetChild(i));
        }
    }

    private void CalculateBound(Transform t, ref Bounds bnd, ref bool set)
    {
        Renderer rnd = t.GetComponent<Renderer>();
        if (rnd != null)
        {
            if (set)
                bnd.Encapsulate(rnd.bounds);
            else
            {
                bnd = rnd.bounds;
                set = true;
            }
        }
        for (int i = 0; i < t.childCount; i++)
        {
            CalculateBound(t.GetChild(i), ref bnd, ref set);
        }
    }

    private Bounds CalculateBound(Transform t)
    {
        Bounds b = new Bounds();
        bool set = false;
        CalculateBound(t, ref b, ref set);
        return b;
    }

    private void Awake()
    {
        objects = new GameObject[bundle.objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject model = Instantiate(bundle.objects[i]);
            Transform t = model.GetComponent<Transform>();
            t.SetParent(viewer);
            t.localScale = Vector3.one;
            t.localEulerAngles = Vector3.zero;
            DestroyCamera(t);
            objects[i] = model;
            Bounds bound = CalculateBound(t);
            float max = Mathf.Max(bound.size.x, bound.size.y, bound.size.z);
            if (Mathf.Approximately(max, 0f))
            {
                max = requiredSize;
            }
            float scale = requiredSize / max;
            t.localPosition = -bound.center * scale;
            t.localScale = Vector3.one * scale;
        }
    }

    private void process(SetModel setModel)
    {
        viewer.gameObject.SetActive(true);
        Debug.Log("got SetModel");
        for (int i = 0;i < objects.Length;i++)
        {
            objects[i].SetActive(i == setModel.index);
        }
        currentTransform = objects[setModel.index].GetComponent<Transform>();hudPoint.Hide();
    }

    private void process(SetPosition setPosition)
    {
        Debug.Log("got SetPosition"+setPosition.visible.ToString());
        if (setPosition.visible)
            hudPoint.Show();
        else
            hudPoint.Hide();
        localPos = new Vector3(setPosition.x, setPosition.y, setPosition.z);
    }

    private void process(FileObject fileObject)
    {
        Debug.Log("got FileObject");
    }

    private void Update()
    {
        if (hudPoint.enable && currentTransform != null)
        {
            hudPoint.SetPoint(currentTransform.TransformPoint(localPos));
        }
        if (client == null)
            return;
        if (!client.Connected)
        {
            Error.ShowError(startScene, "Server Disconnected");
            return;
        }

        Telepathy.Message msg;
        while (client.GetNextMessage(out msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    break;

                case Telepathy.EventType.Data:
                    SetPosition setPosition = DataParser.DeserializeObject<SetPosition>(msg.data);
                    if (setPosition != null)
                    {
                        process(setPosition);
                        break;
                    }
                    SetModel setModel = DataParser.DeserializeObject<SetModel>(msg.data);
                    if (setModel != null)
                    {
                        process(setModel);
                        break;
                    }
                    FileObject fileObject = DataParser.DeserializeObject<FileObject>(msg.data);
                    if (fileObject != null)
                    {
                        process(fileObject);
                        break;
                    }

                    break;
                case Telepathy.EventType.Disconnected:
                    Error.ShowError(startScene, "Server Disconnected");
                    break;
            }
        }
    }
}
