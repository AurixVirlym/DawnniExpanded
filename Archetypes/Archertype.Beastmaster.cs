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
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Nature) <= Proficiency.Trained, "You must be trained in Nature.")
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
                "Your animal companions has grown up, becoming a mature animal companion and gaining additional capabilities.",
                "The following increases are applied to your animal companion:"
                + "\n\n- Strength, Dexterity, Constitution, and Wisdom modifiers increase by 1."
                + "\n- Unarmed attack damage increases from one die to two dice."
                + "\n- Proficiency rank for Perception and all saving throws increases to expert."
                + "\n- Proficiency ranks in Intimidation, Stealth, and Survival increase to trained, and if it was already trained in one of those skills from its type, the proficiency rank in that skill increases to expert."
                + "\n\nEven if you don't use the Command an Animal action, your animal companion can still use 1 action at the end of your turn.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
                .WithCustomName("Mature Beastmaster Companion")
                .WithEquivalent(values => values.AllFeats.Contains(ArchetypeDruid.DruidMatureFeat))
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BeastMasterDedicationFeat) && values.AllFeatNames.Contains(FeatName.AnimalCompanion), "You must have the beastmaster dedication feat and have selected an Animal Companion.")
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                  sheet.RangerBenefitsToCompanion += (Action<Creature, Creature>)((companion, ranger) =>
                      {
                        companion.MaxHP += companion.Level;
                        companion.Abilities.Strength += 1;
                        companion.Abilities.Dexterity += 1;
                        companion.Abilities.Constitution += 1;
                        companion.Abilities.Wisdom += 1;
                        if (companion.UnarmedStrike.WeaponProperties.DamageDieCount == 1)
                        {
                          companion.UnarmedStrike.WeaponProperties.DamageDieCount += 1;
                        }


                        foreach (QEffect qf in companion.QEffects.Where<QEffect>(qf => qf.AdditionalUnarmedStrike != null))
                        {
                          if (qf.AdditionalUnarmedStrike.WeaponProperties.DamageDieCount == 1)
                          {
                            qf.AdditionalUnarmedStrike.WeaponProperties.DamageDieCount += 1;
                          }
                        }

                        companion.Perception += 2;
                        companion.Proficiencies.Set(Trait.Perception, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Fortitude, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Will, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Reflex, Proficiency.Expert);

                        if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Trained)
                        {
                          sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Untrained)
                        {
                          sheet.SetProficiency(Trait.Survival, Proficiency.Trained);
                        }

                        if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Trained)
                        {
                          sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Untrained)
                        {
                          sheet.SetProficiency(Trait.Intimidation, Proficiency.Trained);
                        }

                        if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Trained)
                        {
                          sheet.SetProficiency(Trait.Stealth, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Untrained)
                        {
                          sheet.SetProficiency(Trait.Stealth, Proficiency.Trained);
                        }

                      });

                }

                ).WithPermanentQEffect("If you don't command your companion, they will act with 1 action at end of your turn.",
                (qf => qf.EndOfYourTurn = (Func<QEffect, Creature, Task>)(async (qfSelf, you) =>
                    {
                      Creature animalCompanion = you.Battle.AllCreatures.FirstOrDefault<Creature>((Func<Creature, bool>)(cr => cr.QEffects.Any<QEffect>((Func<QEffect, bool>)(qf => qf.Id == QEffectId.RangersCompanion && qf.Source == you)) && cr.Actions.CanTakeActions()));

                      if (animalCompanion == null)
                      {

                        return;
                      }


                      if (!you.Actions.ActionHistoryThisTurn.Any<CombatAction>((Func<CombatAction, bool>)(ac => ac.Name == "Command your Animal Companion" || ac.ActionId == ActionId.Delay)))
                      {
                        you.Occupies.Overhead("Mature Companion.", Color.Green);
                        animalCompanion.AddQEffect(new QEffect()
                        {
                          ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn,
                          StartOfYourTurn = (Func<QEffect, Creature, Task>)(async (effect, creature) =>
                                {
                                  creature.Actions.UseUpActions(1, ActionDisplayStyle.Summoned);
                                  return;
                                })
                        });
                        await CommonSpellEffects.YourMinionActs(animalCompanion);

                      }

                    })
                ));



    ModManager.AddFeat(BeastMasterDedicationFeat);
    ModManager.AddFeat(BeastMasterMatureFeat);
  }
}