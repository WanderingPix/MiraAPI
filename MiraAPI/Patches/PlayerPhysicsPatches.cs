using System.Linq;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace MiraAPI.Patches;

/// <summary>
/// General patches for the PlayerPhysics class.
/// </summary>
[HarmonyPatch(typeof(PlayerPhysics))]
public static class PlayerPhysicsPatches
{
      /// <summary>
      /// Applies the and modifier speed to the player's velocity. 
      /// </summary>
      /// <param name="__instance">PlayerPhysics instance.</param>
      [HarmonyPostfix]
      [HarmonyPatch(nameof(PlayerPhysics.FixedUpdate))]
      public static void ApplySpeedMod(PlayerPhysics __instance)
      {
            if (__instance.AmOwner)
            {
                  var ModifierSpeeds = new Vector2(0f,0f);

                  var Vel = __instance.Velocity;


                  foreach (var Modifier in PlayerControl.LocalPlayer.GetModifierComponent().ActiveModifiers.Where(x => x.Speed != new Vector2(1f,1f)))
                  {
                        ModifierSpeeds.x = ModifierSpeeds.x + Modifier.Speed.x;
                  }
                  
                  Vel.x *= ModifierSpeeds.x;
                  Vel.y *= ModifierSpeeds.y;
            }
      }
}

