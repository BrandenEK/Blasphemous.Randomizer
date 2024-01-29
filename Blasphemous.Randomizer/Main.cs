using BepInEx;

namespace Blasphemous.Randomizer;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.0.1")]
public class Main : BaseUnityPlugin
{
    public static Randomizer Randomizer { get; private set; }

    private void Start()
    {
        Randomizer = new Randomizer();
    }
}
