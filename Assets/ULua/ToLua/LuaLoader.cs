using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;
using HotFix;

namespace LuaFramework
{
    /// <summary>
    /// 集成自LuaFileUtils，重写里面的ReadFile，
    /// </summary>
    public class LuaLoader : LuaFileUtils
    {
        public LuaLoader()
        {
            instance = this;
        }

        /// <summary>
        /// 添加打入Lua代码的AssetBundle
        /// </summary>
        /// <param name="bundle"></param>
        public void AddBundle(string bundleName)
        {
            Context.CacheAddr = Path.Combine(Application.persistentDataPath, "contentcache/" + Context.PlatformIdentifier); 
            string url = Context.CacheAddr + "/" + Context.AssetBundlePrefix + "/" + bundleName.ToLower();
            if (File.Exists(url))
            {
                var bytes = File.ReadAllBytes(url);
                AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
                if (bundle != null)
                {
                    bundleName = bundleName.Replace("lua/", "").Replace(".unity3d", "");
                    base.AddSearchBundle(bundleName.ToLower(), bundle);
                }
            }
        }

        /// <summary>
        /// 当LuaVM加载Lua文件的时候，这里就会被调用，
        /// 用户可以自定义加载行为，只要返回byte[]即可。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            return base.ReadFile(fileName);
        }
    }
}