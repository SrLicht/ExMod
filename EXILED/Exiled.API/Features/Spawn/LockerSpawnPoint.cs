// -----------------------------------------------------------------------
// <copyright file="LockerSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Features.Spawn
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles the spawn point inside a locker.
    /// </summary>
    public class LockerSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the zone where the locker is located.
        /// </summary>
        public ZoneType Zone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use a random locker chamber's position for spawning.
        /// If <see langword="true"/>, <see cref="Offset"/> will be ignored.
        /// </summary>
        public bool UseChamber { get; set; }

        /// <summary>
        /// Gets or sets the offset position within the locker where the spawn point is located, relative to the locker's origin.
        /// </summary>
        public Vector3? Offset { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Zone.ToString();
            set => throw new InvalidOperationException("The name of this type of SpawnPoint cannot be changed.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get
            {
                SupplyLocker foundLocker = SupplyLocker.Random(Zone) ?? throw new NullReferenceException("No locker found in the specified zone.");

                // If UseChamber is true, use a random chamber's position.
                if (UseChamber)
                    return foundLocker.RandomChamberPosition;

                // Otherwise, use the Offset if provided, or the locker's position.
                return Offset.HasValue ? foundLocker.Transform.TransformPoint(Offset.Value) : foundLocker.Position;
            }
            set => throw new InvalidOperationException("The position of this type of SpawnPoint cannot be changed.");
        }
    }
}
