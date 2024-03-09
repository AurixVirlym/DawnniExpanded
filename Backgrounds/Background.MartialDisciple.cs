using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System.Collections.Generic;

namespace Dawnsbury.Mods.DawnniExpanded.Backgrounds
{
    /// <summary>
    /// Creates the Martial Disciple background
    /// </summary>
    public class BackgroundMartialDisciple
    {
        /// <summary>
        /// The actual Martial Disciple background feat (assuming you called LoadMod())
        /// </summary>
        public static BackgroundSelectionFeat MartialDiscipleBackground;
        /// <summary>
        /// Loads the Martial Disciple background into the Dawnsbury Days game.
        /// </summary>
        public static void LoadMod()
        {

            MartialDiscipleBackground = new BackgroundSelectionFeat(FeatName.CustomFeat, "You dedicated yourself to intense training and rigorous study to become a great warrior. The school you attended might have been a traditionalist monastery, an elite military academy, or the local branch of a prestigious mercenary organization."
                , "You're trained in the {b}Athletics{/b} skill. You gain the {b}Powerful Leap{/b} feat."
                , new List<AbilityBoost> { new LimitedAbilityBoost(Ability.Strength, Ability.Dexterity), new FreeAbilityBoost() })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFeat(FeatPowerfulLeap.PowerfulLeapTrueFeat, null);
                    sheet.GrantFeat(FeatName.Athletics);
                }).WithCustomName("Martial Disciple")
                as BackgroundSelectionFeat;
            MartialDiscipleBackground.Traits.Add(DawnniExpanded.DETrait);
            ModManager.AddFeat(MartialDiscipleBackground);
        }
    }
}
