using Blasphemous.ForgivingSpikes.Components;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes.Patches;

/// <summary>
/// Contains useful static methods that assist patching
/// </summary>
internal class PatchController
{
    internal static GameObject TpoCheckTrapParent => Core.Logic.Penitent.transform.Find("#Constitution/Feet").gameObject;

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
}
