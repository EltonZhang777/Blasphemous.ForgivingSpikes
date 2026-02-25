using Gameplay.GameControllers.Penitent.Damage;
using HarmonyLib;

namespace Blasphemous.ForgivingSpikes.Patches;

[HarmonyPatch(typeof(PenitentDamageArea))]
class PenitentDamageArea_SyncCheckTrapHitboxSize_Patch
{
    [HarmonyPatch("SetTopSmallDamageArea")]
    [HarmonyPostfix]
    public static void SyncTopSmall()
    {
        PatchController.SyncCheckTrapHitboxSize();
    }

    [HarmonyPatch("SetBottomSmallDamageArea")]
    [HarmonyPostfix]
    public static void SyncBottomSmall()
    {
        PatchController.SyncCheckTrapHitboxSize();
    }

    [HarmonyPatch("SetDefaultDamageArea")]
    [HarmonyPostfix]
    public static void SyncDefault()
    {
        PatchController.SyncCheckTrapHitboxSize();
    }
}
