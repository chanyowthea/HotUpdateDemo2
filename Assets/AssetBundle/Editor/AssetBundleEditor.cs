using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditor
{
    [MenuItem("Tools/BuildBundles")]
    static void BuildAllAssetBundles()
    {
        BuildAssetBundles(BuildTarget.StandaloneWindows64);
        BuildAssetBundles(BuildTarget.Android);
        //BuildAssetBundles(BuildTarget.iOS);
    }

    static void BuildAssetBundles(BuildTarget target)
    {
        //第一个参数获取的是AssetBundle存放的相对地址
        string path = Application.streamingAssetsPath + "/" + GetDirByBuildTarget(target) + "/" + HotFix.Context.AssetBundlePrefix + "/";
        Debug.Log("path=" + path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path); 
        }

        BuildPipeline.BuildAssetBundles(
         path,
         BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle,
         target);

        var list = Recursive(path);
        for (int i = 0, length = list.Count; i < length; i++)
        {
            var tempName = list[i];
            if (string.IsNullOrEmpty(Path.GetExtension(tempName)) || (Path.GetExtension(tempName) == ".assetbundle"))
            {
                FileInfo info = new FileInfo(tempName);
                string destPath = tempName + HotFix.Context._assetBundleSuffix;
                if (File.Exists(destPath))
                {
                    File.Delete(destPath); 
                }
                info.MoveTo(destPath);
            }
        }

#if SERVER
        FileUtil.WriteFileInfo();
#endif
    }

    static List<string> Recursive(string path)
    {
        List<string> files = new List<string>();

        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        // 获取本目录子一级文件
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }

        // 遍历子二级文件夹
        foreach (string dir in dirs)
        {
            var l = Recursive(dir);
            if (l.Count > 0)
            {
                files.AddRange(l);
            }
        }
        return files;
    }

    static public string GetDirByBuildTarget(BuildTarget target)
    {
        if (target == BuildTarget.Android)
        {
            return "android";
        }
        else if (target == BuildTarget.iOS)
        {
            return "ios";
        }
        else if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows)
        {
            return "win";
        }
        else
        {
            return "unsupported";
        }
    }

}
