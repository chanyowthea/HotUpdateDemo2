#if SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileUtil
{
    public static void WriteFileInfo()
    {
        string path = HotFix.Context._assetBundlePath; 
        string[] names = Directory.GetFiles(path);
        
        // 加载StreamingAssets的AssetBundle
        AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(path + HotFix.Context.AssetBundlePrefix + HotFix.Context._assetBundleSuffix);
        // 加载AssetBundleManifest
        var manifest = (AssetBundleManifest)manifestAssetBundle.LoadAsset("AssetBundleManifest");

        string fileInfoPath = HotFix.Context._localFileInfoPath;
        if (!Directory.Exists(fileInfoPath))
        {
            Directory.CreateDirectory(Directory.GetParent(fileInfoPath).FullName); 
        }
        using (FileStream fs = new FileStream(fileInfoPath, FileMode.Create))
        {
            StreamWriter r = new StreamWriter(fs);
            string format = "{0},{1},{2},{3}";

            string s0 = path + HotFix.Context.AssetBundlePrefix;
            FileInfo info0 = new FileInfo(s0);
            r.WriteLine(string.Format(format, HotFix.Context.AssetBundlePrefix, GetFileHash(s0), info0.Length, 0));

            s0 = path + HotFix.Context.AssetBundlePrefix + ".manifest";
            info0 = new FileInfo(s0);
            r.WriteLine(string.Format(format, HotFix.Context.AssetBundlePrefix + ".manifest", GetFileHash(s0), info0.Length, 0));

            for (int i = 0, length = names.Length; i < length; i++)
            {
                var n = Path.GetFileName(names[i]);
                if (Path.GetExtension(n) != "" || n == HotFix.Context.AssetBundlePrefix)
                {
                    continue;
                }
                FileInfo info = new FileInfo(names[i]);
                string hash = manifest.GetAssetBundleHash(n).ToString();
                long len = info.Length;
                byte status = 0;
                r.WriteLine(string.Format(format, n, hash, len, status));
            }
            r.Flush();
        }

        string versionInfoPath = HotFix.Context._localVersionInfoPath;
        using (FileStream fs = new FileStream(versionInfoPath, FileMode.Create))
        {
            byte[] bs = Encoding.UTF8.GetBytes(Application.version);
            fs.Write(bs, 0, bs.Length); 
        }
    }

    public static List<string> Recursive(string path)
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

    public static string GetFileHash(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            int len = (int)fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach (byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            return fileMD5;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);
            return "";
        }
    }
}
#endif
