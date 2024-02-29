using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System.Collections.Generic;

namespace Dawnsbury.Mods.DawnniExpanded
{
    /// <summary>
    /// creates the Field Medic background
    /// </summary>
    public class BackgroundFieldMedic
    {
        /// <summary>
        /// the actual Field Medic background (assuming you called LoadMod())
        /// </summary>
        public static BackgroundSelectionFeat FieldMedicBackground;
        /// <summary>
        /// Loads the Field Medic Background into the Dawnsbury Days game
        /// </summary>
        public static void LoadMod()
        {
            
            FieldMedicBackground = new BackgroundSelectionFeat(FeatName.CustomFeat, "In the chaotic rush of battle, you learned to adapt to rapidly changing conditions as you administered to battle casualties. You patched up soldiers, guards, or other combatants, and learned a fair amount about the logistics of war."
                , "Choose two ability boosts. One must be to {b}Constitution{/b} or {b}Wisdom{/b}, and one is a free ability boost.\r\nYou're trained in the {b}Medicine{/b} skill. You gain the {b}Battle Medicine{/b} feat."
                , new List<AbilityBoost>{ new LimitedAbilityBoost(Ability.Constitution, Ability.Wisdom), new FreeAbilityBoost() } )
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFeat(FeatBattleMedicine.BattleMedicineTrueFeat,null);
                    sheet.GrantFeat(FeatName.Medicine);
                }).WithCustomName("Field Medic")
                as BackgroundSelectionFeat;
            FieldMedicBackground.Traits.Add(DawnniExpanded.DETrait);
            ModManager.AddFeat(FieldMedicBackground);
        }
    }
}
