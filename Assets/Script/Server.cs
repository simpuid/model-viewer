using System.Collections;
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
    public GameObject header;
    public RectTransform panel;
    private Text[] textArray;



    public void Awake()
    {
        connections = new HashSet<int>();
        currentServerIndex = 0;
        textArray = new Text[bundle.names.Length];
        for (int i = 0;i < textArray.Length; i++)
        {
            GameObject head = Instantiate(header);
            textArray[i] = head.GetComponent<Text>();
            head.GetComponent<RectTransform>().SetParent(panel);
            textArray[i].text = bundle.names[i];
            head.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    public void Update()
    {
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
    }
}
