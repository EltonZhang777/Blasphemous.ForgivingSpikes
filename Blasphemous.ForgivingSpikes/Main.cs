using BepInEx;

namespace Blasphemous.ForgivingSpikes
{
    [BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
    [BepInDependency("Blasphemous.ModdingAPI", "0.1.0")]
    public class Main : BaseUnityPlugin
    {
        public static ForgivingSpikes ForgivingSpikes { get; private set; }

        private void Start()
        {
            ForgivingSpikes = new ForgivingSpikes();
        }
    }
}
