using HarmonyLib;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("VanillaPersonaWeaponsExpanded.Mod").PatchAll();
        }
    }
}
