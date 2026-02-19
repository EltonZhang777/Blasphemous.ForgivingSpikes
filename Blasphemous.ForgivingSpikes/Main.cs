using BepInEx;
using Blasphemous.ModdingAPI;

namespace Blasphemous.ForgivingSpikes;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "3.0.0")]
[BepInDependency("Blasphemous.CheatConsole", "1.0.0")]
internal class Main : BaseUnityPlugin
{
    internal static ForgivingSpikes ForgivingSpikes { get; private set; }

    private void Start()
    {
        ForgivingSpikes = new ForgivingSpikes();
    }

    internal static void LogIfDebug(string message)
    {
#if DEBUG
        ModLog.Warn($"[DEBUG] {message}");
#endif
    }
}
