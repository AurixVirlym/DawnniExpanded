using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Treasure;

using Microsoft.Xna.Framework;


namespace Dawnsbury.Mods.DawnniExpanded
{
  public class MonsterBadger
  {

    public static QEffectId CivieQFId = ModManager.RegisterEnumMember<QEffectId>("CivieId");

    public static QEffectId CommandCivieQFId = ModManager.RegisterEnumMember<QEffectId>("CommandCivieId");

    public static Trait PushTrait = ModManager.RegisterTrait(
          "Push",
          new TraitProperties("Push", false, "", true)
          {
          });

    public static QEffect NewCivilain()
    {
      QEffect QFCivie = new QEffect("Civilian", "You can command the civilian to take certain actions with a skill check.")
      {
        Id = CivieQFId,
        EndOfYourTurn = async delegate (QEffect qfCivilian, Creature civilian)
        {
          civilian.AI.Tactic = Tactic.DoNothing;
          civilian.OwningFaction = civilian.Battle.GaiaFriends;
        },
        StateCheck = async delegate (QEffect qf)
        {
          Creature leshy = qf.Owner;

          foreach (Creature item in qf.Owner.Battle.AllCreatures.Where((Creature cr) => cr.OwningFaction.IsHumanControlled && cr != leshy && !cr.HasEffect(CommandCivieQFId)))
          {
            QEffect PlayerActionsQF = new QEffect(ExpirationCondition.Ephemeral)
            {
              Id = CommandCivieQFId,
              ProvideActionIntoPossibilitySection = delegate (QEffect qfAlly, PossibilitySection section)
                  {
                    if (section.PossibilitySectionId != PossibilitySectionId.ContextualActions)
                    {
                      return null;
                    }

                    Creature actionowner = item;

                    return new SubmenuPossibility(leshy.Illustration, "Direct a Civilian")
                    {
                      Subsections =
                            {
                                    new PossibilitySection("Direct a Civilian")
                                    {
                                        Possibilities =
                                        {
                                            (Possibility)new ActionPossibility(new CombatAction(actionowner, leshy.Illustration, "Flee!", new Trait[2]
                                            {
                                                Trait.Auditory,
                                                Trait.Basic
                                            }, "Make a Nature, Diplomacy or Intimidation check against DC 5.\n\n{b}Success{/b} The civilian will flee the nearest enemy as best it can during its next turn.",
                                            Target.RangedFriend(30)
                                            .WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>) ((self, target) => !target.HasEffect(CivieQFId) || target.HasEffect(QEffectId.Unconscious)
                                            ? Usability.NotUsableOnThisCreature("Not a civilian or unconscious") : Usability.Usable)))
                                            .WithActionCost(1)
                                            .WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Nature, Skill.Diplomacy, Skill.Intimidation), Checks.FlatDC(5)))
                                            .WithEffectOnEachTarget(async delegate(CombatAction spell, Creature caster, Creature target, CheckResult result)

                                            {
                                                if (result >= CheckResult.Success)
                                                {

                                                    target.AddQEffect(new QEffect("Fleeing", "You must spend all actions to move away from the nearest enemy as best possible.", ExpirationCondition.ExpiresAtEndOfYourTurn, null, IllustrationName.Fleeing)
                                                    {
                                                        Id = QEffectId.FleeingAllDanger,
                                                    });
                                                    target.Occupies.Overhead("I will flee!", Color.Black, target?.ToString() + " will spend its next turn fleeing the nearest enemy.");
                                                }
                                            })),
                                            (Possibility)new ActionPossibility(new CombatAction(actionowner, leshy.Illustration, "Attack!", new Trait[2]
                                            {
                                                Trait.Auditory,
                                                Trait.Basic
                                            }, "Make a Nature, Diplomacy or Intimidation check against DC 10.\n\n{b}Success{/b} The civilian will attack your enemies as best it can during its next turn.", Target.RangedFriend(30).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>) ((self, target) => !target.HasEffect(CivieQFId) || target.HasEffect(QEffectId.Unconscious) ? Usability.NotUsableOnThisCreature("Not a civilian or unconscious") : Usability.Usable))).WithActionCost(1).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Nature, Skill.Diplomacy, Skill.Intimidation), Checks.FlatDC(10))).WithEffectOnEachTarget(async delegate(CombatAction spell, Creature caster, Creature target, CheckResult result)
                                            {
                                                if (result >= CheckResult.Success)
                                                {
                                                    target.WithTactics(Tactic.Standard);
                                                    target.RemoveAllQEffects((QEffect qf) => qf.Id == QEffectId.Fleeing);
                                                    target.Occupies.Overhead("I will fight!", Color.Black, target?.ToString() + " will spend its next turn taking actions as normal.");
                                                }
                                            })),
                                            (Possibility)new ActionPossibility(new CombatAction(actionowner, leshy.Illustration, "Do exactly as I say!", new Trait[2]
                                            {
                                                Trait.Auditory,
                                                Trait.Basic
                                            }, "Make a Nature, Diplomacy or Intimidation check against DC 13.\n\n{b}Success{/b} You will assume direct control of the civilian during their next turn, choosing how the civilians spends its three actions.", Target.RangedFriend(30).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>) ((self, target) => !target.HasEffect(CivieQFId) || target.HasEffect(QEffectId.Unconscious) ? Usability.NotUsableOnThisCreature("Not a civilian or unconscious") : Usability.Usable))).WithActionCost(1).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Nature, Skill.Diplomacy, Skill.Intimidation), Checks.FlatDC(13))).WithEffectOnEachTarget(async delegate(CombatAction spell, Creature caster, Creature target, CheckResult result)
                                            {
                                                if (result >= CheckResult.Success)
                                                {
                                                    target.OwningFaction = target.Battle.You;
                                                    target.RemoveAllQEffects((QEffect qf) => qf.Id == QEffectId.Fleeing);
                                                    target.Occupies.Overhead("I will do as you say!", Color.Black, "You will control " + target?.ToString() + " during her next turn.");
                                                }
                                            }))
                                        }
                                    }
                            },
                      PossibilityGroup = "Interactions"
                    };
                  }
            };

            item.AddQEffect(PlayerActionsQF);
          }
        }
      };

      return QFCivie;

    }

    public static QEffect MonsterPush()
    {
      return new QEffect("Push", "When your Strike hits, you can spend an action to push a target 5ft without making a check.", ExpirationCondition.Never, null, IllustrationName.None)
      {
        Innate = true,
        ProvideMainAction = delegate (QEffect qfPush)
        {
          Creature zombie = qfPush.Owner;
          IEnumerable<Creature> source = from cr in zombie.Battle.AllCreatures.Where(delegate (Creature cr)
                  {
                    CombatAction combatAction = zombie.Actions.ActionHistoryThisTurn.LastOrDefault();
                    return combatAction != null && combatAction.CheckResult >= CheckResult.Success && combatAction.HasTrait(PushTrait) && combatAction.ChosenTargets.ChosenCreature == cr;
                  })
                                         select cr;
          return new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Push")
          {
            Subsections =
                {
                        new PossibilitySection("Push")
                        {
                            Possibilities = source.Select((Func<Creature, Possibility>)((Creature lt) =>
                            new ActionPossibility(new CombatAction(zombie, IllustrationName.GenericCombatManeuver, "Push " + lt.Name,
                             new Trait[1] { Trait.Melee },
                              "Push the target.", Target.Melee((Target t, Creature a, Creature d) => (!d.HasEffect(QEffectId.Unconscious)) && !d.IsFlatFootedTo(a,t.OwnerAction) && a.Actions.ActionsLeft == 1 ? 1.0737418E+09f : (-2.1474836E+09f))
                              .WithAdditionalConditionOnTargetCreature((Creature a, Creature d) => (d != lt) ? Usability.CommonReasons.TargetIsNotPossibleForComplexReason : Usability.Usable)).
                              WithEffectOnEachTarget(async delegate(CombatAction ca, Creature a, Creature d, CheckResult cr)
                            {

                              await a.PushCreature(d,1);
                            })))).ToList()
                        }
                }
          };
        }
      };
    }

    public static Creature CreateSmallBadger()
    {
      Creature SmallBadger = new Creature(new ModdedIllustration("DawnniburyExpandedAssets/SmallBadger.png"),
                      "Badger",
                      new List<Trait> { Trait.Animal, Trait.Neutral, Trait.Small },
                      0, 6, 5,
                      new Defenses(16, 8, 6, 6),
                      16,
                      new Abilities(0, 1, 2, -4, +2, -2),
                      new Skills(athletics: 4, stealth: 6))
                  .AddQEffect(QEffect.Ferocity())
                  .WithProficiency(Trait.Unarmed, Proficiency.Legendary)
                  .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d8", DamageKind.Piercing))
                  .WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d6", DamageKind.Slashing, Trait.Agile));
      return SmallBadger;
    }

    public static Illustration JojoBadger = new ModdedIllustration("DawnniburyExpandedAssets/JojoBadger.png");
    public static void LoadMod()
    {
      ModManager.RegisterNewCreature("Badger", (encounter) =>
      {

        return CreateSmallBadger();

      });

      ModManager.RegisterNewCreature("Mountain Goat", (encounter) =>
      {

        Creature MountainGoat = new Creature(new ModdedIllustration("DawnniburyExpandedAssets/MountainGoat.png"),
                   "Mountain Goat",
                   new List<Trait> { Trait.Animal, Trait.Neutral },
                   0, 6, 4,
                   new Defenses(14, 7, 4, 4),
                   16,
                   new Abilities(3, 2, 3, -4, 2, 0),
                   new Skills(athletics: 7, acrobatics: 4))
               .WithProficiency(Trait.Unarmed, Proficiency.Trained)
               .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "horn", "1d4", DamageKind.Bludgeoning, PushTrait))
               .AddQEffect(MonsterPush())
               .WithTactics(Tactic.Mindless);

        return MountainGoat;

      });

      ModManager.RegisterNewCreature("Guard Dog", (encounter) =>
      {

        Creature GuardDog = new Creature(IllustrationName.Wolf256,
                   "Guard Dog",
                   new List<Trait> { Trait.Animal, Trait.Neutral, Trait.Small },
                   -1, 6, 6,
                   new Defenses(15, 5, 7, 4),
                   8,
                   new Abilities(1, 2, 2, -4, 1, -1),
                   new Skills(athletics: 5, acrobatics: 4, stealth: 5, survival: 4))
               .WithProficiency(Trait.Unarmed, Proficiency.Master)
               .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d4", DamageKind.Piercing))
               .WithTactics(Tactic.PackAttack)
               .AddQEffect(new QEffect("Pack Attack", "This dog deals " + "1d4" + " extra damage to creatures within reach of at least two of the dogs's alies.")
               {
                 YouDealDamageWithStrike = ((QEffect qEffect, CombatAction yourStrike, DiceFormula diceFormula, Creature target) => target.Occupies.Neighbours.Creatures.Count<Creature>((Func<Creature, bool>)(cr => cr.FriendOf(qEffect.Owner))) >= 3 ? diceFormula.Add(DiceFormula.FromText("1d4", "Pack Attack")) : diceFormula)
               });


        return GuardDog;

      });

      ModManager.RegisterNewCreature("Ball Python", (encounter) =>
      {

        Creature BallPython = new Creature(new ModdedIllustration("DawnniburyExpandedAssets/HugSnake.png"),
                   "Ball Python",
                   new List<Trait> { Trait.Animal, Trait.Neutral },
                   1, 4, 7,
                   new Defenses(16, 8, 10, 4),
                   20,
                   new Abilities(3, 3, 3, -4, 1, -2),
                   new Skills(athletics: 6, acrobatics: 6, stealth: 6, survival: 4))
               .WithProficiency(Trait.Unarmed, Proficiency.Expert)
                .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d8", DamageKind.Piercing, Trait.Grab))
               .AddQEffect(QEffect.MonsterGrab())
               .AddQEffect(QEffect.Constrict("1d8", 17))
               .WithTactics(Tactic.Mindless);

        return BallPython;

      });

      ModManager.RegisterNewCreature("Commoner", (encounter) =>
      {

        Creature Commoner = new Creature(new ModdedIllustration("DawnniburyExpandedAssets/Commoner.png"),
                   "Commoner",
                   new List<Trait> { Trait.Human, Trait.Good, Trait.Humanoid },
                   -1, 3, 5,
                   new Defenses(13, 6, 3, 3),
                   10,
                   new Abilities(3, 1, 2, 0, 1, 0),
                   new Skills(athletics: 5))
        {
          SpawnAsFriends = true
        }
               .WithProficiency(Trait.Unarmed, (Proficiency)3)
               .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "fist", "1d4", DamageKind.Bludgeoning, Trait.Agile))
               .AddHeldItem(Items.CreateNew(ItemName.Sickle))
               .AddQEffect(NewCivilain())
               .WithTactics(Tactic.Standard)
               .WithProficiency(Trait.Weapon, (Proficiency)3)
               .WithEntersInitiativeOrder(true);



        return Commoner;

      });



      ModManager.RegisterNewCreature("Giant Badger", (encounter) =>
     {
       Creature GiantBadger = new Creature(new ModdedIllustration("DawnniburyExpandedAssets/SwolBadger.png"),
                  "Giant Badger",
                  new List<Trait> { Trait.Animal, Trait.Neutral },
                  2, 8, 5,
                  new Defenses(18, 10, 6, 8),
                  30,
                  new Abilities(4, 1, 3, -4, 3, -1),
                  new Skills(athletics: 8, stealth: 7))
              .AddQEffect(QEffect.Ferocity())
              .WithProficiency(Trait.Unarmed, (Proficiency)5)
              .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d8", DamageKind.Piercing))
              .WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d6", DamageKind.Slashing, Trait.Agile))
              .AddQEffect(new QEffect()
              {

                Innate = true,
                Name = "Enter Badger Rage",
                DoNotShowUpOverhead = true,
                Description = "Badger gains +4 to damage and reduces their AC by 1. Evil.",
                ProvideMainAction = (qfBadgerRage => (Possibility)(ActionPossibility)
                   new CombatAction(qfBadgerRage.Owner, (Illustration)IllustrationName.Rage, "Enter Badger Rage", new Trait[4]
                   {
          Trait.Concentrate,
          Trait.Emotion,
          Trait.Mental,
          Trait.Basic
      }, "Badger gains +4 to damage and reduces their AC by 1. Evil.", Target.Self((Func<Creature, AI, float>)((cr, ai) =>
      {
        if (cr.Actions.ActionsLeft >= 2 &&
          cr.Battle.AllCreatures.Any<Creature>(enemy =>
          enemy.EnemyOf(cr)
          && cr.IsAdjacentTo(enemy))
          && cr.HP != cr.MaxHP && cr.QEffects.All(qf => qf.Name != "Badger Rage")
          )
        {
          return (float)int.MaxValue;
        }
        else return int.MinValue;

      })))
                   .WithActionCost(1)
                   .WithEffectOnSelf(async (spell, caster) =>
                                       {
                                         QEffect BadgerRage = new QEffect()
                                         {
                                           Name = "Badger Rage",
                                           DoNotShowUpOverhead = true,
                                           Description = "Badger gains +4 to damage and reduces their AC by 1. Evil.",
                                           Illustration = IllustrationName.Rage,
                                           BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                                            {
                                              if (defense == Defense.AC)
                                              {
                                                return new Bonus(-1, BonusType.Untyped, "Badger Rage");
                                              }
                                              else return null;
                                            },

                                           BonusToDamage = (QEffect qf, CombatAction attack, Creature target) =>
                                          {
                                            return new Bonus(4, BonusType.Untyped, "Badger Rage");
                                          },

                                           ExpiresAt = ExpirationCondition.Never,
                                           StateCheck = Qfrage =>
                                          {
                                            Qfrage.Owner.Illustration = JojoBadger;
                                            if (Qfrage.Owner.HasEffect(QEffectId.CalmEmotions))
                                            {
                                              Qfrage.ExpiresAt = ExpirationCondition.Immediately;

                                            }

                                          }
                                         };
                                         caster.AddQEffect(BadgerRage);
                                         caster.WithTactics(Tactic.Mindless);
                                         caster.AddQEffect(new QEffect("Immunity to Badger Rage", "[this condition has no description]", ExpirationCondition.Never, caster, IllustrationName.None)
                                         {
                                           PreventTargetingBy = (CombatAction action) => (action.Name != spell.Name) ? null : "immunity"
                                         }
                                           );
                                       })

      ),


              });

       return GiantBadger;
     });

      ModManager.RegisterNewCreature("Satyr", (encounter) => { return GenerateSatyr(); });
    }

    public static Creature GenerateSatyr()
    {

      return new Creature((Illustration)new ModdedIllustration("DawnniburyExpandedAssets/Satyr.png"), "Satyr", (IList<Trait>)new Trait[3]
      {
        Trait.Chaotic,
        Trait.Neutral,
        Trait.Humanoid
      }, 4, 10, 7,
      new Defenses(19, 9, 11, 12),
      80,
       new Abilities(4, 4, 1, 1, 2, 5),
       new Skills(athletics: 8, deception: 13, diplomacy: 13, intimidation: 11, stealth: 11, performance: 13, survival: 8, nature: 8))
       .AddHeldItem(Items.CreateNew(ItemName.CompositeLongbow))
       .WithProficiency(Trait.Unarmed, (Proficiency)3)
       .WithProficiency(Trait.Weapon, Proficiency.Master)
       .WithProficiency(Trait.Spell, Proficiency.Trained)
       .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.FleetStep, "hooves", "1d4", DamageKind.Bludgeoning, Trait.Agile, Trait.Finesse))
       .WithSpellProficiencyBasedOnSpellAttack(11, Ability.Charisma)
       .AddSpellcastingSource(SpellcastingKind.Spontaneous, Trait.Bard, Ability.Charisma, Trait.Occult)
       .WithSpells(new SpellId[0],
       new SpellId[]{
        SpellId.CalmEmotions,
        SpellId.TouchOfIdiocy,
        SpellId.Fear,
        SpellId.HideousLaughter,
        SpellInspireCourage.Id,
      },
      new SpellId[0],
      new SpellId[1] { SpellId.Fear }).WithSpontaneousSlots(3, 0, 1)
      .Done()

       .AddQEffect(new QEffect()
       {

         StartOfCombat = (async (QEffect qf) =>
         {
           qf.Owner.Battle.Cinematics.EnterCutscene();
           await qf.Owner.Battle.Cinematics.LineAsync(qf.Owner, "The filthy rats of the city have finally clawed their way into my garden, the so called civilized animals do not dance to beautiful tunes.");
           await qf.Owner.Battle.Cinematics.LineAsync(qf.Owner, "You rats protect the unnatural and unholy, those who destory the world brick by brick. I will not allow it.");
           await qf.Owner.Battle.Cinematics.LineAsync(qf.Owner, "Nor will the friends of the wilds. Comrades, dance with these intruders!");
           qf.Owner.Battle.Cinematics.ExitCutscene();
         }),


         YouBeginAction = (async (qf, hostileAction) =>
                 {
                   if (hostileAction.ActionCost <= 1 || !hostileAction.HasTrait(Trait.Spell))
                   {
                     return;
                   }
                   await qf.Owner.StrideAsync("Choose where to Stride or Step with Fleet Performer.", allowStep: true, allowPass: true, allowCancel: true, maximumHalfSpeed: true);

                 }),
         AfterYouTakeAction = (async (qf, hostileAction) =>
                 {
                   if (hostileAction.ActionCost <= 1 || !hostileAction.HasTrait(Trait.Spell))
                   {
                     return;
                   }
                   await qf.Owner.StrideAsync("Choose where to Stride or Step with Fleet Performer.", allowStep: true, allowPass: true, allowCancel: true, maximumHalfSpeed: true);

                 }),
         /*
                 ProvideContextualAction = (qf => (Possibility) (ActionPossibility) 
                 new CombatAction(qf.Owner, (Illustration) IllustrationName.FleetStep, "Flee", new Trait[2]
                 {
                   Trait.Move,
                   Trait.Basic
                 },"Try to not be next to enemy.",Target.Self((Func<Creature, AI, float>) ((cr, ai) =>
                 {
                   if (cr.Actions.ActionsLeft == 1 &&
                     cr.Battle.AllCreatures.Any<Creature>(enemy => 
                     enemy.EnemyOf(cr)  
                     && cr.IsAdjacentTo(enemy)) 
                     )
                     {
                     return (float) int.MaxValue;
                     } else return int.MinValue;

                 })))
                 .WithActionCost(1)
                 .WithEffectOnSelf(async (CombatAction spell, Creature caster) =>{
                   await caster.StrideAsync("Choose where to Stride or Step.",true,allowCancel:false,allowPass:false);
                 })),
         */
         Innate = true,
         Name = "Fleet Performer",
         Description = "When the satyr casts a spell using two or more actions, they can Step or Stride before and after the cast at half speed."
       });
    }



  }
}