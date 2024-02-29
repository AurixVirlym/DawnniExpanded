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

namespace Dawnsbury.Mods.DawnniExpanded.Backgrounds
{
    /// <summary>
    /// Creates the Dancer Background
    /// </summary>
    public class BackgroundDancer
    {
        /// <summary>
        /// The actual Dancer Background feat (assuming you called LoadMod())
        /// </summary>
        public static BackgroundSelectionFeat DancerBackground;
        /// <summary>
        /// Loads the Dancer Background into the Dawnsbury Days game.
        /// </summary>
        public static void LoadMod()
        {

            DancerBackground = new BackgroundSelectionFeat(FeatName.CustomFeat, "In your younger days, you would dance, perhaps professionally, perhaps in private. Either way, this experience has proved valuable to your career as an adventurer."
                , "Choose two ability boosts. One must be to {b}Dexterity{/b} or {b}Charisma{/b}, and one is a free ability boost.\r\nYou're trained in the{b}Acrobatics{/b} skill. You gain the {b}Feather Step{/b} feat."
                , new List<AbilityBoost> { new LimitedAbilityBoost(Ability.Dexterity, Ability.Charisma), new FreeAbilityBoost() })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.GrantFeat(FeatName.FeatherStep);
                    sheet.GrantFeat(FeatName.Acrobatics);
                }).WithCustomName("Dancer")
                as BackgroundSelectionFeat;
            DancerBackground.Traits.Add(DawnniExpanded.DETrait);
            DancerBackground.Traits.Add(Trait.Homebrew);
            ModManager.AddFeat(DancerBackground);
        }
    }
}
