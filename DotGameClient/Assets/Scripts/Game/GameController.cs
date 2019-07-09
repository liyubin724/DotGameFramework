using Dot.Core;
using Dot.XLuaEx;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public LuaAsset[] preloadLua;
    void Start()
    {
        GameApplication.GLua.InitLuaEnv(preloadLua);
        GameApplication.GLua.StartLuaEnv();
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        GameApplication.GLua.EndLuaEnv();
    }
}
