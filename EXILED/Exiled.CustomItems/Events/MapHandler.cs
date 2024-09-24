// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="Exiled Team">
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
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.WaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            foreach (CustomItem customItem in CustomItem.Registered)
                customItem?.SpawnAll();
        }
    }
}