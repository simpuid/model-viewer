using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    public static Bundle bundle;
    public static FileObject fileObject;
    public static Telepathy.Server server;
    public HashSet<int> connections;
    public int currentServerIndex;
    public SetPosition currentPosition;
    public Transform currentTransform;
    public GameObject header;
    public RectTransform panel;
    public Transform viewer;
    public float requiredSize;
    public HUDPoint hudPoint;
    private Text[] textArray;
    private GameObject[] objects;

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

    private void CreateMeshCollider(Transform t)
    {
        MeshFilter filter = t.GetComponent<MeshFilter>();
        if (filter != null)
        {
            MeshCollider c = t.gameObject.AddComponent<MeshCollider>();
            c.sharedMesh = filter.mesh;
        }
        for (int i = 0; i < t.childCount; i++)
        {
            CreateMeshCollider(t.GetChild(i));
        }
    }

    public void Awake()
    {
        connections = new HashSet<int>();
        hudPoint.Hide();
        currentPosition = new SetPosition();
        currentPosition.x = 0;
        currentPosition.y = 0;
        currentPosition.z = 0;
        currentPosition.visible = false;
        textArray = new Text[bundle.names.Length];
        for (int i = 0; i < textArray.Length; i++)
        {
            GameObject head = Instantiate(header);
            textArray[i] = head.GetComponent<Text>();
            head.GetComponent<RectTransform>().SetParent(panel);
            textArray[i].text = bundle.names[i];
            head.GetComponent<RectTransform>().localScale = Vector3.one;
        }
        objects = new GameObject[textArray.Length];
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
            CreateMeshCollider(t);
        }
        changeModel(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            changeModel(currentServerIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            changeModel(currentServerIndex + 1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitPoint;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitPoint, Mathf.Infinity))
            {
                currentPosition.visible = true;
                Vector3 pos = currentTransform.InverseTransformPoint(hitPoint.point);
                currentPosition.x = pos.x; currentPosition.y = pos.y; currentPosition.z = pos.z;
                hudPoint.Show();
            }
            else
            {
                currentPosition.visible = false;
                hudPoint.Hide();
            }
            changePosition(currentPosition.x, currentPosition.y, currentPosition.z, currentPosition.visible);
        }

        if (hudPoint.enable)
        {
            hudPoint.SetPoint(currentTransform.TransformPoint(new Vector3(currentPosition.x, currentPosition.y, currentPosition.z)));
        }

        if (server == null)
            return;
        if (!server.Active)
            return;
        Telepathy.Message msg;
        while (server.GetNextMessage(out msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    {
                        Debug.Log(msg.connectionId + " Connected");
                        connections.Add(msg.connectionId);
                        OnClientConnected(msg.connectionId);
                    }
                    break;
                case Telepathy.EventType.Disconnected:
                    {
                        Debug.Log(msg.connectionId + " Disconnected");
                        connections.Remove(msg.connectionId);
                    }
                    break;
            }
        }
    }

    public void OnClientConnected(int connectionID)
    {
        byte[] bytesArray = DataParser.ObjecttoByteArray<FileObject>(fileObject);
        server.Send(connectionID, bytesArray);
        SetModel model = new SetModel();
        model.index = currentServerIndex;
        byte[] firstData = DataParser.ObjecttoByteArray<SetModel>(model);
        server.Send(connectionID, firstData);
        byte[] secondData = DataParser.ObjecttoByteArray<SetPosition>(currentPosition);
        server.Send(connectionID, secondData);
    }

    public void changeModel(int index)
    {
        currentServerIndex = ((index % textArray.Length + textArray.Length) % textArray.Length);
        SetModel model = new SetModel();
        model.index = currentServerIndex;
        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i].color = (i == currentServerIndex) ? Color.yellow : Color.white;
        }
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == currentServerIndex);
        }
        currentTransform = objects[currentServerIndex].GetComponent<Transform>();
        currentPosition.visible = false;
        hudPoint.Hide();
        byte[] data = DataParser.ObjecttoByteArray<SetModel>(model);
        foreach (int id in connections)
            server.Send(id, data);
    }

    public void changePosition(float x, float y, float z, bool visible)
    {
        currentPosition.x = x;
        currentPosition.y = y;
        currentPosition.z = z;
        currentPosition.visible = visible;
        byte[] data = DataParser.ObjecttoByteArray<SetPosition>(currentPosition);
        foreach (int id in connections)
            server.Send(id, data);
    }
}
