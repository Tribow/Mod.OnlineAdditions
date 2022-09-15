﻿using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using Centrifuge.Distance.Game;
using Centrifuge.Distance.GUI.Data;
using Centrifuge.Distance.GUI.Controls;
using System;
using UnityEngine;

namespace Distance.OnlineAdditions
{
    /// <summary>
    /// The mod's main class containing its entry point
    /// </summary>
    [ModEntryPoint("Online Additions")]
    public sealed class Mod : MonoBehaviour
    {
        public static Mod Instance { get; private set; }

        public IManager Manager { get; private set; }

        public Log Logger { get; private set; }

        public ConfigLogic Config { get; private set; }

        public bool UploadScore { get; set; }

        /// <summary>
        /// Method called as soon as the mod is loaded.
        /// WARNING:	Do not load asset bundles/textures in this function
        ///				The unity assets systems are not yet loaded when this
        ///				function is called. Loading assets here can lead to
        ///				unpredictable behaviour and crashes!
        /// </summary>
        public void Initialize(IManager manager)
        {
            // Do not destroy the current game object when loading a new scene
            DontDestroyOnLoad(this);

            Instance = this;

            Manager = manager;

            Config = gameObject.AddComponent<ConfigLogic>();

            //Check whether or not leaderboard uploads can happen
            OnConfigChanged(Config);

            //Subcribe to config event
            Config.OnChanged += OnConfigChanged;

            // Create a log file
            Logger = LogManager.GetForCurrentAssembly();

            Logger.Info("Thanks for using Online Online Additions! Your one stop shop for everything multiplayer!");

            try
            {
                CreateSettingsMenu();
            }
            catch (Exception e)
            {
                Logger.Info(e);
                Logger.Info("This likely happened because you have the wrong version of Centrifuge.Distance. \nTo fix this, be sure to use the Centrifuge.Distance.dll file that came included with the mod's zip file. \nDespite this error, the mod will still function, however, you will not have access to the mod's menu.");
            }

            RuntimePatcher.AutoPatch();
        }

        private void CreateSettingsMenu()
        {
            MenuTree settingsMenu = new MenuTree("menu.mod.onlineadditions", "Online Additions Settings")
            {
                new CheckBox(MenuDisplayMode.Both, "settings:enable_onlinecheats", "ENABLE CHEATS IN MULTIPLAYER")
                .WithGetter(() => Config.EnableCheatsInMultiplayer)
                .WithSetter((x) => Config.EnableCheatsInMultiplayer = x)
                .WithDescription("Toggle whether all cheats can be enabled in multiplayer. (Turns off leaderboard uploads)"),

                new CheckBox(MenuDisplayMode.Both, "settings:hide_chat", "HIDE CHAT")
                .WithGetter(() => Config.HideChat)
                .WithSetter((x) => Config.HideChat = x)
                .WithDescription("Toggle whether the chat stays completely hidden in multiplayer or not."),

                new CheckBox(MenuDisplayMode.Both, "settings:hide_playername", "HIDE ONLINE PLAYER NAMES")
                .WithGetter(() => Config.HidePlayerNames)
                .WithSetter((x) => Config.HidePlayerNames = x)
                .WithDescription("Toggle whether online player names are visible."),

                new CheckBox(MenuDisplayMode.Both, "settings:enable_collision", "ENABLE ONLINE COLLISIONS")
                .WithGetter(() => Config.EnableCollision)
                .WithSetter((x) => Config.EnableCollision = x)
                .WithDescription("Toggle whether or not collisions are enabled when playing online. (Turns off leaderboard uploads)"),
            };

            Menus.AddNew(MenuDisplayMode.Both, settingsMenu, "ONLINE ADDITIONS", "Settings for the Online Additions mod");
        }

        public void OnConfigChanged(ConfigLogic config)
        {
            //If at any point collision is turned on, disable leaderboard upload
            if (config.EnableCollision || config.EnableCheatsInMultiplayer)
                UploadScore = false;
        }
    }
}



