using GraphicCustomization;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace VanillaPersonaWeaponsExpanded
{
    [HotSwappable]
    public class Dialog_ChoosePersonaWeapon : Dialog_GraphicCustomization
    {
        public Thing currentWeapon;

        public ChoiceLetter_ChoosePersonaWeapon choiceLetter;
        public override Vector2 InitialSize => new Vector2(700, 600);

        public List<Thing> allWeapons;

        public List<WeaponTraitDef> allWeaponTraits;
        public WeaponTraitDef currentWeaponTrait;

        public List<AbilityDef> allPsycasts;
        public AbilityDef currentPsycast;
        public Dialog_ChoosePersonaWeapon(ChoiceLetter_ChoosePersonaWeapon choiceLetter, List<Thing> allWeapons,
            CompGraphicCustomization comp, Pawn pawn = null) : base(comp, pawn)
        {
            this.allWeapons = allWeapons;
            this.currentWeapon = comp.parent;
            this.choiceLetter = choiceLetter;
            this.allWeaponTraits = DefDatabase<WeaponTraitDef>.AllDefsListForReading;
            this.currentWeaponTrait = allWeaponTraits.RandomElement();
            if (ModCompatibility.VPELoaded && comp is CompGraphicCustomization_PsychicWeapon)
            {
                allPsycasts = ModCompatibility.AllPsycasts();
                currentPsycast = allPsycasts.RandomElement();
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect titleRect = DrawTitle(ref inRect);

            var floatMenuButtonsRect = new Rect(inRect.x + 150, titleRect.yMax, inRect.width - 300, 32);
            MakeFloatOptionButtons(floatMenuButtonsRect, 
                leftAction: delegate
                {
                    if (allWeapons.IndexOf(currentWeapon) > 0)
                    {
                        currentWeapon = allWeapons[allWeapons.IndexOf(currentWeapon) - 1];
                    }
                    else
                    {
                        currentWeapon = allWeapons[allWeapons.Count - 1];
                    }
                    this.Init(currentWeapon.TryGetComp<CompGraphicCustomization>());
                }, centerAction: delegate
                {
                    FloatMenuUtility.MakeMenu(allWeapons, entry => entry.def.LabelCap, (Thing variant) => delegate
                    {
                        currentWeapon = variant;
                        this.Init(currentWeapon.TryGetComp<CompGraphicCustomization>());
                    });
                }, centerButtonName: currentWeapon.def.LabelCap,
                rightAction: delegate
                {
                    if (allWeapons.IndexOf(currentWeapon) < allWeapons.Count - 1)
                    {
                        currentWeapon = allWeapons[allWeapons.IndexOf(currentWeapon) + 1];
                    }
                    else
                    {
                        currentWeapon = allWeapons[0];
                    }
                    this.Init(currentWeapon.TryGetComp<CompGraphicCustomization>());
                });

            var height = GetScrollHeight();
            Rect outerRect = new Rect(inRect.x, floatMenuButtonsRect.yMax + 15, inRect.width, 430);
            Rect viewArea = new Rect(inRect.x, outerRect.y, inRect.width - 16, height);
            Rect itemTextureRect = new Rect(inRect.x + 10, viewArea.y, 250, 250);
            DrawItem(itemTextureRect);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, viewArea, true);
            DrawCustomizationArea(itemTextureRect);
            Widgets.EndScrollView();

            var cancelRect = new Rect((inRect.width / 2f) - 155, inRect.height - 32, 150, 32);
            if (Widgets.ButtonText(cancelRect, "VEF.Cancel".Translate()))
            {
                this.Close();
            }
            var confirmRect = new Rect((inRect.width / 2f) + 5, inRect.height - 32, 250, 32);
            DrawConfirmButton(confirmRect, "VPWE.ClaimFor".Translate(pawn.Named("PAWN")), delegate
            {
                this.comp.texVariantsToCustomize = this.currentVariants;
                this.comp.Customize();
                if (compGeneratedName != null)
                {
                    compGeneratedName.name = currentName;
                }
                var compBladelink = currentWeapon.TryGetComp<CompBladelinkWeapon>();
                compBladelink.traits.Clear();
                compBladelink.traits.Add(this.currentWeaponTrait);
                if (comp is CompGraphicCustomization_PsychicWeapon compPsychicWeapon)
                {
                    compPsychicWeapon.ability = currentPsycast;
                }
                SendWeapon(pawn, compBladelink, currentWeapon); 
                Current.Game.GetComponent<GameComponent_PersonaWeapons>().unresolvedLetters.Remove(choiceLetter);
                Find.Archive.Remove(choiceLetter);
                this.Close();
            });
        }

        public static void SendWeapon(Pawn receiver, CompBladelinkWeapon compBladelink, Thing weaponToSend)
        {
            compBladelink.CodeFor(receiver);
            var map = receiver.MapHeld ?? Find.AnyPlayerHomeMap;
            var qualityComp = weaponToSend.TryGetComp<CompQuality>();
            if (qualityComp != null)
            {
                qualityComp.SetQuality(QualityCategory.Excellent, ArtGenerationContext.Outsider);
            }
            DropPodUtility.DropThingsNear(map.Center, map, new List<Thing> { weaponToSend }, 110, canInstaDropDuringInit: false, leaveSlag: true);
            Find.LetterStack.ReceiveLetter("VPWE.ReceivedWeaponTitle".Translate(), "VPWE.ReceivedWeaponDesc".Translate(weaponToSend.Label, receiver.Named("PAWN")), LetterDefOf.NeutralEvent, weaponToSend);
        }

        protected override void DrawCustomizationArea(Rect itemTextureRect)
        {
            base.DrawCustomizationArea(itemTextureRect);
            var personaTitleRect = new Rect(itemTextureRect.x, itemTextureRect.yMax + 26, 150, 24);
            DrawSelection(personaTitleRect, "VPWE.Persona".Translate(), delegate
            {
                if (allWeaponTraits.IndexOf(currentWeaponTrait) > 0)
                {
                    currentWeaponTrait = allWeaponTraits[allWeaponTraits.IndexOf(currentWeaponTrait) - 1];
                }
                else
                {
                    currentWeaponTrait = allWeaponTraits[allWeaponTraits.Count - 1];
                }
            }, delegate
            {
                FloatMenuUtility.MakeMenu(allWeaponTraits, entry => entry.LabelCap, (WeaponTraitDef variant) => delegate
                {
                    currentWeaponTrait = variant;
                });
            }, delegate
            {
                if (allWeaponTraits.IndexOf(currentWeaponTrait) < allWeaponTraits.Count - 1)
                {
                    currentWeaponTrait = allWeaponTraits[allWeaponTraits.IndexOf(currentWeaponTrait) + 1];
                }
                else
                {
                    currentWeaponTrait = allWeaponTraits[0];
                }
            }, currentWeaponTrait, 250f);
            if (ModCompatibility.VPELoaded && comp is CompGraphicCustomization_PsychicWeapon)
            {
                var psycastRect = new Rect(itemTextureRect.xMax +25f, personaTitleRect.y, 350f, personaTitleRect.height);
                DrawSelection(psycastRect, "VPWE.Psycast".Translate(), delegate
                {
                    if (allPsycasts.IndexOf(currentPsycast) > 0)
                    {
                        currentPsycast = allPsycasts[allPsycasts.IndexOf(currentPsycast) - 1];
                    }
                    else
                    {
                        currentPsycast = allPsycasts[allPsycasts.Count - 1];
                    }
                }, delegate
                {
                    var list = new List<FloatMenuOption>();
                    foreach (var psycast in allPsycasts)
                    {
                        var curPsy = psycast;
                        var floatOption = new PsycastFloatMenuOption(psycast.LabelCap, delegate
                        {
                            currentPsycast = curPsy;
                        }, itemIcon: curPsy.icon, iconColor: Color.white);
                        floatOption.SetSizeMode(FloatMenuSizeMode.Normal);
                        list.Add(floatOption);
                    }
                    Find.WindowStack.Add(new FloatMenu(list));
                }, delegate
                {
                    if (allPsycasts.IndexOf(currentPsycast) < allPsycasts.Count - 1)
                    {
                        currentPsycast = allPsycasts[allPsycasts.IndexOf(currentPsycast) + 1];
                    }
                    else
                    {
                        currentPsycast = allPsycasts[0];
                    }
                }, currentPsycast, 350f);
            }
        }

        private void DrawSelection(Rect selectionRect, string title, Action leftAction, Action centerAction, Action rightAction, 
            Def currentItem, float floatMenuWidth)
        {
            Widgets.Label(selectionRect, title);
            var floatMenuButtonsRect = new Rect(selectionRect.x, selectionRect.yMax, floatMenuWidth, 32);
            MakeFloatOptionButtons(floatMenuButtonsRect, leftAction: leftAction, centerAction: centerAction, 
                centerButtonName: currentItem.LabelCap, rightAction: rightAction);
            var description = currentItem.LabelCap + ": " + currentItem.description.CapitalizeFirst();
            var height = Text.CalcHeight(description, floatMenuButtonsRect.width);
            var descriptionRect = new Rect(floatMenuButtonsRect.x, floatMenuButtonsRect.yMax + 15, floatMenuButtonsRect.width, height);
            Widgets.Label(descriptionRect, description);
        }

        public override float GetScrollHeight()
        {
            var baseNumber = base.GetScrollHeight();
            var weaponTraitDescription = currentWeaponTrait.LabelCap + ": " + currentWeaponTrait.description.CapitalizeFirst();
            var height = Text.CalcHeight(weaponTraitDescription, 250);
            return Mathf.Max(250 + 24 + 32 + height + 35, baseNumber);
        }
        
        protected override void Randomize()
        {
            base.Randomize();
            this.currentWeaponTrait = allWeaponTraits.RandomElement();
        }
    }
}
