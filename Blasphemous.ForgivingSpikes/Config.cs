using Blasphemous.ForgivingSpikes.Components;

namespace Blasphemous.ForgivingSpikes;

/// <summary>
/// Main config for ForgivingSpikes mod
/// </summary>
public class Config
{
    /// <summary>
    /// The global spike penalty config. 
    /// Modifying this takes the least priority than modifications made by other mods
    /// </summary>
    public SpikePenaltyConfig globalSpikePenaltyConfig = SpikePenaltyConfig.Instakill;

    public const string NOTE_MESSAGE = $"Modifying penalty in config takes the least priority than modifications made by other mods. (i.e. if any other mod sets spike penalty, config is useless)";
}
