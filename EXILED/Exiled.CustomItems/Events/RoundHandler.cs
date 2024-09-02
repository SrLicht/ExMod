// -----------------------------------------------------------------------
// <copyright file="RoundHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.CustomItems.API.Features;
    using MEC;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class RoundHandler
    {
<<<<<<< HEAD:EXILED/Exiled.CustomItems/Events/RoundHandler.cs
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
=======
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.Generated"/>
        public void OnMapGenerated()
>>>>>>> 96ccf323 (``[Exiled::CustomItems]`` ``[Exiled::API]`` ``[Exiled::CustomRoles]`` Adding news Spawnpoints, Wrapper for Locker and added LockerType (#77)):EXILED/Exiled.CustomItems/Events/MapHandler.cs
        {
            Timing.CallDelayed(0.5f, () => // Delay its necessary for the spawnpoints of lockers and rooms to be generated.
            {
                foreach (CustomItem customItem in CustomItem.Registered)
                    customItem?.SpawnAll();
            });
        }
    }
}