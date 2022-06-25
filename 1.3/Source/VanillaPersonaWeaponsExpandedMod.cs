using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    public class VanillaPersonaWeaponsExpandedMod : Mod
    {
        public VanillaPersonaWeaponsExpandedMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VanillaPersonaWeaponsExpanded.Mod").PatchAll();
        }
    }
    
    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), "OnPostTitleChanged")]
    public static class Pawn_RoyaltyTracker_OnPostTitleChanged
    {
        public static void Postfix(Pawn_RoyaltyTracker __instance, Faction faction, RoyalTitleDef prevTitle, RoyalTitleDef newTitle)
        {
            if ((prevTitle is null || prevTitle.seniority < newTitle.seniority) && (newTitle == VPWE_DefOf.Baron 
                || prevTitle is null && newTitle.seniority > VPWE_DefOf.Baron.seniority) && faction == Faction.OfEmpire)
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
