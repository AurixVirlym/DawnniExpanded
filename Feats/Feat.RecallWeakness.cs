using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using System.Linq;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Possibilities;
using System;
using System.Collections.Generic;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using System.Threading.Tasks;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class FeatRecallWeakness
{

  public static Feat IndepthWeakness;

  public static Feat SlightestGlanceWeakness;

  public static Feat CombatAssessment;

  public static CalculatedNumber.CalculatedNumberProducer LoreCheck(Trait trait, Skill skill)
  {
    return delegate (CombatAction combatAction, Creature self, Creature? target)
    {
      List<Bonus> list = new List<Bonus>();
      foreach (QEffect qEffect in self.QEffects)
      {
        list.Add(qEffect.BonusToAllChecksAndDCs?.Invoke(qEffect));
        list.Add(qEffect.BonusToAbilityBasedChecksRollsAndDCs?.Invoke(qEffect, Ability.Intelligence));
        list.Add(qEffect.BonusToAttackRolls?.Invoke(qEffect, combatAction, target));
        list.Add(qEffect.BonusToSkills?.Invoke(skill));
        list.Add(qEffect.BonusToSkillChecks?.Invoke(skill, combatAction, target));
      }

      return new CalculatedNumber((int)self.Proficiencies.Get(trait) + self.ProficiencyLevel + self.Abilities.Intelligence, "Lore Skill", list);
    };
  }

  public static CombatAction RecallWeaknessAction(Creature self)
  {
    var RecallWeaknessTargets = Target.Ranged(6);

    if (self.PersistentCharacterSheet.Calculated.AllFeats.Contains<Feat>(SlightestGlanceWeakness))
    {
      RecallWeaknessTargets = Target.Ranged(12);

      if (self.Proficiencies.Get(Trait.Perception) >= Proficiency.Master)
      {
        RecallWeaknessTargets = Target.Ranged(24);
      }

    }

    return new CombatAction(self, IllustrationName.Action, "Recall Weakness", new Trait[] { Trait.Basic, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait, Trait.Skill },
                              "You attempt to recall a weakness of a foe to use to your advantage. Attempt a skill check against a foe within 30ft using a skill relevant to a creature's type (see table below) with a level based DC on the foe's level." + S.FourDegreesOfSuccess(
        "The creatures gains a -2 circumstance penalty to the next saving throw check it attempts against your allies before the end of your next turn.",
        "As critical success except the penalty is reduced to -1.",
        null,
        "You falsely recall information about the creature, granting it a + 1 circumstance bonus to the next saving throw check it attempts against your allies before the end of your next turn.") + "\n\nThe foe becomes immune to your Recall Weakness for the rest of the encounter regardless of the result." +
        "\n\n{b}Arcana{/b} Constructs, Beasts and Elementals.\n{b}Crafting{/b} Constructs.\n{b}Nature{/b} Animals, Beasts, Elementals, Fey and Plants.\n{b}Occultism{/b} Aberrations, Spirits and Oozes.\n{b}Religion{/b} Celestials, Fiends and Undead.\n{b}Society{/b} Humanoid.\n\nModder's Note: This action is a homebrew replacement for the Recall Knowledge action.",
                                  RecallWeaknessTargets)
                                            .WithActionCost(1)
                                            .WithActiveRollSpecification(
                                              new ActiveRollSpecification(
                                                KnowledgeRoll(),
                                                LevelBasedDC()))
                              .WithSoundEffect(SfxName.OpenPage)
                              .WithActionId(ActionID)
                              .WithActionCost(1)
                              .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (action, caster, target, checkResult) =>
    {
      int num;

      target.AddQEffect(QEffect.ImmunityToTargeting(ActionID, caster));

      switch (checkResult)
      {
        case CheckResult.CriticalFailure:
          num = +1;
          break;
        case CheckResult.Success:
          num = -1;
          break;
        case CheckResult.CriticalSuccess:
          num = -2;
          break;
        default:
          num = 0;
          break;
      }

      if (checkResult == CheckResult.Failure)
        return;

      bool IndepthCheck = false;

      if (caster.PersistentCharacterSheet.Calculated.AllFeats.Contains<Feat>(IndepthWeakness))
      {
        IndepthCheck = true;
      }
      target.AddQEffect(RecallWeaknessEffect(num, caster, IndepthCheck));

    }));
  }

  public static ActionId ActionID = ModManager.RegisterEnumMember<ActionId>("RecallWeaknessActionID");

  public static ActionId CombatAssessmentActionID = ModManager.RegisterEnumMember<ActionId>("Combat Assessment");

  public static CalculatedNumber.CalculatedNumberProducer LevelBasedDC()
  {
    return delegate (CombatAction action, Creature effectsource, Creature? roller)
    {
      int baseValue = DCs.LevelBased(roller.Level);

      return new CalculatedNumber(baseValue, "Level " + roller.Level.ToString() + " DC", new List<Bonus>());
    };
  }


  public static List<Trait> KnowledgeSkillsNeeded(Creature self, Creature target)
  {

    List<Trait> UsableSkills = new List<Trait>();

    //Arcana
    if (target.Traits.Contains(Trait.Construct)
    || target.Traits.Contains(Trait.Beast)
    || target.Traits.Contains(Trait.Elemental)
    || target.Traits.Contains(Trait.Arcana)
    || target.Traits.Contains(Trait.Dragon)
    )
    {
      UsableSkills.Add(Trait.Arcana);
    }

    //Crafting
    if (target.Traits.Contains(Trait.Construct))
    {
      UsableSkills.Add(Trait.Crafting);
    }

    //Nature
    if (target.Traits.Contains(Trait.Animal)
    || target.Traits.Contains(Trait.Beast)
    || target.Traits.Contains(Trait.Elemental)
    || target.Traits.Contains(Trait.Primal)
    // || target.Traits.Contains(Trait.Fey)
    || target.Traits.Contains(Trait.Plant)
    || target.Traits.Contains(Trait.Leshy)
    )
    {
      UsableSkills.Add(Trait.Nature);
    }

    //Occultism
    if (target.Traits.Contains(Trait.Aberration)
      // ||target.Traits.Contains(Trait.Ooze)
      || target.Traits.Contains(Trait.Occult)

      )
    {
      UsableSkills.Add(Trait.Occultism);
    }

    //Religion
    if (target.Traits.Contains(Trait.Undead)
      || target.Traits.Contains(Trait.Fiend)
      || target.Traits.Contains(Trait.Demon)
      || target.Traits.Contains(Trait.Divine)
      || target.Traits.Contains(Trait.Celestial)
      || target.Traits.Contains(Trait.Starborn)
      )
    {
      UsableSkills.Add(Trait.Religion);
    }

    //Society
    if (target.Traits.Contains(Trait.Human)
      || target.Traits.Contains(Trait.Humanoid)
      || target.Traits.Contains(Trait.Orc)
      || target.Traits.Contains(Trait.Kobold)
      || target.Traits.Contains(Trait.Merfolk)
      )
    {
      UsableSkills.Add(Trait.Society);
    }

    if (self.Proficiencies.Get(NewSkills.BardicLoreSkillTrait) >= Proficiency.Trained)
    {
      UsableSkills.Add(NewSkills.BardicLoreSkillTrait);
    }

    if (UsableSkills.Count() == 0)
    {
      UsableSkills.Add(Trait.Society);
    }


    return UsableSkills;

  }

  public static CalculatedNumber.CalculatedNumberProducer KnowledgeRoll()
  {
    return delegate (CombatAction action, Creature caster, Creature? target)
    {

      List<CalculatedNumber.CalculatedNumberProducer> bestAmongSkills = new List<CalculatedNumber.CalculatedNumberProducer>();

      //Arcana
      if (target.Traits.Contains(Trait.Construct)
      || target.Traits.Contains(Trait.Beast)
      || target.Traits.Contains(Trait.Elemental)
      || target.Traits.Contains(Trait.Arcana)
      || target.Traits.Contains(Trait.Dragon)
      )
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Arcana));
      }

      //Crafting
      if (target.Traits.Contains(Trait.Construct))
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Crafting));
      }

      //Nature
      if (target.Traits.Contains(Trait.Animal)
      || target.Traits.Contains(Trait.Beast)
      || target.Traits.Contains(Trait.Elemental)
      || target.Traits.Contains(Trait.Primal)
      //|| target.Traits.Contains(Trait.Fey)
      || target.Traits.Contains(Trait.Plant)
      || target.Traits.Contains(Trait.Leshy)
      )
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Nature));
      }

      //Occultism
      if (target.Traits.Contains(Trait.Aberration)
        //|| target.Traits.Contains(Trait.Ooze)
        || target.Traits.Contains(Trait.Occult)

        )
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Occultism));
      }

      //Religion
      if (target.Traits.Contains(Trait.Undead)
        || target.Traits.Contains(Trait.Fiend)
        || target.Traits.Contains(Trait.Demon)
        || target.Traits.Contains(Trait.Divine)
        || target.Traits.Contains(Trait.Celestial)
        || target.Traits.Contains(Trait.Starborn)
        )
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Religion));
      }

      //Society
      if (target.Traits.Contains(Trait.Human)
        || target.Traits.Contains(Trait.Humanoid)
        || target.Traits.Contains(Trait.Orc)
        || target.Traits.Contains(Trait.Kobold)
        || target.Traits.Contains(Trait.Merfolk)
        )
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Society));
      }

      if (caster.Proficiencies.Get(NewSkills.BardicLoreSkillTrait) >= Proficiency.Trained)
      {
        bestAmongSkills.Add(LoreCheck(NewSkills.BardicLoreSkillTrait, NewSkills.BardicLoreSkill));
      }

      if (bestAmongSkills.Count() == 0)
      {
        bestAmongSkills.Add(Checks.SkillCheck(Skill.Society));
      }


      CombatAction combatAction2 = action;
      Creature self2 = caster;
      Creature? target2 = target;


      return bestAmongSkills.MaxBy((CalculatedNumber.CalculatedNumberProducer producer) => producer(combatAction2, self2, target2).TotalNumber)(combatAction2, self2, target2);

    };
  }

  public static QEffect RecallWeaknessEffect(int EffectValue, Creature User, bool hasIndepth = false)

  {
    int ModifierValue = EffectValue;
    return new QEffect()

    {
      CountsAsADebuff = true,
      Source = User,
      Illustration = IllustrationName.Action,
      Name = "Recall Weakness " + ModifierValue,
      Description = "The creature is taking a " + EffectValue + " circumstance bonus to it's next saving throw.",
      CannotExpireThisTurn = true,
      ExpiresAt = ExpirationCondition.ExpiresAtEndOfSourcesTurn,
      BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                          {
                            if (defense == Defense.Will || defense == Defense.Reflex || defense == Defense.Fortitude)
                            {
                              return new Bonus(ModifierValue, BonusType.Circumstance, "Recall Weakness");
                            }
                            else return null;
                          },
      BeforeYourSavingThrow = async (effect, action, you) =>
               {
                 if (hasIndepth == false || EffectValue == 1)
                 {
                   effect.CannotExpireThisTurn = false;
                   effect.ExpiresAt = ExpirationCondition.Immediately;
                 }
                 return;
               },

    };
  }
  public static void LoadMod()

  {

    IndepthWeakness = new TrueFeat(FeatName.CustomFeat,
                    2,
                    "Your knowledge of enemy weaknesses runs deeper than most.",
                    "Whenever you use the Recall Weakness action, your Critical Success and Success are not removed after the foe makes a saving throw check.",
                    new Trait[] { Trait.General, Trait.SkillFeat, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait })
                    .WithPrerequisite((values) =>
                    values.GetProficiency(Trait.Arcana) >= Proficiency.Expert
                    || values.GetProficiency(Trait.Crafting) >= Proficiency.Expert
                    || values.GetProficiency(Trait.Nature) >= Proficiency.Expert
                    || values.GetProficiency(Trait.Occultism) >= Proficiency.Expert
                    || values.GetProficiency(Trait.Religion) >= Proficiency.Expert
                    || values.GetProficiency(Trait.Society) >= Proficiency.Expert
                    || values.GetProficiency(NewSkills.BardicLoreSkillTrait) >= Proficiency.Trained
                    , "You must be expert in Arcane, Crafting, Nature, Occultism, Religion or Society or be trained in Bardic Lore")
                   .WithCustomName("In-depth Weakness");

    SlightestGlanceWeakness = new TrueFeat(FeatName.CustomFeat,
                    2,
                    "You only need a small glismpe of your foe to understand their weakness.",
                    "Whenever you use the Recall Weakness action, you may target foes within 60ft instead of 30ft.\n\nIf you are a master of perception, you may use Recall Weakness on foes within 120ft.",
                    new Trait[] { Trait.General, Trait.SkillFeat, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait })
                    .WithPrerequisite((values) =>
                    values.GetProficiency(Trait.Perception) >= Proficiency.Expert
                    , "You must be expert in Perception.")
                   .WithCustomName("Slightest Glance Weakness");

    CombatAssessment = new TrueFeat(FeatName.CustomFeat, 1, "You make a telegraphed attack to learn about your foe.", "Make a melee Strike. On a hit, you can immediately attempt a check to Recall Weakness about the target. On a critical hit, you gain a +2 circumstance bonus to the check to Recall Weakness.\n\nThe target is temporarily immune to Combat Assessment for 1 day."
, new Trait[1]
{
        Trait.Fighter
}).WithActionCost(1).WithCustomName("Combat Assessment").WithPermanentQEffect("If you hit, you Recall Weakness against the target", (Action<QEffect>)(qf => qf.ProvideStrikeModifier = (Func<Item, CombatAction>)(item =>
{
  CombatAction strike = qf.Owner.CreateStrike(item);
  strike.Illustration = (Illustration)new SideBySideIllustration(strike.Illustration, (Illustration)IllustrationName.Action);
  strike.Name = "Combat Assessment " + strike.Name;
  strike.Traits.Add(Trait.Basic);
  strike.ActionId = CombatAssessmentActionID;
  strike.Description = StrikeRules.CreateBasicStrikeDescription(strike.StrikeModifiers, additionalSuccessText: "Recall Weakness against the target", additionalCriticalSuccessText: "Gain a +2 circumstance bonus to the check to Recall Weakness.", additionalAftertext: "The target is temporarily immune to Combat Assessment for 1 day.");

  strike.StrikeModifiers.OnEachTarget += (Func<Creature, Creature, CheckResult, Task>)(async (caster, target, checkResult) =>
  {


    target.AddQEffect(QEffect.ImmunityToTargeting(CombatAssessmentActionID, caster));

    if (checkResult < CheckResult.Success)
      return;

    if (target.Alive == false)
      return;

    if (checkResult == CheckResult.CriticalSuccess)
    {
      strike.Owner.AddQEffect(new QEffect("Combat Assessment (Critical Success)", null, ExpirationCondition.Ephemeral, null)
      {
        BonusToSkillChecks = (skill, action, target) => action.ActionId == FeatRecallWeakness.ActionID ? new Bonus(2, BonusType.Circumstance, "Combat Assessment (Critical Success)") : (Bonus)null,
      });
    }


    TBattle battle = strike.Owner.Battle;


    await battle.GameLoop.FullCast(RecallWeaknessAction(strike.Owner), new ChosenTargets
    {
      ChosenCreature = target
    });

  });

  ((CreatureTarget)strike.Target).WithAdditionalConditionOnTargetCreature((CreatureTargetingRequirement)new AdjacencyCreatureTargetingRequirement());
  return strike;
})));


    ModManager.RegisterActionOnEachCreature(creature =>
        {
          // We add an effect to every single creature...

          if (creature.PersistentCharacterSheet != null)
          {
            creature.AddQEffect(
                new QEffect()
                {

                  Name = "Recall Weakness Granter",
                  ProvideActionIntoPossibilitySection = (qfself, possibilitySection1) =>
                                        {
                                          if (possibilitySection1.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                                          {
                                            return null;
                                          }



                                          return new ActionPossibility
                            (RecallWeaknessAction(creature));

                                        }

                });
          }
        });


    ModManager.AddFeat(IndepthWeakness);
    ModManager.AddFeat(SlightestGlanceWeakness);
    ModManager.AddFeat(CombatAssessment);
  }
}