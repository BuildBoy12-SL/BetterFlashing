// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BetterFlashing
{
    using System.Collections.Generic;
    using Exiled.API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be shown.
        /// </summary>
        public bool ShowDebug { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum distance a flash grenade can affect a player in the facility.
        /// </summary>
        public float MaximumFacilityDistance { get; set; } = 50f;

        /// <summary>
        /// Gets or sets the maximum distance a flash grenade can affect a player on surface.
        /// </summary>
        public float MaximumSurfaceDistance { get; set; } = 50f;

        /// <summary>
        /// Gets or sets the multiplier of the intensity when the player is in a darkened room.
        /// </summary>
        public float DarkRoomMultiplier { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets the maximum amount of time that the flash effect can last for a player.
        /// </summary>
        public float MaximumFlashDuration { get; set; } = 15f;

        /// <summary>
        /// Gets or sets the maximum amount of time that the flash effect will last for an enraged Scp096.
        /// </summary>
        public byte MaxEnraged096Intensity { get; set; } = 10;

        /// <summary>
        /// Gets or sets a collection of points on a curve to calculate intensity based on position.
        /// </summary>
        public Dictionary<float, float> PowerOverDistance { get; set; } = new Dictionary<float, float>()
        {
            [0f] = 1f,
            [20.29813f] = 0.3458812f,
            [30f] = 0f,
        };

        /// <summary>
        /// Gets or sets a collection of points on a curve to calculate intensity based on rotation.
        /// </summary>
        public Dictionary<float, float> PowerOverLooking { get; set; } = new Dictionary<float, float>()
        {
            [-1f] = 1,
            [1f] = 0,
        };

        /// <summary>
        /// Gets or sets a collection of points on a curve to calculate intensity based on position to override on the distance plots.
        /// </summary>
        public Dictionary<float, float> PowerOverGuaranteedDistance { get; set; } = new Dictionary<float, float>()
        {
            [15f] = 0.001f,
            [0f] = 1f,
        };

        /// <summary>
        /// Parses all configs from their serializable forms to what is required.
        /// </summary>
        public void Validate()
        {
            Methods.PowerOverDistance = ConfigValidator.Curve(PowerOverDistance);
            Methods.PowerOverLooking = ConfigValidator.Curve(PowerOverLooking);
            Methods.PowerOverGuaranteedDistance = ConfigValidator.Curve(PowerOverGuaranteedDistance);
        }
    }
}