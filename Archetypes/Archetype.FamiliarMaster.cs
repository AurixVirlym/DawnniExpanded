using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
using Dawnsbury.Mods.DawnniExpanded.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeFamiliarMaster
{

  public static Feat FamiliarMasterDedication;
  public static Feat EnhancedFamiliarArchetype;

  public static Trait FamiliarMasterTrait = ModManager.RegisterTrait(
        "DE_FamiliarMasterTrait",
        new TraitProperties("Familiar Master", false)
  {
  });
  public static void LoadMod()

  {

    FamiliarMasterDedication = ArchetypeFeats.CreateAgnosticArchetypeDedication(FamiliarMasterTrait, "You have forged a mystical bond with a creature. This might have involved complex rituals and invocations, such as meditating under the moon until something crept out of the forest. Or maybe you just did each other a good turn, such as rescuing the beast from a trap or a foe, and then being rescued in turn. Whatever the details, you are now comrades until the end.",
        "You gain a familiar. If you already have a familiar, you gain the Enhanced Familiar feat.")
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
    FamiliarMasterDedication.Traits.Add(DawnniExpanded.DETrait);

    EnhancedFamiliarArchetype = new TrueFeat(ModManager.RegisterFeatName("DE_EnhancedFamiliar", "Enhanced Familiar"),
                    4,
                    "You infuse your familiar with additional magical energy.",
                    "You can select two more familiar abilities.",
                    new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, Trait.Homebrew })
                    .WithAvailableAsArchetypeFeat(FamiliarMasterTrait)
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