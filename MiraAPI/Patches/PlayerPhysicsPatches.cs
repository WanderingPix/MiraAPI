using System.Linq;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;

namespace MiraAPI.Patches;

/// <summary>
/// General patches for the PlayerPhysics class.
/// </summary>
[HarmonyPatch(typeof(PlayerPhysics))]
public static class PlayerPhysicsPatches
{
      /// <summary>
      /// Applies the role's speed modifier 
      /// </summary>
      /// <param name="__instance">PlayerPhysics instance.</param>
      [HarmonyPostfix]
      [HarmonyPatch(nameof(PlayerPhysics.FixedUpdate))]
      public static void ApplySpeedMod(PlayerPhysics __instance)
      {
            if (__instance.AmOwner && PlayerControl.LocalPlayer.Data.Role is ICustomRole customRole)
            {
                  __instance.velocity.x *= customRole.Speed.x;
                  __instance.velocity.y *= customRole.Speed.y;
            }
      }
}
