using System.Collections.Generic;
using System.Linq;

namespace VRisingServerApiPlugin.http.security;

public class AuthorizedUser
{
    public string Username { get; set; }
    public string Password { get; set; }

    private AuthorizedUser(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public static IEnumerable<AuthorizedUser> ParseConfig(string authorizedUsers)
    {
        return (from user in authorizedUsers.Split(",")
            select user.Split(':')
            into parts
            where parts.Length == 2
            select new AuthorizedUser(parts[0].Trim(), parts[1].Trim())).ToList();
    }
}