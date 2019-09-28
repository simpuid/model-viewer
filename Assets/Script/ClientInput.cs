using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientInput : MonoBehaviour
{
    public string clientScene;
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
        StartCoroutine(CheckForConnection());
    }

    IEnumerator CheckForConnection()
    {
        if (Client.client == null)
            yield break;
        while (Client.client.Connecting)
        {
            yield return new WaitForEndOfFrame();
        }
        if (Client.client.Connected)
        {
            SceneManager.LoadScene(clientScene);
        }
        else
        {
            Error.ShowError(startScene, "Can't connect! Check config");
        }
    }
}
