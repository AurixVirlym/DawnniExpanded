using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.DawnniExpanded
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
                , "Choose two ability boosts. One must be to {b}Strength{/b} or {b}Dexterity{/b}, and one is a free ability boost.\r\nYou're trained in the{b}Athletics{/b} skill. You gain the {b}Powerful Leap{/b} feat."
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
