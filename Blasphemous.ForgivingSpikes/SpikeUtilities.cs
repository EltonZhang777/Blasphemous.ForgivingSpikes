using Framework.Managers;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes;

/// <summary>
/// Contains configurable settings and useful accessors of spikes that other mods can access.
/// </summary>
public static class SpikeUtilities
{
    /// <summary>
    /// Current spike penalty type
    /// </summary>
    public static SpikePenaltyType CurrentPenaltyType { get; internal set; }

    /// <summary>
    /// Damage amount of the spike. 
    /// If <see cref="CurrentPenaltyType"/> is <see cref="SpikePenaltyType.PercentageDamage"/>, it is a ratio to TPO's max health in range [0, 1]
    /// </summary>
    public static float SpikeDamageAmount { get; internal set; }

    internal static float TpoMaxHealth => Core.Logic.Penitent.Stats.Life.CurrentMax;

    /// <summary>
    /// Penalty when TPO touches spikes
    /// </summary>
    public enum SpikePenaltyType
    {
        /// <summary>
        /// Same as vanilla, instakilling TPO
        /// </summary>
        Instakill,

        /// <summary>
        /// Deal a fixed amount of damage
        /// </summary>
        FixedDamage,

        /// <summary>
        /// Deal a percentage of TPO's max health of damage
        /// </summary>
        PercentageDamage
    }

    static SpikeUtilities()
    {
        CurrentPenaltyType = SpikePenaltyType.Instakill;
    }

    /// <summary>
    /// Set spike penalty type and damage
    /// </summary>
    public static void SetSpikePenalty(SpikePenaltyType penaltyType, float damage)
    {
        CurrentPenaltyType = penaltyType;
        if (penaltyType == SpikePenaltyType.PercentageDamage)
        {
            // ratio to TPO's max HP must be in range [0, 1]
            damage = Mathf.Clamp01(damage);
        }
        SpikeDamageAmount = damage;
    }
}
