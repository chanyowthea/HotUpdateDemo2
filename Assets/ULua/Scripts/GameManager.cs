using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LuaState luaState = new LuaState();
            luaState.Start();
            DelegateFactory.Init();
            LuaBinder.Bind(luaState);

            luaState.AddSearchPath(@"D:\FF\Test\TestLua\Assets\" + LuaConst._rootDir + "/Lua" + "/classA.lua");
            LuaFramework.LuaLoader loader = new LuaFramework.LuaLoader();
            loader.AddBundle("lua.unity3d");
            AssetBundleLoad.instance.LoadAssetBundle("lua");
            loader.beZip = true;
            luaState.DoFile("Main.lua");
            LuaFunction func = luaState.GetFunction("Main");
            func.Call();
            func.Dispose();
            func = null;

            luaState.CheckTop();
            luaState.Dispose();
            luaState = null;
        }
    }
}
