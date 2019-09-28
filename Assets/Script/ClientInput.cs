using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientInput : MonoBehaviour
{
    public string nextScene;
    public string startScene;
    public int defaultPort;
    public InputField ipInput;
    public InputField portInput;
    public Button button;

    private void Start()
    {
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }

    public void OnClick()
    {
        Debug.Log(ipInput.text);
        Debug.Log(portInput.text);
        int port = defaultPort;
        int.TryParse(portInput.text, out port);
        string ip = ipInput.text;

        Client.client?.Disconnect();
        Client.client = new Telepathy.Client();
        Client.client.Connect(ip, port);
        StartCoroutine(InitializeConnection());
    }

    private void Update()
    {
        if (Client.client == null)
            return;
        if (!Client.client.Connected)
            return;

        Telepathy.Message msg;
        while (Client.client.GetNextMessage(out msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    Debug.Log("Connected Event");
                    Debug.Log(msg.connectionId + " Connected");
                    break;

                case Telepathy.EventType.Data:
                    Debug.Log("Data Event");
                    FileObject file = DataParser.DeserializeObject<FileObject>(msg.data);
                    BundleLoaderWeb.fileObject = file;
                    Debug.Log(file == null);
                    SceneManager.LoadScene(nextScene);
                    break;

                case Telepathy.EventType.Disconnected:
                    Error.ShowError(startScene, "Server Disconnected");
                    break;
            }
        }
    }

    IEnumerator InitializeConnection()
    {
        if (Client.client == null)
            yield break;
        while (Client.client.Connecting)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!Client.client.Connected)
        {
            Error.ShowError(startScene, "Can't connect! Check config");
        }
        Debug.Log("Client Connected");
    }
}
