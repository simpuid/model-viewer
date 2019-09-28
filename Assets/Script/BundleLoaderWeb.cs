using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BundleLoaderWeb : MonoBehaviour
{
    public static FileObject fileObject;

    public string nextScene;

    private void Awake()
    {
        StartCoroutine(Load(fileObject));
    }

    private IEnumerator Load(FileObject file)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(file.url, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
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
