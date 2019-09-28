using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class BundleLoaderFile : MonoBehaviour
{
    public string bundleName;
    public string objectOrder;
    public string nextScene;

    private void Awake()
    {
        StartCoroutine(Load(bundleName, objectOrder));
    }

    private IEnumerator Load(string bundleName,string orderFileName)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, bundleName));
        Debug.Log(Path.Combine(Application.streamingAssetsPath, bundleName));
        if (bundle == null)
        {
            Debug.Log("bundle loading failed");
            Error.ShowError(null, "Can't load file\n"+ Path.Combine(Application.dataPath, bundleName));
            yield break;
        }
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
