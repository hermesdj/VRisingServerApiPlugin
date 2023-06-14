#nullable enable
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using VRisingServerApiPlugin.command;
using VRisingServerApiPlugin.http.security;
using VRisingServerApiPlugin.query;

namespace VRisingServerApiPlugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ApiPlugin : BasePlugin
{
    private Harmony? _harmony;
    private Component? _queryDispatcher;

# nullable disable
    internal static ManualLogSource Logger { get; private set; }
    public static ApiPlugin Instance { get; private set; }
#nullable enable

    public ApiPlugin()
    {
        Instance = this;
        Logger = Log;

        var authorizedUsers = Config.Bind<string>("Authentication", "AuthorizedUsers", "",
            "A list of comma separated username:password entries that defines the accounts allowed to query the API");

        HttpSecuritySingleton.GetInstance().Initialize(authorizedUsers);
    }

    public override void Load()
    {
        if (!ServerWorld.IsServer)
        {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} must be installed server side");
            return;
        }

        CommandRegistry.RegisterAll();

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