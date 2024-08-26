// -----------------------------------------------------------------------
// <copyright file="SupplyLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;
    using Mirror;
    using PluginAPI.Core.Items;
    using UnityEngine;

#nullable enable
    /// <summary>
    /// The in-game SupplyLocker.
    /// </summary>
    public class SupplyLocker : IWrapper<Locker>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="Locker"/>s and their corresponding <see cref="SupplyLocker"/>.
        /// </summary>
        internal static readonly Dictionary<Locker, SupplyLocker> LockerToSupplyLocker = new(250, new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplyLocker"/> class.
        /// </summary>
        /// <param name="locker">The encapsulated <see cref="Locker"/>.</param>
        internal SupplyLocker(Locker locker)
        {
            Base = locker;
            LockerToSupplyLocker.Add(locker, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="SupplyLocker"/> which contains all the <see cref="SupplyLocker"/> instances.
        /// </summary>
        public static IReadOnlyCollection<SupplyLocker> List => LockerToSupplyLocker.Values;

        /// <summary>
        /// Gets the <see cref="Locker"/> instance of the supply locker.
        /// </summary>
        public Locker Base { get; }

        /// <summary>
        /// Gets the <see cref="SupplyLocker"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the <see cref="SupplyLocker"/> position.
        /// </summary>
        public Vector3 Position => Base.transform.position;

        /// <summary>
        /// Gets the <see cref="SupplyLocker"/> rotation.
        /// </summary>
        public Quaternion Rotation => Base.transform.rotation;

        /// <summary>
        /// Gets the <see cref="Features.Room"/> in which the <see cref="SupplyLocker"/> is located.
        /// </summary>
        public Room? Room => Room.Get(Position);

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the locker is located.
        /// </summary>
        public ZoneType Zone => Room?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets the <see cref="LockerChamber"/>'s in this <see cref="SupplyLocker"/>.
        /// </summary>
        public IEnumerable<LockerChamber> Chambers => Base.Chambers;

        /// <summary>
        /// Gets the <see cref="LockerChamber"/> who has been open.
        /// </summary>
        public IEnumerable<LockerChamber> OpenedChambers => Chambers.Where(c => c.IsOpen).ToArray();

        /// <summary>
        /// Gets a random position from one of the <see cref="Chambers"/>.
        /// </summary>
        public Vector3 RandomChamberPosition
        {
            get
            {
                LockerChamber randomChamber = Chambers.GetRandomValue();

                // Determine if the chamber uses multiple spawn points and has at least one available spawn point.
                if (randomChamber._useMultipleSpawnpoints && randomChamber._spawnpoints.Length > 0)
                {
                    // Return the position of a random spawn point within the chamber.
                    return randomChamber._spawnpoints.RandomItem().position;
                }

                // Return the position of the main spawn point for the chamber.
                return randomChamber._spawnpoint.position;
            }
        }

        /// <summary>
        /// Gets the <see cref="SupplyLocker"/> belonging to the <see cref="Locker"/>, if any.
        /// </summary>
        /// <param name="locker">The <see cref="Locker"/> to get.</param>
        /// <returns>A <see cref="SupplyLocker"/> or <see langword="null"/> if not found.</returns>
        public static SupplyLocker? Get(Locker locker) => locker == null ? null :
            LockerToSupplyLocker.TryGetValue(locker, out SupplyLocker supply) ? supply : new SupplyLocker(locker);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="SupplyLocker"/> given the specified <see cref="ZoneType"/>.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to search for.</param>
        /// <returns>The <see cref="SupplyLocker"/> with the given <see cref="ZoneType"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<SupplyLocker> Get(ZoneType zoneType) => Get(room => room.Zone.HasFlag(zoneType));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="SupplyLocker"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="SupplyLocker"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<SupplyLocker> Get(Func<SupplyLocker, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets a random <see cref="SupplyLocker"/>.
        /// </summary>
        /// <param name="zoneType">Filters by <see cref="ZoneType"/>.</param>
        /// <returns><see cref="SupplyLocker"/> object.</returns>
        public static SupplyLocker Random(ZoneType zoneType = ZoneType.Unspecified) => (zoneType is not ZoneType.Unspecified ? Get(r => r.Zone.HasFlag(zoneType)) : List).GetRandomValue();

        /// <summary>
        /// Adds an item to a randomly selected locker chamber.
        /// </summary>
        /// <param name="item">The <see cref="ItemPickup"/> to be added to the locker chamber.</param>
        public void AddItem(ItemPickup item)
        {
            // Select a random chamber from the available locker chambers.
            LockerChamber chamber = Chambers.GetRandomValue();

            // Determine the parent transform where the item will be placed.
            Transform parentTransform = chamber._useMultipleSpawnpoints && chamber._spawnpoints.Length > 0
                ? chamber._spawnpoints.RandomItem()
                : chamber._spawnpoint;

            // If the chamber is open, immediately set the item's parent and spawn it.
            if (chamber.IsOpen)
            {
                item.Transform.SetParent(parentTransform);
                item.Spawn();
            }
            else
            {
                // If the item is already spawned on the network, unspawn it before proceeding.
                if (NetworkServer.spawned.ContainsKey(item.OriginalObject.netId))
                    NetworkServer.UnSpawn(item.GameObject);

                // Set the item's parent transform.
                item.Transform.SetParent(parentTransform);

                // Lock the item in place.
                item.IsLocked = true;

                // Notify any pickup distributor triggers.
                (item.OriginalObject as IPickupDistributorTrigger)?.OnDistributed();

                // If the item has a Rigidbody component, make it kinematic and reset its position and rotation.
                if (item.Rigidbody != null)
                {
                    item.Rigidbody.isKinematic = true;
                    item.Rigidbody.transform.localPosition = Vector3.zero;
                    item.Rigidbody.transform.localRotation = Quaternion.identity;

                    // Add the Rigidbody to the list of bodies to be unfrozen later.
                    SpawnablesDistributorBase.BodiesToUnfreeze.Add(item.Rigidbody);
                }

                // If the chamber is configured to spawn items on the first opening, add the item to the list of items to be spawned.
                // Otherwise, spawn the item immediately.
                if (chamber._spawnOnFirstChamberOpening)
                    chamber._toBeSpawned.Add(item.OriginalObject);
                else
                    ItemDistributor.SpawnPickup(item.OriginalObject);
            }
        }

        /// <summary>
        /// Spawns an item of the specified <see cref="ItemType"/> to the locker by creating a new <see cref="ItemPickup"/>.
        /// </summary>
        /// <param name="type">The type of item to be added.</param>
        public void AddItem(ItemType type) => AddItem(ItemPickup.Create(type, default, default));
    }
}
