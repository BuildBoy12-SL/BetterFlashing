// -----------------------------------------------------------------------
// <copyright file="ConfigValidator.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BetterFlashing
{
    using System.Collections.Generic;
    using NorthwoodLib.Pools;
    using UnityEngine;

    /// <summary>
    /// Parses serialized config values further into more useful forms.
    /// </summary>
    public static class ConfigValidator
    {
        /// <summary>
        /// Translates a <see cref="Dictionary{TKey,TValue}"/> into an <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/> to be translated.</param>
        /// <returns>A <see cref="AnimationCurve"/> with values stripped from the provided <see cref="Dictionary{TKey,TValue}"/>.</returns>
        public static AnimationCurve Curve(Dictionary<float, float> dictionary)
        {
            List<Keyframe> keyframes = ListPool<Keyframe>.Shared.Rent();

            foreach (var kvp in dictionary)
                keyframes.Add(new Keyframe(kvp.Key, kvp.Value));

            Keyframe[] keyframeArray = keyframes.ToArray();
            ListPool<Keyframe>.Shared.Return(keyframes);
            return new AnimationCurve(keyframeArray);
        }
    }
}