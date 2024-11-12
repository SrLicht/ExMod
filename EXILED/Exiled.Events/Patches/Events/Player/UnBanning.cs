// -----------------------------------------------------------------------
// <copyright file="UnBanning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="BanHandler.RemoveBan(string, BanHandler.BanType, bool)" />.
    /// Adds the <see cref="Handlers.Player.UnBanning" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UnBanning))]
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.RemoveBan))]
    internal static class UnBanning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Gets if (!EventManager.ExecuteEvent(new BanRevokedEvent(id, banType)) && !forced) return;
            MethodInfo executeEventMethod = typeof(EventManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == nameof(EventManager.ExecuteEvent)
                                     && !m.IsGenericMethod);
            int offset = -8;

            // Find the index of the last `EventManager.ExecuteEvent` call.
            int index = newInstructions.FindLastIndex(i => i.Calls(executeEventMethod)) + offset;

            // Define label for branching logic.
            Label returnLabel = generator.DefineLabel();

            // Declare the `UnBanningEventArgs` local variable.
            LocalBuilder ev = generator.DeclareLocal(typeof(UnBanningEventArgs));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Load `id` (argument 0) onto the stack.
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // Load `banType` (argument 1) onto the stack.
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // UnBanningEventArgs ev = new(id, banType)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnBanningEventArgs))[0]),

                    // Duplicate the object on the stack to pass
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Store the duplicated object in the `ev` local variable.
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Scp173.OnBeingObserved(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUnBanning))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UnBanningEventArgs), nameof(UnBanningEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel), // If `IsAllowed` is false, jump to the `returnLabel`.
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
