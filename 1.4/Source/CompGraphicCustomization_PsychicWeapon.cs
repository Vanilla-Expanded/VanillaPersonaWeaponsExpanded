using GraphicCustomization;
using RimWorld;
using System.Collections.Generic;
using Verse;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace VanillaPersonaWeaponsExpanded
{
    public class CompGraphicCustomization_PsychicWeapon : CompGraphicCustomization
    {
        public AbilityDef ability;

        public bool alreadyHad;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref ability, nameof(ability));
            Scribe_Values.Look(ref alreadyHad, nameof(alreadyHad));
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (ability is null || ModCompatibility.VPELoaded is false || ModCompatibility.PawnIsPsycaster(pawn) is false)
            {
                return;
            }
            var comp = pawn.GetComp<CompAbilities>();
            if (comp == null) return;
            alreadyHad = comp.HasAbility(ability);
            if (!alreadyHad) comp.GiveAbility(ability);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
            if (ModCompatibility.VPELoaded is false)
            {
                return;
            }
            if (ability == null) return;
            if (!alreadyHad) pawn.GetComp<CompAbilities>().LearnedAbilities.RemoveAll(ab => ab.def == ability);
            alreadyHad = false;
        }


        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (ability != null)
            {
                yield return new StatDrawEntry(StatCategoryDefOf.Weapon_Melee, "VREA.GivesAbility".Translate(), ability.LabelCap, ability.description, 0);
            }
        }
    }
}
