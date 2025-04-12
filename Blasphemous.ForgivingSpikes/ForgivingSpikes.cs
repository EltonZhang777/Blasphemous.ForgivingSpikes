using Blasphemous.CheatConsole;
using Blasphemous.ForgivingSpikes.Commands;
using Blasphemous.ForgivingSpikes.Patches;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Gameplay.UI;
using UnityEngine;

namespace Blasphemous.ForgivingSpikes;

internal class ForgivingSpikes : BlasMod
{
    internal Config config;

    internal ForgivingSpikes() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        //LocalizationHandler.RegisterDefaultLanguage("en");

        // initialize spike penalty config to values in `.cfg` before other mods specify penalty config
        config = ConfigHandler.Load<Config>();
        SpikeUtilities.SetGlobalConfig(config.globalSpikePenaltyConfig);
        ConfigHandler.Save(config);

        SpikeUtilities.UseGlobalConfig();
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new SpikeCommand());
    }

    protected override void OnAllInitialized()
    { }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        // if TPO changed level, it must be out of spikes and alive.
        PatchController.diedToSpikeDamage = false;

        Coroutine storeSafePositionCoroutine = null;
        if (SceneHelper.GameSceneLoaded)
        {
            if (!PatchController.isStoringSafePosition)
            {
                PatchController.isStoringSafePosition = true;
                storeSafePositionCoroutine = UIController.instance.StartCoroutine(PatchController.StoreLastSafePosition(PatchController.storeSafePositionInterval));
            }
        }
        else if (SceneHelper.MenuSceneLoaded)
        {
            PatchController.isStoringSafePosition = false;
            UIController.instance.StopCoroutine(storeSafePositionCoroutine);
        }
    }

    protected override void OnDispose()
    {
        ConfigHandler.Save(config);
    }
}
