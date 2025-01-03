// -----------------------------------------------------------------------
// <copyright file="BloodType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of blood decals.
    /// </summary>
    /// <seealso cref="Features.Map.PlaceBlood(UnityEngine.Vector3, UnityEngine.Vector3)"/>
    /// <seealso cref="Features.Player.PlaceBlood(UnityEngine.Vector3)"/>
    public enum BloodType
    {
        /// <summary>
        /// The default blood decal.
        /// </summary>
        Default,

        /// <summary>
        /// The blood decal placed after Scp106 sends someone to the pocket dimension.
        /// </summary>
        Scp106,

        /// <summary>
        /// The spreaded blood decal.
        /// </summary>
        Spreaded,

        /// <summary>
        /// The faded blood decal.
        /// </summary>
        Faded,
    }
}