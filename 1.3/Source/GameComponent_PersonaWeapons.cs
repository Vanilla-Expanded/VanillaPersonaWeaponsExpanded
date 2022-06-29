using RimWorld;
using System.Collections.Generic;
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
                if (!Find.LetterStack.LettersListForReading.Contains(letter))
                {
                    var diff = Find.TickManager.TicksGame - letter.tickWhenOpened;
                    if (diff >= GenDate.TicksPerDay * 7)
                    {
                        if (letter.pawn.IsColonist)
                        {
                            Find.LetterStack.ReceiveLetter(letter);
                        }
                        else
                        {
                            unresolvedLetters.RemoveAt(i);
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
    }
}
