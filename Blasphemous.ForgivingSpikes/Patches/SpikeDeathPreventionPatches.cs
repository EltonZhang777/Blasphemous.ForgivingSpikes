using Framework.Managers;
using Gameplay.GameControllers.Entities;
using HarmonyLib;

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