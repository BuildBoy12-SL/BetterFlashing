// -----------------------------------------------------------------------
// <copyright file="FlashExplosionPatch.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BetterFlashing.Patches
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using Grenades;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="FlashGrenade.ServersideExplosion"/> to override with <see cref="Methods.RunFlash"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashGrenade), nameof(FlashGrenade.ServersideExplosion))]
    internal static class FlashExplosionPatch
    {
        private static bool Prefix(FlashGrenade __instance)
        {
            if (!(Player.Get(__instance.thrower.hub) is Player thrower))
            {
                Log.Debug("Could not find the thrower of the flash bang, using native method.");
                return true;
            }

            Plugin.Instance.Methods.RunFlash(__instance, thrower);
            return false;
        }
    }
}