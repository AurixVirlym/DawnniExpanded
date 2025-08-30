using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Creatures;
using System;




namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeDuelist
{

  public static Feat DuelistDedicationFeat;

  public static Feat DuelingParryFeat;

  public static Feat DuelistsChallengeFeat;
  public static void LoadMod()

  {

    DuelistDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "Across the world, students in martial academies practice with their blades to master one-on-one combat. The libraries of such schools hold deep troves of information detailing hundreds of combat techniques, battle stances, and honorable rules of engagement. Those who gain admission to such schools might train in formalized duels—and that's certainly the more genteel route to take. However, others assert that there's no better place to try out dueling techniques than in the life-and-death struggles common to an adventurer's life.",
            "You are always ready to draw your weapon and begin a duel, no matter the circumstances.\n\nYou gain the Quick Draw feat.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Duelist Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.LightArmor) >= Proficiency.Trained && values.GetProficiency(Trait.Simple) >= Proficiency.Trained, "You must be trained in light armor and simple weapons.")
            .WithOnSheet(sheet => sheet.GrantFeat(FeatName.QuickDraw));

    DuelingParryFeat = new TrueFeat(FeatName.CustomFeat,
            4,
            "You can parry attacks against you with your one-handed weapon",
            "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithOnCreature((sheet, creature) =>
  {

    QEffect DuellingParryHolder = new QEffect()
    {
      Name = "Dueling Parry Granter",
      ProvideMainAction = (qfTechnical =>
          {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return new ActionPossibility
          (new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"), "Dueling Parry", new Trait[] { Trait.Basic, DawnniExpanded.DETrait },
                    "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.",
                        Target.Self()
                    .WithAdditionalRestriction((a) =>
                    {
                      if (a.QEffects.Any((QEffect x) => x.Name == "Dueling Parry"))
                      {
                        return "Already parrying.";
                      };

                      if (a.HasFreeHand)
                      {

                        if (a.HeldItems.FirstOrDefault() == null)
                        {
                          return "Not holding a Melee Weapon";
                        }
                        if (!a.HeldItems.First().HasTrait(Trait.Melee))
                        {
                          return "Not holding a Melee Weapon";
                        }
                        else return null;

                      }
                      else return "No Free Hand.";


                    })
                    )
                    .WithSoundEffect(SfxName.RaiseShield)
                    .WithActionCost(1)
                    .WithEffectOnSelf(async (spell, caster) =>
                {
                  QEffect DuellingParryEffect = new QEffect()
                  {
                    Name = "Dueling Parry",
                    Owner = caster,
                    DoNotShowUpOverhead = true,
                    Description = "+2 circumstance bonus to AC until the start of your next turn.",
                    Illustration = new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"),
                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                                {
                                  if (defense == Defense.AC)
                                  {
                                    return new Bonus(2, BonusType.Circumstance, "Dueling Parry");
                                  }
                                  else return null;
                                },
                    ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn,
                    StateCheck = Qfduel =>
                              {

                                if (Qfduel.Owner.HeldItems.FirstOrDefault() == null)
                                {
                                  Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                }
                                else
                                if (!Qfduel.Owner.HasFreeHand || !Qfduel.Owner.HeldItems.First().HasTrait(Trait.Melee) || Qfduel.Owner.HasEffect(QEffectId.Unconscious) || Qfduel.Owner.HasEffect(QEffectId.Dying))
                                {
                                  Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                }

                              }
                  };
                  caster.AddQEffect(DuellingParryEffect);

                }));

          }
        )

    };

    creature.AddQEffect(DuellingParryHolder);
  })
            .WithCustomName("Dueling Parry (Duelist){icon:Action}")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DuelistDedicationFeat), "You must have the Duelist Dedidcation feat.")
            .WithEquivalent(values => values.AllFeats.Contains(FeatDuelingParry.FeatDuelingParryFighter));


    DuelistsChallengeFeat = new TrueFeat(FeatName.CustomFeat,
              4,
              "You can parry attacks against you with your one-handed weapon",
              "Select one foe that you can see and proclaim a challenge.\n\nThat foe is your dueling opponent until they are defeated, flee, or the encounter ends. \n\nAny time you hit that enemy using a single one-handed melee weapon while your other hand or hands are free, you gain a circumstance bonus to the Strike's damage equal to the number of damage dice your weapon deals.\n\nIf you attack a creature other than your dueling opponent, you take a circumstance penalty to damage equal to the number of damage dice your weapon deals.",
              new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, Trait.Open })
              .WithOnCreature((sheet, creature) =>
    {

      QEffect DuelistsChallengeHolder = new QEffect()
      {
        Name = "Duelist's Challenge",
        ProvideMainAction = (qfTechnical =>
          {

            return new ActionPossibility
        (new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"), "Duelist's Challenge", new Trait[] { Trait.Basic, DawnniExpanded.DETrait, Trait.Open },
                  "Select one foe that you can see and proclaim a challenge.\n\nThat foe is your dueling opponent until they are defeated or the encounter ends.\n\nAny time you hit that enemy using a single one-handed melee weapon while your other hand or hands are free, you gain a circumstance bonus to the Strike's damage equal to the number of damage dice your weapon deals.\n\nIf you attack a creature other than your dueling opponent, you take a circumstance penalty to damage equal to the number of damage dice your weapon deals.",
                      Target.Ranged(999)

                  )
                  .WithSoundEffect(SfxName.Intimidate)
                  .WithActionCost(1)
                  .WithEffectOnChosenTargets(async (spell, caster, Targets) =>
              {
                Creature target = Targets.ChosenCreature;

                QEffect DuellingParryEffect = new QEffect()
                {
                  Name = "Duelist's Challenge",
                  Owner = caster,
                  DoNotShowUpOverhead = true,
                  Description = "Challenging " + target.Name + ".\nYou gain a bonus to damage when targeting your challenged foe and a penalty when targeting anyone else.",
                  Illustration = new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"),
                  PreventTakingAction = newAttack => newAttack.Name != "Duelist's Challenge" ? null : "You are already challenging a foe.",
                  BonusToDamage = ((effect, action, defender) =>
                  {
                    if (!action.HasTrait(Trait.Strike)
                    || !action.HasTrait(Trait.Weapon)

                    || action.HasTrait(Trait.Unarmed))
                    {
                      return null;
                    }
                    String count = action.TrueDamageFormula.ToString();
                    if (count.Length >= 3)
                    {
                      int DmgDiceNumber = Int32.Parse(count.Substring(0, 1));

                      if (defender == target)
                      {
                        if (!effect.Owner.HasFreeHand || !action.HasTrait(Trait.Melee))
                        {
                          return null;
                        }

                        return new Bonus(DmgDiceNumber, BonusType.Circumstance, "Duelist's Challenge");
                      }
                      else
                      {
                        return new Bonus(-DmgDiceNumber, BonusType.Circumstance, "Duelist's Challenge");
                      }
                    }
                    else return null;

                  }),
                  ExpiresAt = ExpirationCondition.Never,
                  StateCheck = Qfduel =>
                  {

                    if (target.Alive == false)
                    {
                      Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                    }


                  }
                };
                caster.AddQEffect(DuellingParryEffect);

              }));

          }
        )

      };

      creature.AddQEffect(DuelistsChallengeHolder);
    })
              .WithCustomName("Duelist's Challenge{icon:Action}")
              .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DuelistDedicationFeat), "You must have the Duelist Dedidcation feat.");


    ModManager.AddFeat(DuelingParryFeat);
    ModManager.AddFeat(DuelistDedicationFeat);
    ModManager.AddFeat(DuelistsChallengeFeat);
  }
}