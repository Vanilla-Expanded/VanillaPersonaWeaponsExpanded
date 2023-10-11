using GraphicCustomization;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    public class ChoiceLetter_ChoosePersonaWeapon : ChoiceLetter
	{
		public int tickWhenOpened;

        public Pawn pawn;
        public override bool CanShowInLetterStack => base.CanShowInLetterStack;
        public override bool CanDismissWithRightClick => false;

        public IEnumerable<ThingDef> AllPersonaWeapons
        {
            get
            {
                foreach (var def in DefDatabase<ThingDef>.AllDefs)
                {
                    if (def.GetCompProperties<CompProperties_BladelinkWeapon>() != null)
                    {
                        if (def.weaponTags != null && def.weaponTags.Any(x => x == "ExcludeFromEmpireTitleReward"))
                        {
                            continue;
                        }
                        yield return def;
                    }
                }
            }
        }
        public override IEnumerable<DiaOption> Choices
		{
			get
			{
				foreach (var def in AllPersonaWeapons)
                {
                    yield return Option_ChooseWeapon(def);
                }
                yield return new DiaOption("VPWE.Postpone".Translate())
				{
					resolveTree = true, action = delegate
                    {
						this.tickWhenOpened = Find.TickManager.TicksGame;
                        Find.LetterStack.RemoveLetter(this);
                    }
				};
			}
		}

        public override void OpenLetter()
        {
            base.OpenLetter();
            this.tickWhenOpened = Find.TickManager.TicksGame;
        }
        private DiaOption Option_ChooseWeapon(ThingDef weaponDef)
		{
			return new DiaOption("VPWE.ClaimWeapon".Translate(weaponDef.LabelCap))
			{
				action = delegate
                {
                    if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<CompBladelinkWeapon>() != null)
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox("VPWE.AlreadyBondedWarning".Translate(pawn.Named("PAWN"),
                            pawn.equipment.Primary.Label), "Yes".Translate(), delegate
                            {
                                OpenChooseDialog(weaponDef);
                            }, "No".Translate()));
                    }
                    else
                    {
                        OpenChooseDialog(weaponDef);
                    }
                },
				resolveTree = true
			};
		}

        private void OpenChooseDialog(ThingDef weaponDef)
        {
            Find.LetterStack.RemoveLetter(this);
            this.tickWhenOpened = Find.TickManager.TicksGame;
            var weapon = ThingMaker.MakeThing(weaponDef, GenStuff.DefaultStuffFor(weaponDef));
            var allWeapons = new List<Thing>
                    {
                        weapon
                    };
            foreach (var otherDef in AllPersonaWeapons.Where(x => x != weaponDef))
            {
                allWeapons.Add(ThingMaker.MakeThing(otherDef, GenStuff.DefaultStuffFor(otherDef)));
            }
            var comp = weapon.TryGetComp<CompGraphicCustomization>();
            if (comp != null)
            {
                Find.WindowStack.Add(new Dialog_ChoosePersonaWeapon(this, allWeapons, comp, pawn));
            }
            else
            {
                Dialog_ChoosePersonaWeapon.SendWeapon(pawn, weapon.TryGetComp<CompBladelinkWeapon>(), weapon);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref tickWhenOpened, "tickWhenOpened");
            Scribe_References.Look(ref pawn, "pawn");
        }
    }
}
