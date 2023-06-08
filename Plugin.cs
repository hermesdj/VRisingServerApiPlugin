#nullable enable
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using VRisingServerApiPlugin.query;

namespace VRisingServerApiPlugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
[Reloadable]
public class Plugin : BasePlugin
{
    internal static ManualLogSource? Logger;
    private Harmony? _harmony;
    private Component? _queryDispatcher;

    public override void Load()
    {
        if (!ServerWorld.IsServer)
        {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} must be installed server side");
            return;
        }

        Logger = Log;
        
        ClassInjector.RegisterTypeInIl2Cpp<QueryDispatcher>();
        _queryDispatcher = AddComponent<QueryDispatcher>();

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        if (_queryDispatcher != null)
        {
            Object.Destroy(_queryDispatcher);
        }

        return true;
    }
}
