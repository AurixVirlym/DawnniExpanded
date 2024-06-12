using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using System;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using System.Threading.Tasks;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Text;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeWrestler
{

  public static Feat WrestlerDedicationFeat;
  public static Feat WrestlerCombatGrab;
  public static Feat WrestlerSnaggingStrike;
  public static Feat WrestlerCrushingGrab;
  public static Feat WrestlerSuplex;
  public static Feat WrestlerElbowBreaker;
  public static void LoadMod()

  {

    WrestlerDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "Wrestlers are athletes who pit their strength and skill against powerful foes. Specializing in a variety of grabs, holds, and strikes, wrestlers are dangerous opponents whose techniques can leave a foe broken and defeated without taking their life.",
            "You become an expert in Athletics. You gain a +2 circumstance bonus to your Fortitude DC when resisting an opponent's attempts to Grapple you or Swallow you Whole.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Wrestler Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Athletics) >= Proficiency.Trained, "You must be trained in Athletics.")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.UnarmoredDefense) >= Proficiency.Trained, "You must be trained in Unarmored Defense.")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Unarmed) >= Proficiency.Trained, "You must be trained in Unarmed Attacks.")
            .WithOnSheet(sheet => sheet.GrantFeat(FeatName.ExpertAthletics))
            .WithOnCreature(sheet => sheet.AddQEffect(new QEffect()
            {
              BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                        {
                          if (attack != null && defense == Defense.Fortitude && attack.HasTrait(Trait.Grab))
                          {
                            return new Bonus(2, BonusType.Circumstance, "Wrestler");
                          }
                          else return null;
                        }
            }));



    WrestlerCombatGrab = new TrueFeat(FeatName.CustomFeat,
            4,
            "You swipe at your opponent and grab at them.",
            "Make a melee Strike while keeping one hand free. If the Strike hits, you grab the target using your free hand.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, Trait.Press })
            .WithCustomName("Combat Grab (Wrestler){icon:Action}")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WrestlerDedicationFeat), "You must have the Wrestler Dedication feat")
            .WithEquivalent(values => values.AllFeatNames.Contains(FeatName.CombatGrab))
            .WithOnSheet((CalculatedCharacterSheetValues values) => values.GrantFeat(FeatName.CombatGrab));

    WrestlerSnaggingStrike = new TrueFeat(FeatName.CustomFeat,
           4,
           "You combine an attack with quick grappling moves to throw an enemy off balance as long as it stays in your reach.", "Make a Strike while keeping one hand free. If this Strike hits, the target is flat-footed until the start of your next turn or until it's no longer within the reach of your hand, whichever comes first.",
           new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
           .WithCustomName("Snagging Strike (Wrestler){icon:Action}")
           .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WrestlerDedicationFeat), "You must have the Wrestler Dedication feat")
           .WithEquivalent(values => values.AllFeatNames.Contains(FeatName.SnaggingStrike))
           .WithOnSheet((CalculatedCharacterSheetValues values) => values.GrantFeat(FeatName.SnaggingStrike));

    WrestlerCrushingGrab = new TrueFeat(FeatName.CustomFeat,
           4,
           "Like a powerful constrictor, you crush targets in your unyielding grasp.", "When you successfully Grapple a creature, you also deal bludgeoning damage to that creature equal to your Strength modifier.",
           new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
           .WithCustomName("Crushing Grab (Wrestler){icon:Action}")
           .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WrestlerDedicationFeat), "You must have the Wrestler Dedication feat")
           .WithEquivalent(values => values.AllFeatNames.Contains(FeatName.CrushingGrab))
           .WithOnSheet((CalculatedCharacterSheetValues values) => values.GrantFeat(FeatName.CrushingGrab));

    WrestlerSuplex = new TrueFeat(FeatName.CustomFeat,
           4,
           "Flexing your entire body, you heave your opponent over your head and slam them into the ground.", "Make an unarmed melee Strike against the creature you have grabbed or restrained; on a success, the target lands prone, and on a critical success, the target lands prone and takes an additional 2d6 bludgeoning damage. Regardless of whether the Strike is successful, you immediately release your hold on the target.",
           new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
           .WithCustomName("Suplex{icon:Action}")
           .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WrestlerDedicationFeat), "You must have the Wrestler Dedication feat")
           .WithPermanentQEffect(null, (Action<QEffect>)(qf => qf.ProvideStrikeModifier = (Func<Item, CombatAction>)(item =>
      {
        if (!item.HasTrait(Trait.Unarmed))
        {
          return null;
        }
        CombatAction strike = qf.Owner.CreateStrike(item);
        strike.Name = "Suplex";
        strike.Illustration = (Illustration)new SideBySideIllustration(strike.Illustration, (Illustration)IllustrationName.Grapple);
        strike.Description = StrikeRules.CreateBasicStrikeDescription(strike.StrikeModifiers, additionalSuccessText: "The target lands prone. and on a critical success, the target lands prone and takes an additional 2d6 bludgeoning damage.", additionalCriticalSuccessText: "the target lands prone and takes an additional 2d6 bludgeoning damage. Regardless of whether the Strike is successful", additionalAftertext: "Regardless of whether the Strike is successful, you immediately release your hold on the target.");

        strike.Traits.Add(Trait.Basic);

        strike.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.Ephemeral)
        {
          AddExtraStrikeDamage = (action, target) =>
                    {
                      if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Unarmed) || action.CheckResult != CheckResult.CriticalSuccess)
                      {
                        return null;
                      }
                      else return (DiceFormula.FromText("1d6", "Suplex"), DamageKind.Bludgeoning);
                    },
        };
        strike.StrikeModifiers.OnEachTarget += (Func<Creature, Creature, CheckResult, Task>)(async (caster, target, checkResult) =>
        {
          target.QEffects.FirstOrDefault(qfgrapple => qfgrapple.Id == QEffectId.Grappled && qfgrapple.Source == caster).ExpiresAt = ExpirationCondition.Immediately;
          caster.HeldItems.RemoveAll((Predicate<Item>)(hi => hi.Grapplee == target));

          if (checkResult >= CheckResult.Success)
            target.AddQEffect(QEffect.Prone());
        });
        ((CreatureTarget)strike.Target).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>)((grappler, target) => !target.QEffects.Any<QEffect>((Func<QEffect, bool>)(qf => qf.Id == QEffectId.Grappled && qf.Source == grappler)) ? Usability.NotUsableOnThisCreature("Target is not grappled by you") : Usability.Usable));
        return strike;
      })));


    WrestlerElbowBreaker = new TrueFeat(FeatName.CustomFeat,
         4,
         "You bend your opponent's body or limbs into agonizing positions that make it difficult for them to maintain their grip.", "Make an unarmed melee Strike against the creature you have grabbed or restrained. This Strike has the following effects in addition to its usual effects."
         + S.FourDegreesOfSuccess(
        "You knock one held item out of the creature's grasp. It falls to the ground in the creature's space.",
        "You weaken your opponent's grasp on one held item. Until the start of that creature's turn, attempts to Disarm the opponent of that item gain a +2 circumstance bonus, and the target takes a -2 circumstance penalty to attacks with the item or other checks requiring a firm grasp on the item.",
        null,
        null),
         new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
         .WithCustomName("Elbow Breaker{icon:Action}")
         .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WrestlerDedicationFeat), "You must have the Wrestler Dedication feat")
         .WithPermanentQEffect(null, (Action<QEffect>)(qf => qf.ProvideStrikeModifier = (Func<Item, CombatAction>)(item =>
    {
      if (!item.HasTrait(Trait.Unarmed))
      {
        return null;
      }
      CombatAction strike = qf.Owner.CreateStrike(item);
      strike.Name = "Elbow Breaker";
      strike.Illustration = (Illustration)new SideBySideIllustration(strike.Illustration, (Illustration)IllustrationName.Grapple);
      strike.Description = StrikeRules.CreateBasicStrikeDescription(strike.StrikeModifiers,
      additionalSuccessText: "You knock one held item out of the creature's grasp. It falls to the ground in the creature's space.",
      additionalCriticalSuccessText: "You knock one held item out of the creature's grasp. It falls to the ground in the creature's space.",
      additionalAftertext: "Regardless of whether the Strike is successful, you immediately release your hold on the target.");


      strike.Traits.Add(Trait.Basic);


      strike.StrikeModifiers.OnEachTarget += (Func<Creature, Creature, CheckResult, Task>)(async (caster, target, checkResult) =>
      {
        if (checkResult >= CheckResult.Success)
        {
          Item disarmItem = target.HeldItems.First<Item>((Func<Item, bool>)(hi => !hi.HasTrait(Trait.Grapplee)));
          if (target.HeldItems.Count<Item>((Func<Item, bool>)(hi => !hi.HasTrait(Trait.Grapplee))) >= 2)
            disarmItem = await caster.Battle.AskForConfirmation(caster, (Illustration)IllustrationName.GenericCombatManeuver, "Which item would you like to disarm your target of?", target.HeldItems[0].Name, target.HeldItems[1].Name) ? target.HeldItems[0] : target.HeldItems[1];
          if (checkResult == CheckResult.CriticalSuccess)
          {
            target.HeldItems.Remove(disarmItem);
            target.Occupies.DropItem(disarmItem);
          }
          else
            target.AddQEffect(new QEffect("Weakened grasp", "Attempts to disarm you gain a +2 circumstance bonus, and your attacks with this item take a -2 circumstance penalty.", ExpirationCondition.ExpiresAtStartOfYourTurn, caster, (Illustration)IllustrationName.GenericCombatManeuver)
            {
              Key = "Weakened grasp",
              BonusToAttackRolls = (Func<QEffect, CombatAction, Creature, Bonus>)((qf, ca, cr) => ca.Item == disarmItem ? new Bonus(-2, BonusType.Circumstance, "Weakened grasp (Disarm)") : (Bonus)null),
              StateCheck = (Action<QEffect>)(qf => qf.Owner.Battle.AllCreatures.ForEach((Action<Creature>)(cr => cr.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
              {
                BonusToAttackRolls = (Func<QEffect, CombatAction, Creature, Bonus>)((qff, caa, crr) => crr == target && caa.ActionId == ActionId.Disarm ? new Bonus(2, BonusType.Circumstance, "Weakened grasp (Disarm)") : (Bonus)null)
              }))))
            });
        }
      });
      ((CreatureTarget)strike.Target).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>)((grappler, target) => !target.QEffects.Any<QEffect>((Func<QEffect, bool>)(qf => qf.Id == QEffectId.Grappled && qf.Source == grappler)) ? Usability.NotUsableOnThisCreature("Target is not grappled by you") : Usability.Usable));
      return strike;
    })));


    ModManager.AddFeat(WrestlerDedicationFeat);
    ModManager.AddFeat(WrestlerCombatGrab);
    ModManager.AddFeat(WrestlerSnaggingStrike);
    ModManager.AddFeat(WrestlerCrushingGrab);
    ModManager.AddFeat(WrestlerSuplex);
    ModManager.AddFeat(WrestlerElbowBreaker);
  }
}