using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VanillaPersonaWeaponsExpanded
{
    public class GameComponent_PersonaWeapons : GameComponent
    {
        public List<ChoiceLetter_ChoosePersonaWeapon> unresolvedLetters = new List<ChoiceLetter_ChoosePersonaWeapon>();
        public GameComponent_PersonaWeapons(Game game)
        {
            
        }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            for (var i = unresolvedLetters.Count - 1; i >= 0; i--)
            {
                var letter = unresolvedLetters[i];
                if (letter == null || !IsAllowedToGetWeapon(letter.pawn))
                {
                    unresolvedLetters.RemoveAt(i);
                }
                else
                {
                    if (!Find.LetterStack.LettersListForReading.Contains(letter))
                    {
                        var diff = Find.TickManager.TicksGame - letter.tickWhenOpened;
                        if (diff >= GenDate.TicksPerDay * 7)
                        {
                            if (letter.pawn.IsColonist)
                            {
                                var map = letter.pawn.MapHeld ?? Find.AnyPlayerHomeMap;
                                if (map != null)
                                {
                                    Find.LetterStack.ReceiveLetter(letter);
                                }
                            }
                            else
                            {
                                unresolvedLetters.RemoveAt(i);
                            }
                        }
                    }
                }

            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref unresolvedLetters, "unresolvedLetters", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                unresolvedLetters ??= new List<ChoiceLetter_ChoosePersonaWeapon>();
            }
        }

        public static AcceptanceReport IsAllowedToGetWeapon(Pawn pawn)
        {
            if (pawn == null)
                return AcceptanceReport.WasRejected;

            // Check if the pawn is dead
            if (pawn.Dead)
                return "VPWE.NotAllowedDead".Translate(pawn.Named("PAWN"));

            // Check if pawn has no royalty tracker, or is below baron in seniority.
            if (pawn.royalty == null || pawn.royalty.AllTitlesForReading.All(t => t.faction != Faction.OfEmpire || t.def.seniority < VPWE_DefOf.Baron.seniority))
                return "VPWE.NotAllowedNoTitle".Translate(pawn.Named("PAWN"));

            return AcceptanceReport.WasAccepted;
        }
    }
}
