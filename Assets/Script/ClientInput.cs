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
        Debug.Log("1");
        StartCoroutine(InitializeConnection());
    }



    IEnumerator InitializeConnection()
    {
        //while (Client.client.Connecting)
        //{
        //    yield return new WaitForEndOfFrame();
        //}
        //if (!Client.client.Connected)
        //{
        //    Error.ShowError(startScene, "Can't connect! Check config");
        //}
        //else
        //{
        //    SceneManager.LoadScene(nextScene);
        //}
        int port = defaultPort;
        int.TryParse(portInput.text, out port);
        string ip = ipInput.text;
        Client.client?.Disconnect();
        Client.client = new Telepathy.Client();
        Client.client.Connect(ip, port);
        Debug.Log("conn");

        Telepathy.Message msg;
        while (!Client.client.GetNextMessage(out msg))
        {
            yield return new WaitForEndOfFrame();
        }
        switch (msg.eventType)
        {
            case Telepathy.EventType.Connected:
                SceneManager.LoadScene(nextScene);
                Debug.Log("connected");
                yield break;
            case Telepathy.EventType.Disconnected:
                Error.ShowError(startScene, "Can't connect! Check config");
                Debug.Log("dis");
                yield break;
        }
    }
}
