using System.Collections.Generic;
using System.Linq;
using Verse;
using VEF.Abilities;

namespace VanillaPersonaWeaponsExpanded
{
    public static class ModCompatibility
    {
        public static bool VPELoaded = ModsConfig.IsActive("VanillaExpanded.VPsycastsE");

        public static List<AbilityDef> AllPsycasts()
        {
            return DefDatabase<AbilityDef>.AllDefs.Where(x => VanillaPsycastsExpanded.AbilityExtensionPsycastUtility
            .Psycast(x) != null).ToList();
        }

        public static bool PawnIsPsycaster(Pawn pawn)
        {
            return VanillaPsycastsExpanded.PsycastUtility.Psycasts(pawn) != null;
        }
    }
}
