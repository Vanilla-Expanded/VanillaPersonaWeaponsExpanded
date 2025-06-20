using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), "OnPostTitleChanged")]
    public static class Pawn_RoyaltyTracker_OnPostTitleChanged
    {
        public static void Postfix(Pawn_RoyaltyTracker __instance, Faction faction, RoyalTitleDef prevTitle, RoyalTitleDef newTitle)
        {
            if (newTitle != null && __instance.pawn.IsColonist && PawnGenerator.IsBeingGenerated(__instance.pawn) is false 
                && Current.CreatingWorld is null && __instance.pawn.Dead is false
                && (prevTitle is null || prevTitle.seniority < VPWE_DefOf.Baron.seniority) 
                && newTitle.seniority >= VPWE_DefOf.Baron.seniority && faction == Faction.OfEmpire)
            {
                var letter = LetterMaker.MakeLetter("VPWE.GainedPersonaWeaponTitle".Translate(__instance.pawn.Named("PAWN")),
                    "VPWE.GainedPersonaWeaponDesc".Translate(__instance.pawn.Named("PAWN"), newTitle.GetLabelFor(__instance.pawn.gender)),
                    VPWE_DefOf.VPWE_ChoosePersonaWeapon, faction) as ChoiceLetter_ChoosePersonaWeapon;
                letter.pawn = __instance.pawn;
                Find.LetterStack.ReceiveLetter(letter);
                Current.Game.GetComponent<GameComponent_PersonaWeapons>().unresolvedLetters.Add(letter);
            }
        }
    }
}
