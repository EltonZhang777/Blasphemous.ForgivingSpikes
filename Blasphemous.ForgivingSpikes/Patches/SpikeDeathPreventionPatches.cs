using Framework.Managers;
using Gameplay.GameControllers.Entities;
using HarmonyLib;
using Blasphemous.ForgivingSpikes.Components;
using Gameplay.UI;
using Blasphemous.ModdingAPI;
using UnityEngine;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Spawn;

namespace Blasphemous.ForgivingSpikes.Patches;

/// <summary>
/// Deal mod spike damage when getting into spikes
/// </summary>
[HarmonyPatch(typeof(CheckTrap), "SpikeTrapDamage")]
class CheckTrap_SpikeTrapDamage_DoModSpikeDamage_Patch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return PatchController.Prefix_DoModSpikeDamage();
    }
}

/// <summary>
/// Deal mod spike damage when falling into abyss without linen
/// </summary>
[HarmonyPatch(typeof(CheckTrap), "AbyssTrapDamage")]
class CheckTrap_AbyssTrapDamage_DoModSpikeDamage_Patch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return PatchController.Prefix_DoModSpikeDamage();
    }
}

/// <summary>
/// Prevent launching penitent death event when TPO wouldn't be dead to spike/abyss damage
/// </summary>
[HarmonyPatch(typeof(EventManager), nameof(EventManager.LaunchEvent))]
class EventManager_Patch
{
    public static bool Prefix(
        string id,
        string parameter)
    {
        if (!((id == "PENITENT_KILLED" && parameter == "SPIKES") || (id == "PENITENT_KILLED" && parameter == "ABYSS")))
            return true;

        if (PatchController.diedToSpikeDamage)
            return true;

        return false;
    }
}

#if DEBUG
[HarmonyPatch(typeof(SpawnManager), "CreatePlayer")]
class SpawnManager_CreatePlayer_ShowDebugInformation_Patch
{
    public static void Prefix(
        Vector3 position, 
        EntityOrientation orientation, 
        bool createNewInstance)
    {
        ModLog.Warn($"Invoked Core.SpawnManager.CreatePlayer({position}, {orientation}, {createNewInstance}) !");
    }

    public static void Postfix()
    {
        ModLog.Warn($"Core.SpawnManager.CreatePlayer() executed successfully!");
    }
}

[HarmonyPatch(typeof(Penitent), "CherubRespawn")]
class Penitent_CherubRespawn_ShowDebugInformation_Patch
{
    public static void Prefix(
        GameObject ___Cherubs)
    {
        ModLog.Warn($"Cherubs is null?: {___Cherubs == null}");
    }
}

[HarmonyPatch(typeof(CherubRespawn), "Start")]
class CherubRespawn_Start_ShowDebugInfo_Patch
{
    public static void Prefix()
    {
        ModLog.Warn($"Started CherubRespawn!");
    }
}

[HarmonyPatch(typeof(CherubRespawn), "Awake")]
class CherubRespawn_Awake_ShowDebugInfo_Patch
{
    public static void Prefix()
    {
        ModLog.Warn($"CherubRespawn awoken!");
    }
}
#endif