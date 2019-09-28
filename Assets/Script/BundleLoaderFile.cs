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
        string path = Application.dataPath;
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(path, bundleName));
        if (bundle == null)
        {
            Error.ShowError(null, "Bundle load fail:\n"+ Path.Combine(path, bundleName));
            yield break;
        }
        FileObject file = FileReader.ReadFile(Path.Combine(path, orderFileName));
        if (file == null)
        {
            Error.ShowError(null, "ModelList load fail\n" + Path.Combine(path, orderFileName));
            yield break;
        }
        GameObject[] objects = new GameObject[file.names.Length];
        for (int i = 0;i < file.names.Length; i++)
        {
            objects[i] = bundle.LoadAsset<GameObject>(file.names[i]);
            if (objects[i] == null)
            {
                Error.ShowError(null, "Model load failed.\nname:" + file.names[i]);
                yield break;
            }
        }
        Bundle finalBundle = new Bundle(objects, file.names);
        Server.bundle = finalBundle;
        Server.fileObject = file;
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
