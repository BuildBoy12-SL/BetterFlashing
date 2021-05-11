// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BetterFlashing
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        private static readonly Plugin InstanceValue = new Plugin();
        private Harmony harmony;

        private Plugin()
        {
        }

        /// <summary>
        /// Gets a static instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; } = InstanceValue;

        /// <inheritdoc/>
        public override string Author { get; } = "Build";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 9, 4);

        /// <inheritdoc/>
        public override Version Version { get; } = new Version(1, 0, 0);

        /// <summary>
        /// Gets an instance of the <see cref="Methods"/> class.
        /// </summary>
        public Methods Methods { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Methods = new Methods(this);

            Config.Validate();
            Exiled.Events.Handlers.Server.ReloadedConfigs += Config.Validate;

            harmony = new Harmony($"build.betterFlashing.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Methods = null;

            Exiled.Events.Handlers.Server.ReloadedConfigs -= Config.Validate;

            harmony?.UnpatchAll();
            base.OnDisabled();
        }

        /// <summary>
        /// Sends a debug message which respects <see cref="Config.ShowDebug"/>.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        internal void SendDebug(object message) => Log.Debug(message, Config.ShowDebug);
    }
}