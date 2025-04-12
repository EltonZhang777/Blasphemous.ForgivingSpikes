using Blasphemous.ForgivingSpikes.Components;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Damage;
using Gameplay.UI;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes.Patches;

/// <summary>
/// Contains useful static methods that assist patching
/// </summary>
internal static class PatchController
{
    internal static GameObject TpoCheckTrapParent => Core.Logic.Penitent.transform.Find("#Constitution/Feet").gameObject;
    internal static bool diedToSpikeDamage = false;
    internal static Queue<Vector3> safePositionQueue = new();
    internal static float storeSafePositionInterval;
    internal static bool isStoringSafePosition = false;

    /// <summary>
    /// Not yet implemented
    /// </summary>
    internal static void SetModTrapCheckerActive(bool active)
    {
        if (active)
        {
            // enable modded, disable vanilla
            CheckTrap vanillaChecker = TpoCheckTrapParent.GetComponent<CheckTrap>();
            if (vanillaChecker != null)
                UnityEngine.Object.Destroy(vanillaChecker);

            TpoCheckTrapParent.AddComponent<ModTrapChecker>();
        }
        else
        {
            // enable vanilla, disable modded
            ModTrapChecker moddedChecker = TpoCheckTrapParent.GetComponent<ModTrapChecker>();
            if (moddedChecker != null)
                UnityEngine.Object.Destroy(moddedChecker);

            TpoCheckTrapParent.AddComponent<CheckTrap>();
        }
    }

    internal static void GetTpoDamageAreaColliderSizeAndOffset(out Vector2 size, out Vector2 offset)
    {
        PenitentDamageArea penitentDamageArea = Core.Logic.Penitent.gameObject.GetComponentInChildren<PenitentDamageArea>();
        BoxCollider2D collider = Traverse.Create(penitentDamageArea).Field("_damageAreaCollider").GetValue<BoxCollider2D>();
        size = collider.size;
        offset = collider.offset;
    }

    internal static bool Prefix_DoModSpikeDamage()
    {
        // if instakill, execute vanilla code
        if (SpikeUtilities.CurrentConfig.spikePenaltyType == SpikePenaltyConfig.SpikePenaltyType.Instakill)
            return true;

        // deal mod damage
        SpikeUtilities.DealSpikeDamageToPenitent();
        if (Core.Logic.Penitent.Stats.Life.Current <= 0)
        {
            PatchController.diedToSpikeDamage = true;
            return true;
        }

        // start respawn coroutine if didn't die to spike damage
        UIController.instance.StartCoroutine(SpikeUtilities.TpoRespawnCoroutine());
        return false;
    }

    internal static void Postfix_SyncCheckTrapHitboxSize()
    {
        PatchController.GetTpoDamageAreaColliderSizeAndOffset(out Vector2 size, out Vector2 offset);
        BoxCollider2D collider = PatchController.TpoCheckTrapParent.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(size.x, size.y * 1.0f);
        collider.offset = offset;
    }

    /// <summary>
    /// Back-up and queue safe position regularly with a given time interval.
    /// </summary>
    internal static IEnumerator StoreLastSafePosition(float interval)
    {
        safePositionQueue = new(Enumerable.Repeat(Core.LevelManager.LastSafePosition, 5));
        while (true)
        {
            Vector3 currentSafePosition = Core.LevelManager.LastSafePosition;
            if (currentSafePosition != safePositionQueue.LastOrDefault())
            {
                safePositionQueue.Dequeue();
                safePositionQueue.Enqueue(currentSafePosition);
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
