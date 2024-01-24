﻿using HarmonyLib;
using System.Reflection;
using Verse;
using VFECore.Abilities;

namespace VanillaPersonaWeaponsExpanded
{
    [HarmonyPatch]
    public static class Hediff_Psylink_InitializeFromPsylink_Patch
    {
        public static bool Prepare() => ModCompatibility.VPELoaded;
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("VanillaPsycastsExpanded.Hediff_PsycastAbilities:InitializeFromPsylink");
        }

        public static void Postfix(Hediff_Psylink psylink)
        {
            var equipment = psylink.pawn.equipment?.Primary;
            if (equipment != null)
            {
                var comp = equipment.GetComp<CompGraphicCustomization_PsychicWeapon>();
                if (comp != null)
                {
                    var compAbilities = psylink.pawn.GetComp<CompAbilities>();
                    if (comp == null) return;
                    if (compAbilities.HasAbility(comp.ability) is false)
                    {
                        comp.TryGiveAbility(psylink.pawn);
                    }
                }
            }
        }
    }
}
