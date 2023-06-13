# VRisingServerApiPlugin

VRising Server API used to expose new HTTP Routes

### Configuring the server

In the ServerHostSettings.json file of the VRising server, you have to activate the API :

```json
{
  "API": {
    "Enabled": true,
    "BindAddress": "*",
    "BindPort": 9090,
    "BasePath": "/",
    "AccessList": "",
    "PrometheusDelay": 30
  }
}
```

Then, you have to put the DLL of this plugin in the plugins directory of the BepInEx folder on your server. In the logs,
you will be able to see the new routes created.
You can then call the routes using another program or something like Postman to query the available endpoints.

### How it works

You can find two working examples on how the endpoints are exposed through the VRising Server HTTP API in the endpoints
folder of the project.
I have implemented a few basic GET and POST methods for two endpoints : clans and players

The API is capable of parsing url params, query params and JSON Bodies. Only JSON bodies are supported and the
Content-Type=application/json must be present in the header of the HTTP request.

For url params, I am using regex groups to match by name.

For the endpoint /players/:id where id is an integer that match a userIndex in the game, the endpoint is declared the
following way :

```csharp
[HttpGet("/(?<id>[0-9]*)")]
public PlayerApiResponse GetPlayerDetails([UrlParam("id")] int userIndex)
```

The regex use a group called id that match any number, and it is mapped to the userIndex parameter of the method by
using the "id" param name.

The plugin will try to cast the url params, query params and request body to the parameter type of the method. See for
example the update clan name method :

```csharp
[HttpPost(@"/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/updateName")]
public GetClanResponse UpdateClanName([UrlParam("id")] Guid clanId, [RequestBody] UpdateClanNameBody? body)
```

The url param id is parsed as a Guid, and the RequestBody is used as the base class to deserialize the JSON of the body
received. The regex used in the HttpPost definition is a regex that match an uuid.

### Using the plugin as a dependency

The CommandRegistry expose a public method RegisterAll. Import the DLL of the project as a dependency to your own
plugin,
then declare some API endpoints using the Attributes declarations explained above, and then call the following method to
register your assembly :

```csharp
CommandRegistry.RegisterAll();
```

### Thanks

- Infinite thanks to [DarkAtra project on his discord bot](https://github.com/DarkAtra/v-rising-discord-bot-companion)
  for the inspiration on how to do this
- Thanks to deca for all his work and for the github workflows I used
  from [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework/)