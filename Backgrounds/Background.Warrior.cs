using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System.Collections.Generic;

namespace Dawnsbury.Mods.DawnniExpanded.Backgrounds
{
    /// <summary>
    /// Creates the Warrior Background
    /// </summary>
    public class BackgroundWarrior
    {
        /// <summary>
        /// The actual Warrior Background feat (assuming you called LoadMod())
        /// </summary>
        public static BackgroundSelectionFeat WarriorBackground;
        /// <summary>
        /// Loads the Warrior Background into the Dawnsbury Days game.
        /// </summary>
        public static void LoadMod()
        {

            WarriorBackground = new BackgroundSelectionFeat(FeatName.CustomFeat, "In your younger days, you waded into battle as a mercenary, a warrior defending a nomadic people, or a member of a militia or army. You might have wanted to break out from the regimented structure of these forces, or you could have always been as independent a warrior as you are now."
                , "You're trained in the {b}Intimidation{/b} skill. You gain the {b}Intimidating Glare{/b} feat."
                , new List<AbilityBoost> { new LimitedAbilityBoost(Ability.Strength, Ability.Constitution), new FreeAbilityBoost() })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.GrantFeat(FeatName.IntimidatingGlare);
                    sheet.GrantFeat(FeatName.Intimidation);
                }).WithCustomName("Warrior")
                as BackgroundSelectionFeat;
            WarriorBackground.Traits.Add(DawnniExpanded.DETrait);
            ModManager.AddFeat(WarriorBackground);
        }
    }
}
