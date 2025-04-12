using Blasphemous.ForgivingSpikes.Components;
using Blasphemous.ForgivingSpikes.Patches;
using Blasphemous.ModdingAPI;
using DG.Tweening;
using Epic.OnlineServices;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes;

/// <summary>
/// Contains configurable settings and useful accessors of spikes that other mods can access.
/// </summary>
public static class SpikeUtilities
{
    /// <summary>
    /// The time between getting hit by spikes and TPO starts to fade out in spikes.
    /// </summary>
    public static float delayBeforeFadeOutInSpikes = 0f;

    /// <summary>
    /// The duration of TPO fading out in spikes.
    /// </summary>
    public static float fadeOutDuration = 0.3f;

    /// <summary>
    /// The time between TPO becomes completely invisible and TPO respawning.
    /// </summary>
    public static float delayBeforeRespawn = 0.5f;

    internal static float spikeInstakillDamage = (float)1e7;
    internal static string inputBlockerName = "MOD_SPIKE_RESPAWNING";

    /// <summary>
    /// Currently active config for spike penalty
    /// </summary>
    public static SpikePenaltyConfig CurrentConfig { get; internal set; }

    /// <summary>
    /// Stored global config for spike penalty. Current config defaults to this value if no other temporary config exist.
    /// </summary>
    public static SpikePenaltyConfig GlobalConfig { get; internal set; }

    /// <summary>
    /// Whether global config is currently used. 
    /// </summary>
    public static bool IsUsingGlobalConfig { get; internal set; }

    internal static float TpoMaxHealth => Core.Logic.Penitent.Stats.Life.CurrentMax;

    /// <summary>
    /// Set current spike penalty config. If you want to change and apply global config, use <see cref="SetGlobalConfig(SpikePenaltyConfig)"/> and then <see cref="UseGlobalConfig"/> instead.
    /// </summary>
    public static void SetCurrentConfig(SpikePenaltyConfig config)
    {
        CurrentConfig = config;
        if (CurrentConfig != GlobalConfig)
        {
            IsUsingGlobalConfig = false;
        }
    }

    /// <summary>
    /// Set the global spike penalty config. Automatically updates to current config if system is using global config currently.
    /// </summary>
    public static void SetGlobalConfig(SpikePenaltyConfig config)
    {
        GlobalConfig = config;
        if (IsUsingGlobalConfig)
        {
            UseGlobalConfig();
        }
    }

    /// <summary>
    /// Set current spike penalty config to global spike penalty config (i.e. abandon all temporary current config)
    /// </summary>
    public static void UseGlobalConfig()
    {
        ModLog.Warn($"Using global config for spikes!");
        CurrentConfig = GlobalConfig;
        IsUsingGlobalConfig = true;
    }

    /// <summary>
    /// Deal an instance of spike damage to TPO with current config
    /// </summary>
    public static void DealSpikeDamageToPenitent()
    {
        DealSpikeDamageToPenitent(CurrentConfig);
    }

    /// <summary>
    /// Deal an instance of spike damage to TPO with the specified configuration
    /// </summary>
    public static void DealSpikeDamageToPenitent(SpikePenaltyConfig config)
    {
        if (config.spikeDamageIgnoreDefense)
        {
            // directly set health of TPO to ignore defense
            Core.Logic.Penitent.Stats.Life.Current = Mathf.Clamp(Core.Logic.Penitent.Stats.Life.Current - CalculateDamageAmount(config), 0, float.MaxValue);
        }
        else
        {
            // deal a normal hit
            Core.Logic.Penitent.Damage(GetSpikeHit(config));
        }
    }

    private static float CalculateDamageAmount(SpikePenaltyConfig config)
    {
        float result;
        result = config.spikePenaltyType switch
        {
            SpikePenaltyConfig.SpikePenaltyType.Instakill => spikeInstakillDamage,
            SpikePenaltyConfig.SpikePenaltyType.FixedDamage => config.spikeDamageAmount,
            SpikePenaltyConfig.SpikePenaltyType.PercentageDamage => config.spikeDamageAmount * TpoMaxHealth,
            _ => spikeInstakillDamage
        };
        return result;
    }

    private static float CalculateDamageAmount()
    {
        return CalculateDamageAmount(CurrentConfig);
    }

    public static Hit GetSpikeHit(SpikePenaltyConfig config)
    {
        Hit result = new()
        {
            AttackingEntity = Core.Logic.Penitent.gameObject,
            DamageAmount = CalculateDamageAmount(config),
            DamageElement = config.spikeDamageElement,
            DamageType = config.spikeDamageType,
            Unblockable = true,
            Unparriable = true,
            Unnavoidable = true
        };
        return result;
    }

    public static Hit GetSpikeHit()
    {
        return GetSpikeHit(CurrentConfig);
    }

    public static IEnumerator TpoRespawnCoroutine()
    {
        Penitent tpo = Core.Logic.Penitent;
        // store safe position to prevent it being updated on spikes
        Vector3 storedSafePosition = Core.LevelManager.LastSafePosition;
        Vector3 tpoCurrentPosition = tpo.GetPosition();
        tpo.Teleport(tpoCurrentPosition);  // stop Penitent from moving
#if DEBUG
        ModLog.Warn($"Current position: {tpo.GetPosition()}; \nSafe position: {Core.LevelManager.LastSafePosition}");
#endif
        
        // if safe position is in spikes, revert safe position to a moment ago until it is away from spikes
        List<Vector3> safePositions = PatchController.safePositionQueue.ToList();
        for (int i = PatchController.safePositionQueue.Count - 1; i >= 0; i--) 
        {
#if DEBUG
            ModLog.Warn($"Safe Position too close in spikes! Reverting safe position to a moment ago");
#endif
            if ((tpoCurrentPosition - safePositions[i]).magnitude > 0.2f)
            {
                storedSafePosition = safePositions[i];
                break;
            }
        }
#if DEBUG
        ModLog.Warn($"Safe position after safety fallback: {storedSafePosition}");
#endif

        // disable player input
        Core.Input.SetBlocker(inputBlockerName, true);

        // play spike death VFX & SFX
        Core.Audio.PlaySfxOnCatalog("PenitentDeathBySpike");
        yield return new WaitForSeconds(delayBeforeFadeOutInSpikes);

        // starts fading out 
        SpriteRenderer TpoSprite = tpo.transform.Find("#Constitution/Body").GetComponent<SpriteRenderer>();
        Tweener tweener = TpoSprite.DOFade(0, fadeOutDuration);
        yield return tweener.WaitForCompletion();

        // delay and respawn
        yield return new WaitForSeconds(delayBeforeRespawn);
        Traverse.Create(Core.LevelManager).Property("LastSafePosition").SetValue(storedSafePosition);  // restore safe position
        tpo.Teleport(storedSafePosition);
        tweener = TpoSprite.DOFade(1, Time.deltaTime);  // set Penitent visible
        yield return tweener.WaitForCompletion();

        // spawning Cherub Respawn VFX, not implemented yet
        /*
        object[] parameters = { storedSafePosition, EntityOrientation.Left, false };
        Traverse.Create(Core.SpawnManager).Method("CreatePlayer", parameters).GetValue(parameters);
        Tpo.CherubRespawn();
        */

        // reset death flags
        CheckTrap checkTrap = tpo.GetComponentInChildren<CheckTrap>();
        Traverse.Create(checkTrap).Property("DeathBySpike").SetValue(false);
        Traverse.Create(checkTrap).Property("DeathByFall").SetValue(false);

        // regain player control after respawn
        Core.Input.SetBlocker(inputBlockerName, false);
        yield return new WaitForSeconds(0.5f);
        Core.Input.SetBlocker("PLAYER_LOGIC", false);
        yield break;
    }
}

