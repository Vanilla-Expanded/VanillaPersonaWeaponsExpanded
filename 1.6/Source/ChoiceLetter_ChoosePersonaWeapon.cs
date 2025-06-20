using RimWorld;
using System.Collections.Generic;
using System.Linq;
using VEF.Graphics;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    public class ChoiceLetter_ChoosePersonaWeapon : ChoiceLetter
	{
		public int tickWhenOpened;

        public Pawn pawn;
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
                var allowed = GameComponent_PersonaWeapons.IsAllowedToGetWeapon(pawn);

                foreach (var def in AllPersonaWeapons)
                {
                    yield return Option_ChooseWeapon(def, allowed);
                }
                yield return new DiaOption("VPWE.Postpone".Translate())
				{
					resolveTree = true, action = delegate
                    {
						this.tickWhenOpened = Find.TickManager.TicksGame;
                        Find.LetterStack.RemoveLetter(this);
                    },
                    disabled = !allowed,
                    disabledReason = allowed.Reason
				};
                yield return new DiaOption("VPWE.RejectLetter".Translate())
                {
                    resolveTree = true,
                    action = () =>
                    {
                        if (allowed)
                        {
                            var dialog = Dialog_MessageBox.CreateConfirmation("VPWE.ConfirmReject".Translate(pawn.Named("PAWN")), RemoveAndResolveLetter, true);
                            Find.WindowStack.Add(dialog);
                        }
                        else
                        {
                            RemoveAndResolveLetter();
                        }
                    }
                };
            }
		}

        public override void OpenLetter()
        {
            base.OpenLetter();
            this.tickWhenOpened = Find.TickManager.TicksGame;
        }
        private DiaOption Option_ChooseWeapon(ThingDef weaponDef, AcceptanceReport allowed)
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
				resolveTree = true,
                disabled = !allowed,
                disabledReason = allowed.Reason
			};
		}

        private void OpenChooseDialog(ThingDef weaponDef)
        {
            this.tickWhenOpened = Find.TickManager.TicksGame;
            var weapon = ThingMaker.MakeThing(weaponDef, GenStuff.DefaultStuffFor(weaponDef));
            var comp = weapon.TryGetComp<CompGraphicCustomization>();
            // If the weapon doesn't have customization comp, show confirmation dialog and return early. No point in doing anything else.
            if (comp == null)
            {
                var dialog = Dialog_MessageBox.CreateConfirmation("VPWE.NoCustomizationWeapon".Translate(weaponDef.Named("WEAPON")).CapitalizeFirst(), () =>
                {
                    if (Dialog_ChoosePersonaWeapon.SendWeapon(pawn, weapon.TryGetComp<CompBladelinkWeapon>(), weapon))
                        RemoveAndResolveLetter();
                });
                Find.WindowStack.Add(dialog);
                return;
            }

            var allWeapons = new List<Thing>
                    {
                        weapon
                    };
            foreach (var otherDef in AllPersonaWeapons.Where(x => x != weaponDef && x.HasComp<CompGraphicCustomization>()))
            {
                allWeapons.Add(ThingMaker.MakeThing(otherDef, GenStuff.DefaultStuffFor(otherDef)));
            }

            Find.WindowStack.Add(new Dialog_ChoosePersonaWeapon(this, allWeapons, comp, pawn));
        }

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref tickWhenOpened, "tickWhenOpened");
            Scribe_References.Look(ref pawn, "pawn");
        }

        public void RemoveAndResolveLetter()
        {
            Current.Game.GetComponent<GameComponent_PersonaWeapons>().unresolvedLetters.Remove(this);
            Find.LetterStack.RemoveLetter(this);
        }
    }
}
