using Framework.Managers;
using Gameplay.GameControllers.Entities;
using HarmonyLib;

namespace Blasphemous.ForgivingSpikes.Patches;

// If in Iframes or first hit with enough health, skip the rest of spike damage
[HarmonyPatch(typeof(CheckTrap), nameof(CheckTrap.SpikeTrapDamage))]
class SpikeDamage_Patch
{
    public static bool Prefix()
    {
        return Main.SpikeProtection.InSpikes();
    }
}

// Only set this if actually dead
[HarmonyPatch(typeof(CheckTrap), nameof(CheckTrap.DeathBySpike), MethodType.Setter)]
class SpikeDamageFlag_Patch
{
    public static bool Prefix() 
    {
        return Main.SpikeProtection.DeadForReal;
    }
}

/// <summary>
/// Prevent launching penitent death event when TPO wouldn't be dead
/// </summary>
[HarmonyPatch(typeof(EventManager), nameof(EventManager.LaunchEvent))]
class EventManager_Patch
{
    public static bool Prefix(
        string id,
        string parameter)
    {
        if (!(id == "PENITENT_KILLED" && parameter == "SPIKES"))
            return true;

    }
}
