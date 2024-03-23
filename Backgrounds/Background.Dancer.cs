using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System.Collections.Generic;


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
                , "You're trained in the {b}Acrobatics{/b} skill. You gain the {b}Feather Step{/b} feat."
                , new List<AbilityBoost> { new LimitedAbilityBoost(Ability.Dexterity, Ability.Charisma), new FreeAbilityBoost() })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.GrantFeat(FeatName.FeatherStep);
                    sheet.GrantFeat(FeatName.Acrobatics);
                }).WithCustomName("Dancer")
                as BackgroundSelectionFeat;
            DancerBackground.Traits.Add(DawnniExpanded.DETrait);
            DancerBackground.Traits.Add(DawnniExpanded.HomebrewTrait);
            ModManager.AddFeat(DancerBackground);
        }
    }
}
