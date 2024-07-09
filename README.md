
# PurgePlugins

Plugin made for the game Rust. Allows for you to specify a list of plugins that can all be loaded/unloaded from a single command. Also allows for you to send a list of broadcast messages after the plugins are loaded/unloaded.

Use case example: Purge on a PVE server. This can also be done via other plugins, but this was a simple plugin custom made for a friends server, for free.




## Authors

- [@sickness0666](https://github.com/sickness0666)/[@sickness01](https://codefling.com/sickness01)


## Commands

#### Unload plugins from config list (from RCON, or admin F1 console)

```http
  purge_plugins  OR   purge_plugins unload
```

#### Load plugins from config list (from RCON, or admin F1 console)

```http
  purge_plugins load
```
## Default Config

```json
{
  "Plugins to load/unload (Case Sensitive)": [
    "TruePVE",
    "ZoneManager"
  ],
  "Plugins to reload (Case Sensitive)": [
    "PVxZoneStatus"
  ],
  "Seconds before reloading plugins from reload list (10 default)": 10,
  "Broadcast Prefix": "ADMIN",
  "Send broadcasts on unload": false,
  "Messages to broadcast on plugin list unload": [
    "PURGE IS LIVE!",
    "PURGE IS LIVE!",
    "PURGE IS LIVE!"
  ],
  "Send broadcasts on load": false,
  "Messages to broadcast on plugin list load": [
    "PURGE IS OVER!",
    "PURGE IS OVER!",
    "PURGE IS OVER!"
  ]
}
```


## Contributing

Contributions are always welcome!

