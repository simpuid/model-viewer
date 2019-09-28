using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BundleLoaderWeb : MonoBehaviour
{
    public string nextScene;
    public string startScene;
    bool receiveMessage = true;

    private void Update()
    {
        if (Client.client == null)
            return;
        if (!Client.client.Connected)
            return;
        if (!receiveMessage)
            return;

        Telepathy.Message msg;
        while (receiveMessage && Client.client.GetNextMessage(out msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    break;

                case Telepathy.EventType.Data:
                    FileObject file = DataParser.DeserializeObject<FileObject>(msg.data);
                    if (file != null)
                    {
                        receiveMessage = false;
                        StartCoroutine(Load(file));
                    }
                    break;
                case Telepathy.EventType.Disconnected:
                    Error.ShowError(startScene, "Server Disconnected");
                    break;
            }
        }
    }

    private IEnumerator Load(FileObject file)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(file.url, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (bundle == null)
        {
            Error.ShowError(null, "Can't download bundle");
            yield break;
        }
        GameObject[] objects = new GameObject[file.names.Length];
        for (int i = 0; i < file.names.Length; i++)
        {
            objects[i] = bundle.LoadAsset<GameObject>(file.names[i]);
            if (objects[i] == null)
            {
                Error.ShowError(null, "Model load failed.\nname:" + file.names[i]);
                yield break;
            }
        }
        Client.bundle = new Bundle(objects, file.names);
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}

