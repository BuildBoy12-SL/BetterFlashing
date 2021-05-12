// -----------------------------------------------------------------------
// <copyright file="Methods.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BetterFlashing
{
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Grenades;
    using UnityEngine;

    /// <summary>
    /// A central class to manage all resources needed to override the base flash system.
    /// </summary>
    public class Methods
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Methods"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public Methods(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// Gets the curve which handles the logic for a players position relative to a flash grenade.
        /// </summary>
        public static AnimationCurve PowerOverDistance { get; internal set; }

        /// <summary>
        /// Gets the curve which handles the logic for a players rotation relative to a flash grenade.
        /// </summary>
        public static AnimationCurve PowerOverLooking { get; internal set; }

        /// <summary>
        /// Gets the curve which handles the logic for when a players position reaches a threshold to a grenade.
        /// </summary>
        public static AnimationCurve PowerOverGuaranteedDistance { get; internal set; }

        /// <summary>
        /// Handles the <see cref="Flashed"/> and <see cref="Deafened"/> effects of all affected players.
        /// </summary>
        /// <param name="flashGrenade">The <see cref="FlashGrenade"/> that exploded.</param>
        /// <param name="thrower">The <see cref="Player"/> that threw the <see cref="FlashGrenade"/>.</param>
        public void RunFlash(FlashGrenade flashGrenade, Player thrower)
        {
            Vector3 grenadePosition = flashGrenade.transform.position;
            plugin.SendDebug($"Flash grenade exploded at {grenadePosition}.");
            foreach (Player player in Player.List)
            {
                if (!IsFlashable(player, thrower, flashGrenade))
                    continue;

                bool isSurface = player.CurrentRoom.Type == RoomType.Surface;
                float maxDistance = isSurface ? plugin.Config.MaximumSurfaceDistance : plugin.Config.MaximumFacilityDistance;
                plugin.SendDebug($"Calculated maximum flash distance for {player.Nickname}: {maxDistance}");

                float distance = Vector3.Distance(player.Position, grenadePosition);
                plugin.SendDebug($"{player.Nickname} is standing {distance} HU away from the flash grenade.");

                if (distance > maxDistance)
                {
                    plugin.SendDebug($"{player.Nickname} is out of range of the flash grenade.");
                    continue;
                }

                float lookingAmount = Vector3.Dot(player.CameraTransform.forward, (player.CameraTransform.position - grenadePosition).normalized);
                plugin.SendDebug($"Looking amount: {lookingAmount}");

                float flashIntensity = PowerOverDistance.Evaluate(distance);
                plugin.SendDebug($"Intensity as determined by {nameof(PowerOverDistance)} curve: {flashIntensity}");

                if (player.ReferenceHub.localCurrentRoomEffects.syncFlicker)
                {
                    flashIntensity *= plugin.Config.DarkRoomMultiplier;
                    plugin.SendDebug($"Player is in dark room, applying dark room multiplier. New time: {flashIntensity}");
                }

                float powerOverLooking = PowerOverLooking.Evaluate(lookingAmount);
                plugin.SendDebug($"Intensity as determined by {nameof(PowerOverLooking)} curve: {powerOverLooking}");

                float combined = powerOverLooking * flashIntensity;
                plugin.SendDebug($"Combined intensity: {combined}.");

                float guaranteed = PowerOverGuaranteedDistance.Evaluate(distance);
                if (powerOverLooking < guaranteed)
                    combined = guaranteed;

                float intensity = combined * 10f * plugin.Config.MaximumFlashDuration;

                byte clampedIntensity;
                if (player.CurrentScp is PlayableScps.Scp096 scp096 && scp096.Enraged)
                    clampedIntensity = (byte)Mathf.Clamp(Mathf.RoundToInt(intensity), 1, plugin.Config.MaxEnraged096Intensity);
                else
                    clampedIntensity = (byte)Mathf.Clamp(Mathf.RoundToInt(intensity), 1, 255);

                plugin.SendDebug($"Intensity clamped as necessary. New intensity: {clampedIntensity}");

                player.ChangeEffectIntensity<Flashed>(clampedIntensity);
                player.EnableEffect(EffectType.Deafened, flashIntensity * plugin.Config.MaximumFlashDuration, true);
            }
        }

        private bool IsFlashable(Player target, Player thrower, FlashGrenade flashGrenade)
        {
            if (target == thrower)
                return false;

            if (target.SessionVariables.ContainsKey("IsNPC"))
                return false;

            if (!thrower.ReferenceHub.weaponManager.GetShootPermission(target.Team))
            {
                plugin.SendDebug($"Shoot permission by {target.Nickname}'s weapon manager was denied, skipping.");
                return false;
            }

            if (Physics.Linecast(flashGrenade.transform.position, target.CameraTransform.position, flashGrenade._ignoredLayers))
            {
                plugin.SendDebug($"Detected a surface with collision between the grenade and {target.Nickname}, skipping.");
                return false;
            }

            return true;
        }
    }
}