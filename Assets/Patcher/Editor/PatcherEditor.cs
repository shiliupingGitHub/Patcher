using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
public class PatcherEditor : Editor {

	[MenuItem("Patcher/PC/BuildAsset")]
    static void BuildPCAsset()
    {
        BuildAsset(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Patcher/Android/BuildAsset")]
    static void BuildAndroidAsset()
    {
        BuildAsset(BuildTarget.Android);
    }
    [MenuItem("Patcher/IOS/BuildAsset")]
    static void BuildIOSAsset()
    {
        BuildAsset(BuildTarget.StandaloneWindows);
    }
    static void BuildAsset(BuildTarget target)
    {
        string Path = string.Empty;
        switch (target)
        {
            case BuildTarget.Android:
                Path = Patcher.GetABsPath(RuntimePlatform.Android);
                break;
            case BuildTarget.iOS:
                Path = Patcher.GetABsPath(RuntimePlatform.IPhonePlayer);
                break;
            default:
                Path = Patcher.GetABsPath(RuntimePlatform.WindowsPlayer);
                break;
        }
        string outPath = "Assets/Patcher/ABs/" + Path;
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, target);
        if(null != manifest)
        {
           
            Patcher.PatcherElem elem = new Patcher.PatcherElem();
            string[] abs = manifest.GetAllAssetBundles();
            foreach(var ab in abs)
            {
                Patcher.PatcherElem.Elem e = new Patcher.PatcherElem.Elem();
                e.szName = ab;
                e.mVersion = manifest.GetAssetBundleHash(ab).ToString();
                e.mDepends = manifest.GetAllDependencies(ab);
                elem.AddElem(e);
            }
            string manifestPath = Application.dataPath + "/Patcher/ABs/" + Path;
            elem.Serialize(manifestPath + "/Pather.txt");
        }
        AssetDatabase.Refresh();
    }
}
