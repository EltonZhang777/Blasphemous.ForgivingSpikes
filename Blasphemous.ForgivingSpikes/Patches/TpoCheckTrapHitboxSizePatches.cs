using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blasphemous.ModdingAPI;
using Gameplay.GameControllers.Penitent.Damage;
using HarmonyLib;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes.Patches;

[HarmonyPatch(typeof(PenitentDamageArea), "SetTopSmallDamageArea")]
class PenitentDamageArea_SetTopSmallDamageArea_SyncCheckTrapHitboxSize_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PatchController.Postfix_SyncCheckTrapHitboxSize();
    }
}

[HarmonyPatch(typeof(PenitentDamageArea), "SetBottomSmallDamageArea")]
class PenitentDamageArea_SetBottomSmallDamageArea_SyncCheckTrapHitboxSize_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PatchController.Postfix_SyncCheckTrapHitboxSize();
    }
}

[HarmonyPatch(typeof(PenitentDamageArea), "SetDefaultDamageArea")]
class PenitentDamageArea_SetDefaultDamageArea_SyncCheckTrapHitboxSize_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PatchController.Postfix_SyncCheckTrapHitboxSize();
    }
}