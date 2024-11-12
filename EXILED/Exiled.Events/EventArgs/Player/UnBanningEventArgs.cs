// -----------------------------------------------------------------------
// <copyright file="UnBanningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before un-banning a userid.
    /// </summary>
    public class UnBanningEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnBanningEventArgs" /> class.
        /// </summary>
        /// <param name="userId"> player userid.
        /// </param>
        /// <param name="type">
        ///  is ip ban or userid ban only.
        /// </param>
        public UnBanningEventArgs(string userId, BanHandler.BanType type)
        {
            UserId = userId;
            BanType = type;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ban can be revoked.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="BanHandler.BanType"/>.
        /// </summary>
        public BanHandler.BanType BanType { get; }

        /// <summary>
        /// Gets a value indicating the SteamId64 being unbanned.
        /// </summary>
        public string UserId { get; }
    }
}
