using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    public static class VPWE_DefOf
    {
        public static RoyalTitleDef Baron;
    }
    public class VanillaPersonaWeaponsExpandedMod : Mod
    {
        public VanillaPersonaWeaponsExpandedMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VanillaPersonaWeaponsExpanded.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), "TryUpdateTitle")]
    public static class Pawn_RoyaltyTracker_TryUpdateTitle
    {
        public static void Postfix(bool __result, Faction faction, bool sendLetter = true, RoyalTitleDef updateTo = null)
        {
            if (__result && updateTo == VPWE_DefOf.Baron)
            {
                
            }
        }
    }
}
