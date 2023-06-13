#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using VRisingServerApiPlugin.command;
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

    private ConfigEntry<string> _authorizedUsers;

    public ApiPlugin() : base()
    {
        Instance = this;
        Logger = Log;

        _authorizedUsers = Config.Bind("Authentication", "AuthorizedUsers", "",
            "A list of comma separated username:password entries that defines the accounts allowed to query the API");
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

    public List<AuthorizedUser> GetAuthorizedUserList()
    {
        return AuthorizedUser.ParseConfig(this._authorizedUsers.Value);
    }

    public bool CheckAuthenticationOfUser(string username, string password)
    {
        return GetAuthorizedUserList()
            .Count(user => user.Username.Equals(username) && user.Password.Equals(password)) == 1;
    }

    public class AuthorizedUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private AuthorizedUser(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static List<AuthorizedUser> ParseConfig(string authorizedUsers)
        {
            return (from user in authorizedUsers.Split(",")
                select user.Split(':')
                into parts
                where parts.Length == 2
                select new AuthorizedUser(parts[0].Trim(), parts[1].Trim())).ToList();
        }
    }
}