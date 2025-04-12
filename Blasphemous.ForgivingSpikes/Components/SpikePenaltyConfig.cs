using Gameplay.GameControllers.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes.Components;

/// <summary>
/// Contains a set of specifications of how spikes penalize TPO
/// </summary>
public struct SpikePenaltyConfig : IEquatable<SpikePenaltyConfig>
{
    /// <summary>
    /// Current spike penalty type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public SpikePenaltyType spikePenaltyType;

    /// <summary>
    /// Damage amount of the spike. 
    /// If <see cref="spikePenaltyType"/> is <see cref="SpikePenaltyType.PercentageDamage"/>, it is a ratio to TPO's max health in range [0, 1]
    /// </summary>
    public float spikeDamageAmount;

    /// <summary>
    /// Whether spike's damage ignore defense and damage reduction
    /// </summary>
    public bool spikeDamageIgnoreDefense = true;

    /// <summary>
    /// Damage element of spike's damage
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DamageArea.DamageElement spikeDamageElement = DamageArea.DamageElement.Normal;

    /// <summary>
    /// Damage element of spike's damage
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DamageArea.DamageType spikeDamageType = DamageArea.DamageType.Heavy;

    /// <summary>
    /// Quick default config for spikes to instakill TPO as vanilla does.
    /// </summary>
    public static SpikePenaltyConfig Instakill => new SpikePenaltyConfig(
        SpikePenaltyType.Instakill,
        (float)1e7);

    /// <summary>
    /// Quick default config for spikes to not deal damage to TPO.
    /// </summary>
    public static SpikePenaltyConfig NoDamage => new SpikePenaltyConfig(
        SpikePenaltyType.FixedDamage,
        0f,
        false,
        DamageArea.DamageElement.Normal,
        DamageArea.DamageType.Normal);

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

    /// <summary>
    /// Constructor of <see cref="SpikePenaltyConfig"/>
    /// </summary>
    public SpikePenaltyConfig(
        SpikePenaltyType spikePenaltyType,
        float spikeDamageAmount,
        bool spikeDamageIgnoreDefense = true,
        DamageArea.DamageElement spikeDamageElement = DamageArea.DamageElement.Normal,
        DamageArea.DamageType spikeDamageType = DamageArea.DamageType.Heavy)
    {
        this.spikePenaltyType = spikePenaltyType;
        this.spikeDamageAmount = spikeDamageAmount;
        this.spikeDamageIgnoreDefense = spikeDamageIgnoreDefense;
        this.spikeDamageElement = spikeDamageElement;
        this.spikeDamageType = spikeDamageType;

        if (spikePenaltyType == SpikePenaltyType.PercentageDamage)
        {
            // ratio to TPO's max HP must be in range [0, 1]
            this.spikeDamageAmount = Mathf.Clamp01(spikeDamageAmount);
        }
    }

    public static bool operator ==(SpikePenaltyConfig a, SpikePenaltyConfig b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(SpikePenaltyConfig a, SpikePenaltyConfig b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is SpikePenaltyConfig config && Equals(config);
    }

    public bool Equals(SpikePenaltyConfig other)
    {
        return spikePenaltyType == other.spikePenaltyType &&
               spikeDamageAmount == other.spikeDamageAmount &&
               spikeDamageIgnoreDefense == other.spikeDamageIgnoreDefense &&
               spikeDamageElement == other.spikeDamageElement &&
               spikeDamageType == other.spikeDamageType;
    }

    public override int GetHashCode()
    {
        int hashCode = 486688808;
        hashCode = hashCode * -1521134295 + spikePenaltyType.GetHashCode();
        hashCode = hashCode * -1521134295 + spikeDamageAmount.GetHashCode();
        hashCode = hashCode * -1521134295 + spikeDamageIgnoreDefense.GetHashCode();
        hashCode = hashCode * -1521134295 + spikeDamageElement.GetHashCode();
        hashCode = hashCode * -1521134295 + spikeDamageType.GetHashCode();
        return hashCode;
    }
}
