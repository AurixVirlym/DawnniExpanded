using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Campaign.Encounters;
using Dawnsbury.Campaign.Encounters.Elements_of_a_Crime;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Text;
using Dawnsbury.IO;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;


namespace Dawnsbury.Mods.DawnniExpanded
{
    public class MonsterBadger
    {
        
    public static Creature CreateSmallBadger(){
    Creature SmallBadger =  new Creature(new ModdedIllustration("DawnniburyExpandedAssets/SmallBadger.png"),
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
                .WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d6", DamageKind.Slashing,Trait.Agile));
                return SmallBadger;
    }

    public static Illustration JojoBadger = new ModdedIllustration("DawnniburyExpandedAssets/JojoBadger.png");
    public static void LoadMod()
    {
        ModManager.RegisterNewCreature("Badger", (encounter) =>
        {
            
           return CreateSmallBadger();
                
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
                .WithProficiency(Trait.Unarmed, (Proficiency)7)
                .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d8", DamageKind.Piercing))
                .WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d6", DamageKind.Slashing,Trait.Agile))
                .AddQEffect(new QEffect()
      {
       
      Innate = true,
      Name = "Enter Badger Rage",
      DoNotShowUpOverhead = true,
      Description = "Badger gains +4 to damage and reduces their AC by 1. Evil.",
        ProvideMainAction =  (qfBadgerRage => (Possibility) (ActionPossibility) 
        new CombatAction(qfBadgerRage.Owner, (Illustration) IllustrationName.Rage, "Enter Badger Rage", new Trait[4]
        {
          Trait.Concentrate,
          Trait.Emotion,
          Trait.Mental,
          Trait.Basic
        },"Badger gains +4 to damage and reduces their AC by 1. Evil.",Target.Self((Func<Creature, AI, float>) ((cr, ai) =>
        {
          if (cr.Actions.ActionsLeft >= 2 &&
            cr.Battle.AllCreatures.Any<Creature>(enemy => 
            enemy.EnemyOf(cr)  
            && cr.IsAdjacentTo(enemy)) 
            && cr.HP != cr.MaxHP && cr.QEffects.All(qf => qf.Name != "Badger Rage")
            )
            {
            return (float) int.MaxValue;
            } else return int.MinValue;

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
                                      } else return null;
                                  },
                                
                                BonusToDamage = (QEffect qf, CombatAction attack, Creature target) => {
                                    return new Bonus(4, BonusType.Untyped, "Badger Rage");
                                },

                                ExpiresAt = ExpirationCondition.Never,
                                StateCheck = Qfrage => {
                                    Qfrage.Owner.Illustration = JojoBadger;
                                    if (Qfrage.Owner.HasEffect(QEffectId.CalmEmotions)){
                                        Qfrage.ExpiresAt = ExpirationCondition.Immediately;
                                    
                                }

                                }
                              };
                            caster.AddQEffect(BadgerRage);
                            caster.WithTactics(Tactic.Mindless);
                            caster.AddQEffect(new QEffect("Immunity to Badger Rage","[this condition has no description]", ExpirationCondition.Never, caster, IllustrationName.None)
                            {
                                PreventTargetingBy = (CombatAction action) => (action.Name != spell.Name) ? null : "immunity"
                            }
                            );
                            })
        
        ),


    });

    return GiantBadger;
    });

    ModManager.RegisterNewCreature("Satyr", (encounter) => {return GenerateSatyr();});
}

public static Creature GenerateSatyr()
    {
      
      return new Creature((Illustration) new ModdedIllustration("DawnniburyExpandedAssets/Satyr.png"), "Satyr", (IList<Trait>) new Trait[3]
      {
        Trait.Chaotic,
        Trait.Neutral,
        Trait.Humanoid
      }, 4, 10, 7, 
      new Defenses(19, 9, 11, 12), 
      80,
       new Abilities(4, 4, 1, 1, 2, 5), 
       new Skills(athletics: 8, deception: 13, diplomacy: 13, intimidation: 11, stealth: 11,performance:13,survival:8,nature:8))
       .AddHeldItem(Items.CreateNew(ItemName.CompositeLongbow))
       .WithProficiency(Trait.Unarmed, (Proficiency)3)
       .WithProficiency(Trait.Weapon, Proficiency.Master)
       .WithProficiency(Trait.Spell, Proficiency.Trained)
       .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.FleetStep, "hooves", "1d4", DamageKind.Bludgeoning, Trait.Agile, Trait.Finesse))
       .AddSpellcasting(Ability.Charisma, 11)
       .AddSpontaneousSpellcasting(0, new SpellId[0],
      3, new SpellId[]{
        SpellId.CalmEmotions,
        SpellId.TouchOfIdiocy,
        SpellId.Fear,
        SpellId.HideousLaughter,
        SpellInspireCourage.spellId,
      },
      0,new SpellId[0],
      1,new SpellId[1]{SpellId.Fear}
      )
      .AddQEffect(new QEffect(){
        YouBeginAction = (async (qf, hostileAction) =>
                {
                  if (hostileAction.ActionCost <= 1 || !hostileAction.HasTrait(Trait.Spell))
                  {
                    return;
                  }
                  await qf.Owner.StrideAsync("Choose where to Stride or Step with Fleet Performer.",allowStep:true,allowPass:true, allowCancel: true,maximumHalfSpeed: true);

                }),
        AfterYouTakeAction = (async (qf, hostileAction) =>
                {
                  if (hostileAction.ActionCost <= 1 || !hostileAction.HasTrait(Trait.Spell))
                  {
                    return;
                  }
                  await qf.Owner.StrideAsync("Choose where to Stride or Step with Fleet Performer.",allowStep:true,allowPass:true, allowCancel: true,maximumHalfSpeed: true);

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