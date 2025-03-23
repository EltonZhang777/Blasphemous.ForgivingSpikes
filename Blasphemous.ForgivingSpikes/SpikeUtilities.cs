using Blasphemous.ForgivingSpikes.Components;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes;

/// <summary>
/// Contains configurable settings and useful accessors of spikes that other mods can access.
/// </summary>
public static class SpikeUtilities
{
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

    internal static float spikeInstakillDamage = (float)1e7;

    public static Hit SpikeHit
    {
        get
        {
            Hit result = new()
            {
                AttackingEntity = Core.Logic.Penitent.gameObject,
                DamageAmount = CalculateDamageAmount(),
                DamageElement = CurrentConfig.spikeDamageElement,
                DamageType = CurrentConfig.spikeDamageType,
                Unblockable = true,
                Unparriable = true,
                Unnavoidable = true
            };
            return result;
        }
    }
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
        CurrentConfig = GlobalConfig;
        IsUsingGlobalConfig = true;
    }

    /// <summary>
    /// Deal an instance of spike damage to TPO
    /// </summary>
    public static void DealSpikeDamageToPenitent()
    {
        if (CurrentConfig.spikeDamageIgnoreDefense)
        {
            // directly set health of TPO to ignore defense
            Core.Logic.Penitent.Stats.Life.Current = Mathf.Clamp(Core.Logic.Penitent.Stats.Life.Current - CalculateDamageAmount(), 0, float.MaxValue);
        }
        else
        {
            // deal a normal hit
            Core.Logic.Penitent.Damage(SpikeHit);
        }
    }

    private static float CalculateDamageAmount()
    {
        float result;
        result = CurrentConfig.spikePenaltyType switch
        {
            SpikePenaltyConfig.SpikePenaltyType.Instakill => spikeInstakillDamage,
            SpikePenaltyConfig.SpikePenaltyType.FixedDamage => CurrentConfig.spikeDamageAmount,
            SpikePenaltyConfig.SpikePenaltyType.PercentageDamage => CurrentConfig.spikeDamageAmount * TpoMaxHealth,
            _ => spikeInstakillDamage
        };
        return result;
    }
}

