using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace VRisingServerApiPlugin.http.security;

public class HttpSecuritySingleton
{
    private static HttpSecuritySingleton _instance;

    private IEnumerable<AuthorizedUser> _authorizedUsers;

    public static HttpSecuritySingleton GetInstance()
    {
        return _instance ??= new HttpSecuritySingleton();
    }

    public void Initialize(ConfigEntry<string> authorizedUsers)
    {
        authorizedUsers.SettingChanged += (_, args) =>
        {
            var changedArgs = (SettingChangedEventArgs)args;
            var changedSettings = (ConfigEntry<string>)changedArgs.ChangedSetting;

            _authorizedUsers = GetAuthorizedUserList(changedSettings.Value);
        };
        _authorizedUsers = GetAuthorizedUserList(authorizedUsers.Value);
    }

    private static IEnumerable<AuthorizedUser> GetAuthorizedUserList(string authorizedUsers)
    {
        return AuthorizedUser.ParseConfig(authorizedUsers);
    }

    public bool CheckAuthenticationOfUser(string username, string password)
    {
        return _authorizedUsers
            .Count(user => user.Username.Equals(username) && user.Password.Equals(password)) == 1;
    }
}