using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Bundle bundle;
    public static FileObject fileObject;
    public static Telepathy.Server server;
    public HashSet<int> connections;
    public int currentServerIndex;

    public void Awake()
    {
        Debug.Log("lol");
        connections = new HashSet<int>();
        currentServerIndex = 0;
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

    public void changeModel(int index)
    {
        currentServerIndex = index;
        SetModel model = new SetModel();
        model.index = currentServerIndex;
        byte[] data = DataParser.ObjecttoByteArray<SetModel>(model);
        foreach(int id in connections)
            server.Send(id, data);
    }
}
