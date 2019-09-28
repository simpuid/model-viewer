using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleLoaderFile : MonoBehaviour
{
    public string bundleName;
    public string objectOrder;

    private void Awake()
    {
        StartCoroutine(Load(bundleName, objectOrder));
    }

    private IEnumerator Load(string bundleName,string orderFileName)
    {
        yield return new WaitForEndOfFrame();
        Error.ShowError(null, "Error");
    }
}
