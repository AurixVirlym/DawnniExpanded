using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using System;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Display;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Data;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeBeastmaster
{

  public static Feat BeastMasterDedicationFeat;
  public static Feat BeastMasterMatureFeat;

  public static QEffect MatureEffect = new QEffect()
  {

    BonusToAbilityBasedChecksRollsAndDCs =
    (QEffect qf, Ability ability) => ability == Ability.Dexterity || ability == Ability.Strength || ability == Ability.Constitution || ability == Ability.Wisdom ? new Bonus(1, BonusType.Untyped, "Mature") : (Bonus)null,

    StartOfCombat = (qf =>
      {
        qf.Owner.Abilities.Strength += 20;

        ++qf.Owner.UnarmedStrike.WeaponProperties.DamageDieCount;
        return null;

      })
  };

  private static Feat WithCompanionPermanentQEffect(
    this Feat feat,
    string shortRulesText,
    QEffect AnimalQeffect)
  {
    feat.WithOnCompanion((Action<Creature, Creature>)((target, ranger) =>
    {
      target.AddQEffect(AnimalQeffect);
    }));
    return feat;
  }

  private static Feat WithOnCompanion(this Feat feat, Action<Creature, Creature> action)
  {
    feat.WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet => sheet.RangerBenefitsToCompanion += action));
    return feat;
  }

  public static void LoadMod()

  {

    BeastMasterDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You attract the loyalty of animal.",
            "You gain the service of a young animal companion that travels with you and obeys your commands. \n\nYou may still take this archetype if you have an animal companion but you should consider retraining if possible.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Beastmaster Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Nature) >= Proficiency.Trained, "You must be trained in Nature.")
            .WithOnSheet(sheet =>
            {

              if (sheet.AllFeatNames.Contains(FeatName.AnimalCompanion))
              {


              }
              else
              {

                sheet.AdditionalClassTraits.Add(Trait.Ranger);

                sheet.AddSelectionOption(
                  new SingleFeatSelectionOption(
                      "Beastmaster Companion",
                      "Beastmaster Companion",
                      -1,
                      (Feat ft) =>
                {

                  if (ft.FeatName == FeatName.AnimalCompanion)
                  {
                    return true;
                  }
                  else return false;
                }));
              }
            });




    BeastMasterMatureFeat = new TrueFeat(FeatName.CustomFeat,
                4,
                "Your animal companion grows up, becoming a mature animal companion and gaining additional capabilities.", "Your animal companion gains the following benefits:\r\n• It gets +1 to Strength, Dexterity, Constitution and Wisdom.\r\n• Its unarmed attack damage increases from one die to two dice (for example, from 1d8 to 2d8).\r\n• Its proficiency with Perception and all saving throws increases to Expert (an effective +2 to Perception and all saves).\r\n• Its proficiency in Intimidation, Stealth and Survival increases by one step (from untrained to trained; or from trained to expert).\r\n• Instead of spending an action to command the animal companion, you can have it act independently as {icon:FreeAction} a free action. If you do, it only gains one action, not two, and it can only use it to move or to make a Strike {i}(you still decide where it moves or who it attacks){/i}.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
                .WithCustomName("Mature Beastmaster Companion")
                .WithEquivalent(values => values.AllFeats.Contains(ArchetypeDruid.DruidMatureFeat))
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BeastMasterDedicationFeat) && values.AllFeatNames.Contains(FeatName.AnimalCompanion), "You must have the beastmaster dedication feat and have selected an Animal Companion.")
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                  sheet.GrantFeat(FeatName.MatureAnimalCompanionDruid);
                }

                );
  



    ModManager.AddFeat(BeastMasterDedicationFeat);
    ModManager.AddFeat(BeastMasterMatureFeat);
  }
}