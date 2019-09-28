using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Bundle bundle;
    public static Telepathy.Client client;
    public string startScene;

    private void process(SetModel setModel)
    {
        Debug.Log("got SetModel");

    }

    private void process(SetPosition setPosition)
    {
        Debug.Log("got SetPosition");

    }

    private void process(FileObject fileObject)
    {
        Debug.Log("got FileObject");
    }

    private void Update()
    {
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
