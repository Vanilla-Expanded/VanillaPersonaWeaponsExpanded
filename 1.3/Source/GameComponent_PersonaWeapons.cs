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
            foreach (var letter in unresolvedLetters)
            {
                if (!Find.LetterStack.LettersListForReading.Contains(letter))
                {
                    var diff = Find.TickManager.TicksGame - letter.tickWhenOpened;
                    if (diff >= GenDate.TicksPerDay * 7)
                    {
                        Find.LetterStack.ReceiveLetter(letter);
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
