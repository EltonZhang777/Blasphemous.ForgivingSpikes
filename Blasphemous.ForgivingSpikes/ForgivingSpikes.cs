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

    private Coroutine _storeSafePositionCoroutine;

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
#if DEBUG
        provider.RegisterCommand(new SpikeCommand());
#endif
    }

    protected override void OnAllInitialized()
    { }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        // if TPO changed level, it must be out of spikes and alive.
        PatchController.diedToSpikeDamage = false;

        if (SceneHelper.GameSceneLoaded)
        {
            // Start the coroutine that continuously stores safe position
            if (!PatchController.isStoringSafePosition)
            {
                PatchController.isStoringSafePosition = true;
                _storeSafePositionCoroutine = UIController.instance.StartCoroutine(
                    PatchController.StoreLastSafePosition(PatchController.storeSafePositionInterval));
            }

            // sync TPO's CheckTrap hitbox size (because it resets when loading a new scene)
            PatchController.SyncCheckTrapHitboxSize();
        }

        if (SceneHelper.MenuSceneLoaded)
        {
            PatchController.isStoringSafePosition = false;
            if (_storeSafePositionCoroutine != null)
            {
                try
                {
                    UIController.instance.StopCoroutine(_storeSafePositionCoroutine);
                }
                catch { }
            }
        }
    }

    protected override void OnDispose()
    {
        ConfigHandler.Save(config);
    }
}
