using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Spawn;
using HarmonyLib;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes.Patches;

/// <summary>
/// Deal mod spike damage when getting into spikes and when falling into abyss without linen
/// </summary>
[HarmonyPatch(typeof(CheckTrap))]
class CheckTrap_DoModSpikeDamage_Patch
{
    [HarmonyPatch("SpikeTrapDamage")]
    [HarmonyPrefix]
    public static bool PatchSpikeDamage()
    {
        return PatchController.InflictModSpikeDamage();
    }

    [HarmonyPatch("AbyssTrapDamage")]
    [HarmonyPrefix]
    public static bool PatchAbyssDamage()
    {
        return PatchController.InflictModSpikeDamage();
    }
}

/// <summary>
/// Prevent launching penitent death event when TPO wouldn't be dead to spike/abyss damage
/// </summary>
[HarmonyPatch(typeof(EventManager))]
class EventManager_PreventPenitentDeath_Patch
{
    [HarmonyPatch("LaunchEvent")]
    [HarmonyPrefix]
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
[HarmonyPatch(typeof(SpawnManager))]
class SpawnManager_ShowDebugInfo_Patch
{
    [HarmonyPatch("CreatePlayer")]
    [HarmonyPrefix]
    public static void Prefix(
        Vector3 position,
        EntityOrientation orientation,
        bool createNewInstance)
    {
        Main.LogIfDebug($"Invoked Core.SpawnManager.CreatePlayer({position}, {orientation}, {createNewInstance}) !");
    }

    [HarmonyPatch("CreatePlayer")]
    [HarmonyPostfix]
    public static void Postfix()
    {
        Main.LogIfDebug($"Core.SpawnManager.CreatePlayer() executed successfully!");
    }
}

[HarmonyPatch(typeof(Penitent))]
class Penitent_ShowDebugInfo_Patch
{
    [HarmonyPatch("CherubRespawn")]
    [HarmonyPrefix]
    public static void Prefix(
        GameObject ___Cherubs)
    {
        Main.LogIfDebug($"Cherubs is null?: {___Cherubs == null}");
    }
}

[HarmonyPatch(typeof(CherubRespawn))]
class CherubRespawn_ShowDebugInfo_Patch
{
    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    public static void StartPrefix()
    {
        Main.LogIfDebug($"Started CherubRespawn!");
    }

    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    public static void AwakePrefix()
    {
        Main.LogIfDebug($"CherubRespawn awoken!");
    }
}

#endif