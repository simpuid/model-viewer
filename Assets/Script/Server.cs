﻿using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Bundle bundle;
    public static FileObject fileObject;
    public static Telepathy.Server server;
    public HashSet<int> connections;
    public int currentServerIndex;
    public SetPosition currentPosition;

    public void Awake()
    {
        Debug.Log("lol");
        connections = new HashSet<int>();
        currentServerIndex = 0;
        currentPosition = new SetPosition();
        currentPosition.x = 0;
        currentPosition.y = 0;
        currentPosition.z = 0;
        currentPosition.visible = false;
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
        byte[] secondData = DataParser.ObjecttoByteArray<SetPosition>(currentPosition);
        server.Send(connectionID, secondData);
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

    public void chamgePosition(float x, float y,float z,bool visible)
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
