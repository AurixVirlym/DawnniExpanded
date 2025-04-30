using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using System;

using Dawnsbury.Mods.DawnniExpanded.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeFamiliarMaster
{

  public static Feat FamiliarMasterDedication;
  public static Feat EnhancedFamiliarArchetype;

  public static Trait FamiliarMasterTrait = ModManager.RegisterTrait(
        "FamiliarMasterTrait",
        new TraitProperties("FamiliarMasterTrait", false, "", false)
  {
  });
  public static void LoadMod()

  {

    FamiliarMasterDedication = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have forged a mystical bond with a creature. This might have involved complex rituals and invocations, such as meditating under the moon until something crept out of the forest. Or maybe you just did each other a good turn, such as rescuing the beast from a trap or a foe, and then being rescued in turn. Whatever the details, you are now comrades until the end.",
            "You gain a familiar. If you already have a familiar, you gain the Enhanced Familiar feat.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Familiar Master Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => !values.AllFeats.Contains(Familiars.EnhancedFamiliar), "You already have the Enhanced Familiar feat.")
            .WithOnSheet(sheet => 
            {
              
              if (sheet.AllFeats.Contains<Feat>(Familiars.Familiar)){
              sheet.AddFeat(EnhancedFamiliarArchetype, null);
              } 
              else {
              sheet.AddFeat(Familiars.Familiar,null);
              }
            }
            );

    EnhancedFamiliarArchetype = new TrueFeat(FeatName.CustomFeat,
                    4,
                    "You infuse your familiar with additional magical energy.",
                    "You can select two more familiar abilities.",
                    new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, Trait.Homebrew })
                    .WithCustomName("Enhanced Familiar (Archetype)")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(FamiliarMasterDedication), "You must have the Familiar Master Dedication feat.")
                    .WithEquivalent(values => values.AllFeats.Contains(Familiars.EnhancedFamiliar))
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

            {
              int abilitynumber = 2;
              sheet.AddSelectionOption(
          new MultipleFeatSelectionOption(
              "Enhanced Familiar Ability Selection",
              "Enhanced Familiar Ability Selection",
              -1,
              (ft) => ft.HasTrait(Familiars.FamiliarAbilityTrait), abilitynumber));
            });


    ModManager.AddFeat(FamiliarMasterDedication);
    ModManager.AddFeat(EnhancedFamiliarArchetype);

  }
}