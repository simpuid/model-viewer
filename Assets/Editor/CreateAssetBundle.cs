using UnityEditor;
using System.IO;

public class CreateAssetBundle
{
    [MenuItem("Assets/Build Bundle")]
    static void BuildAllAssetBundlesAndroid()
    {
        string assetBundleDirectoryAndroid = "Assets/AssetBundles/Android";
        if (!Directory.Exists(assetBundleDirectoryAndroid))
        {
            Directory.CreateDirectory(assetBundleDirectoryAndroid);
        }

        string assetBundleDirectoryWindows = "Assets/AssetBundles/Windows";
        if (!Directory.Exists(assetBundleDirectoryWindows))
        {
            Directory.CreateDirectory(assetBundleDirectoryWindows);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryWindows, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles(assetBundleDirectoryAndroid, BuildAssetBundleOptions.None, BuildTarget.Android);

    }
}
