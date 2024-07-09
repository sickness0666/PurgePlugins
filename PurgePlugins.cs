using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("PurgePlugins", "SiCkNeSs", "1.0.0")]
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
                loadPlugins(configData.LoadPlugins, false);
            }
            else
            {
                if (arg.Args[0].ToLower() == "load")
                {
                    loadPlugins(configData.LoadPlugins);
                }
            }

            //10 second delay to allow plugins to load/unload properly before reloading others
            timer.Once(10, () => reloadPlugins(configData.ReloadPlugins));
            
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