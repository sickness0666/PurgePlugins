using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("PurgePlugins", "SiCkNeSs", "1.0.1")]
    [Description("Unload/Load plugins you specify for purge or other events")]
    partial class PurgePlugins : RustPlugin
    {
        #region Commands
        [ConsoleCommand("purge_plugins")]
        private void CmdUnload(ConsoleSystem.Arg arg)
        {
            if (!arg.IsRcon && arg.Connection != null && !arg.IsAdmin)
            {
                Puts("Not Admin");
                return;
            }

            //reload config incase its been changed since load
            LoadConfig();

            if (arg.Args == null || arg.Args.Length == 0 || arg.Args[0].ToLower() == "unload")
            {
                //unload plugins
                loadPlugins(configData.LoadPlugins, false);

                if (configData.BroadcastUnload)
                {
                    foreach (var broadcastMessage in configData.BroadcastMessagesUnload)
                    {
                        if (string.IsNullOrEmpty(configData.BroadcastPrefix))
                        {
                            Server.Broadcast(broadcastMessage);
                        }
                        else
                        {
                            Server.Broadcast(broadcastMessage, configData.BroadcastPrefix);
                        }
                    }
                }
            }
            else
            {
                if (arg.Args[0].ToLower() == "load")
                {
                    //load plugins
                    loadPlugins(configData.LoadPlugins);

                    if (configData.BroadcastUnload)
                    {
                        foreach (var broadcastMessage in configData.BroadcastMessagesLoad)
                        {
                            if (string.IsNullOrEmpty(configData.BroadcastPrefix))
                            {
                                Server.Broadcast(broadcastMessage);
                            }
                            else
                            {
                                Server.Broadcast(broadcastMessage, configData.BroadcastPrefix);
                            }
                        }
                    }
                }
            }

            //10 second delay to allow plugins to load/unload properly before reloading others
            timer.Once(configData.ReloadTime, () => reloadPlugins(configData.ReloadPlugins));
            
        }
        #endregion Commands

        #region Methods
        private void reloadPlugins(string[] plugins)
        {
            foreach (var item in plugins)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Puts("Reloading: " + item);
                    Interface.Oxide.ReloadPlugin(item);
                }
            }
        }
        private void loadPlugins(string[] plugins, bool load = true)
        {
            if (load)
            {
                //Loading plugins
                foreach (var item in plugins)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Puts("Loading: " + item);
                        Interface.Oxide.LoadPlugin(item);
                    }
                }

                //exit out of method
                return;
            }

            //unloading
            foreach (var item in plugins)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Puts("Unloading: " + item);
                    Interface.Oxide.UnloadPlugin(item);
                }
            }
        }
        #endregion Methods


        #region ConfigurationFile

        private ConfigData configData;

        private class ConfigData
        {
            [JsonProperty("Plugins to load/unload (Case Sensitive)")]
            public string[] LoadPlugins = { "TruePVE", "ZoneManager" };

            [JsonProperty("Plugins to reload (Case Sensitive)")]
            public string[] ReloadPlugins = { "PVxZoneStatus" };

            [JsonProperty("Seconds before reloading plugins from reload list (10 default)")]
            public int ReloadTime = 10;

            [JsonProperty("Broadcast Prefix")]
            public string BroadcastPrefix = "ADMIN:";

            [JsonProperty(PropertyName = "Send broadcasts on unload")]
            public bool BroadcastUnload = false;

            [JsonProperty("Messages to broadcast on plugin list unload")]
            public string[] BroadcastMessagesUnload = { "PURGE IS LIVE!", "PURGE IS LIVE!", "PURGE IS LIVE!" };

            [JsonProperty(PropertyName = "Send broadcasts on load")]
            public bool BroadcastLoad = false;

            [JsonProperty("Messages to broadcast on plugin list load")]
            public string[] BroadcastMessagesLoad = { "PURGE IS OVER!", "PURGE IS OVER!", "PURGE IS OVER!" };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                configData = Config.ReadObject<ConfigData>();
                if (configData == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                PrintError($"The configuration file is corrupted. \n{ex}");
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");
            configData = new ConfigData();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(configData);
        }

        #endregion ConfigurationFile

        #region Language
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["title1"] = "Purge",
            }, this);
        }
        #endregion Language
    }
}