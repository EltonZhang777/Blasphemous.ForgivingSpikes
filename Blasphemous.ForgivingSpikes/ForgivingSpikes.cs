using Blasphemous.ModdingAPI;

namespace Blasphemous.ForgivingSpikes;

internal class ForgivingSpikes : BlasMod
{
    internal Config config;

    internal ForgivingSpikes() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        //LocalizationHandler.RegisterDefaultLanguage("en");
        config = ConfigHandler.Load<Config>();
    }

    protected override void OnAllInitialized()
    {
#if DEBUG
        SpikeUtilities.SetSpikePenalty(SpikeUtilities.SpikePenaltyType.PercentageDamage, 0.4f);
#endif
    }

    protected override void OnDispose()
    {
        ConfigHandler.Save(config);
    }
}
