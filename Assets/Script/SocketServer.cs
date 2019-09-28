using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocketServer : MonoBehaviour
{
    public int port;
    public string nextScene;
    private void Awake()
    {
        Application.runInBackground = true;

        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;

        Server.server = new Telepathy.Server();
        Server.server.Start(port);
    }

    private void Update()
    {
        if (Server.server != null)
        {
            if (Server.server.Active)
            {
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            }
        }
    }
}
