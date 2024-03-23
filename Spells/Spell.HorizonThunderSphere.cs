using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Display.Text;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Possibilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Core;

namespace Dawnsbury.Mods.DawnniExpanded;

public class SpellHorizonThunderSphere
{

    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/HorizonThunderSphere.png");
    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
    {

        CombatAction HorizonThunderSphereSpell = Spells.CreateModern(SpellIllustration,
                "Horizon Thunder Sphere",
            new[] { Trait.Electricity, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait },
                    "You gather magical energy into your palm, forming a concentrated ball of electricity that crackles and rumbles like impossibly distant thunder.",
                    "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round. The number of actions you spend when Casting this Spell determines the range and other parameters."
                    + "\r\n\r\n{icon:TwoActions} This spell has a range of 30 feet."
                    + "\r\n{icon:ThreeActions} This spell has a range of 60 feet and deals half damage on a failure (but not a critical failure) as the electricity lashes out and jolts the target."
                    + "\r\n{b}Two Rounds{/b} If you spend 3 actions Casting the Spell, you can avoid finishing the spell and spend another 3 actions on your next turn to empower the spell even further. If you do, after attacking the target, whether you hit or miss, the ball of lightning explodes, dealing " + S.HeightenedVariable(spellLevel * 2, 2) + "d6 electricity damage to all other creatures in a 10-foot emanation around the target (basic Reflex save). Additionally, you spark with electricity for 1 minute, dealing " + S.HeightenedVariable(spellLevel * 1, 1) + " electricity damage to creatures that Grab you or that hit you with an unarmed Strike or a non-reach melee weapon."
                    + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1.")

                    ,
                     new DependsOnActionsSpentTarget(
                    null,
                    Target.Ranged(6),
                    Target.Ranged(12),
                    Target.Self()
                    ),
                        spellLevel,
                        null
                        ).WithActionCost(-5)
                        .WithCreateVariantDescription((int actionCost, SpellVariant _) => actionCost switch
                    {
                        2 => "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round." + "\r\n\r\nThis spell has a range of 30 feet." + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1."),
                        3 => "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round." + "\r\n\r\nThis spell has a range of 60 feet and deals half damage on a failure (but not a critical failure) as the electricity lashes out and jolts the target." + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1."),
                        6 => "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round." + "\r\n\r\nThis spell has a range of 60 feet and deals half damage on a failure (but not a critical failure) as the electricity lashes out and jolts the target. You do not finishing the spell this turn and may spend another 3 actions on your next turn to empower the spell even further. If you do, after attacking the target, whether you hit or miss, the ball of lightning explodes, dealing " + S.HeightenedVariable(spellLevel * 2, 2) + "d6 electricity damage to all other creatures in a 10-foot emanation around the target (basic Reflex save). Additionally, you spark with electricity for 1 minute, dealing " + S.HeightenedVariable(spellLevel * 1, 1) + " electricity damage to creatures that Grab you or that hit you with an unarmed Strike or a non-reach melee weapon." + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1."),
                        _ => null,
                    })
                        .WithSoundEffect(SfxName.ElectricArc)
                        .WithSpellAttackRoll()
                        .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, spellcaster, target, result) =>
                        {


                            string HitDamage = 1 + spellLevel * 2 + "d6";
                            string BurstDmage = (spellLevel * 2) + "d6";

                            CombatAction HorizonThunderSphereSixAction = Spells.CreateModern(SpellIllustration,
                            "Fully Charged Horizon Thunder Sphere",
                            new[] { Trait.Electricity, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait },
                                "You gather magical energy into your palm, forming a concentrated ball of electricity that crackles and rumbles like impossibly distant thunder.",
                                "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round." + "\r\n\r\nThis spell has a range of 60 feet and deals half damage on a failure (but not a critical failure) as the electricity lashes out and jolts the target. You do not finishing the spell this turn and may spend another 3 actions on your next turn to empower the spell even further. If you do, after attacking the target, whether you hit or miss, the ball of lightning explodes, dealing " + S.HeightenedVariable(spellLevel * 2, 2) + "d6 electricity damage to all other creatures in a 10-foot emanation around the target (basic Reflex save). Additionally, you spark with electricity for 1 minute, dealing " + S.HeightenedVariable(spellLevel * 1, 1) + " electricity damage to creatures that Grab you or that hit you with an unarmed Strike or a non-reach melee weapon." + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1.")
                                ,
                                Target.Ranged(12)
                                ,
                                spellLevel,
                                null
                                ).WithActionCost(3)
                                .WithSpellAttackRoll()
                                .WithSoundEffect(SfxName.ElectricArc)
                                .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell1, caster, target1, result) =>
                                {

                                    string HitDamage1 = 1 + spellLevel * 2 + "d6";
                                    string BurstDamage1 = (spellLevel * 2) + "d6";

                                    if (result == CheckResult.Failure)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(spell1, caster, target1, CheckResult.Success, HitDamage1, DamageKind.Electricity);
                                    }
                                    else
                                    {
                                        await CommonSpellEffects.DealAttackRollDamage(spell1, caster, target1, result, HitDamage1, DamageKind.Electricity);
                                    }

                                    if (result == CheckResult.CriticalSuccess)
                                    {
                                        target1.AddQEffect(QEffect.Dazzled().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                                    }

                                    List<Tile> sphereEffect = new List<Tile>();
                                    foreach (Edge item in target1.Occupies.Neighbours.ToList())
                                    {
                                        sphereEffect.Add(item.Tile);
                                    }
                                    await CommonAnimations.CreateConeAnimation(target1.Battle, target1.Occupies.ToCenterVector(), sphereEffect, 25, ProjectileKind.Cone, SpellIllustration);



                                    foreach (Creature target2 in target1.Battle.AllCreatures.Where<Creature>((Func<Creature, bool>)(cr => cr.DistanceTo(target1) <= 2 && cr != target1)).ToList<Creature>())
                                    {
                                        CheckResult checkResult = CommonSpellEffects.RollSpellSavingThrow(target2, spell1, Defense.Reflex);
                                        await CommonSpellEffects.DealBasicDamage(spell, caster, target2, checkResult, BurstDamage1, DamageKind.Electricity);

                                    }

                                    QEffect HorizonThunderSphereSparks = new QEffect("Horizon Thunder Sphere Sparks",
                                        "Deal " + spell.SpellLevel.ToString() + " electricity to foes grabbing or strking you with unarmed attacks",
                                        ExpirationCondition.Never,
                                        spellcaster, SpellIllustration)
                                    {
                                        AfterYouTakeDamage = (Delegates.AfterYouTakeDamage)(async (effect, amount, damageKind, combatAction, critical) =>
                                       {
                                           CombatAction combatAction1 = combatAction;
                                           if (combatAction1 != null)
                                           {
                                               if ((combatAction1.HasTrait(Trait.Melee) && combatAction1.HasTrait(Trait.Grab)) || combatAction1.HasTrait(Trait.Grab))
                                               {
                                                   await spellcaster.DealDirectDamage(spell, DiceFormula.FromText(spell.SpellLevel.ToString(), "Horizon Thunder Sphere Sparks"), combatAction.Owner, CheckResult.Failure, DamageKind.Electricity);
                                               }
                                           }

                                       })
                                    };

                                    if (spellcaster.HasEffect(HorizonThunderSphereSparks))
                                    {
                                        return;
                                    }
                                    caster.AddQEffect(HorizonThunderSphereSparks);

                                    caster.QEffects.First((QEffect qf) => qf.Name == "Two Round Horizon Thunder Sphere").ExpiresAt = ExpirationCondition.Immediately;



                                }



                                ));

                            HorizonThunderSphereSixAction.Owner = spellcaster;
                            HorizonThunderSphereSixAction.SpellcastingSource = spell.SpellcastingSource;

                            CombatAction HorizonThunderSphereThreeAction = Spells.CreateModern(SpellIllustration,
                        "Three Action Horizon Thunder Sphere",
                        new[] { Trait.Electricity, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait },
                            "You gather magical energy into your palm, forming a concentrated ball of electricity that crackles and rumbles like impossibly distant thunder.",
                            "Make a ranged spell attack roll against your target's AC. On a success, you deal " + S.HeightenedVariable(1 + spellLevel * 2, 3) + "d6 electricity damage. On a critical success, the target takes double damage and is dazzled for 1 round." + "\r\n\r\nThis spell has a range of 60 feet and deals half damage on a failure (but not a critical failure) as the electricity lashes out and jolts the target." + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage on a hit, as well as the burst damage for two-round casting time, each increase by 2d6, and the damage creatures take if they Grapple or hit you while you're in your sparking state increases by 1.")
                            ,
                            Target.Ranged(12)
                            ,
                            spell.SpellLevel,
                            null
                            ).WithActionCost(0)
                            .WithSpellAttackRoll()
                            .WithSoundEffect(SfxName.ElectricArc)
                            .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell1, caster, target1, result) =>
                            {

                                string HitDamage1 = 1 + spellLevel * 2 + "d6";
                                string BurstDamage1 = (spellLevel * 2) + "d6";

                                if (result == CheckResult.Failure)
                                {
                                    await CommonSpellEffects.DealBasicDamage(spell1, caster, target1, CheckResult.Success, HitDamage1, DamageKind.Electricity);
                                }
                                else
                                {
                                    await CommonSpellEffects.DealAttackRollDamage(spell1, caster, target1, result, HitDamage1, DamageKind.Electricity);
                                }

                                if (result == CheckResult.CriticalSuccess)
                                {
                                    target1.AddQEffect(QEffect.Dazzled().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                                }



                            }))
                            .WithEffectOnSelf(async (spell1, caster) =>
                            {
                                caster.QEffects.First((QEffect qf) => qf.Name == "Two Round Horizon Thunder Sphere").ExpiresAt = ExpirationCondition.Immediately;
                            });

                            HorizonThunderSphereThreeAction.Owner = spellcaster;
                            HorizonThunderSphereThreeAction.SpellcastingSource = spell.SpellcastingSource;

                            if (spell.SpentActions == 6)
                            {

                                QEffect SixActionThunderSphereEffect =
                                new QEffect("Two Round Horizon Thunder Sphere",
                                "You are charging a Horizon Thunder Sphere.",
                                ExpirationCondition.Never,
                                spellcaster, IllustrationName.None)
                                {

                                    DoNotShowUpOverhead = true,
                                    ProvideContextualAction = qfUserQEffect =>
                                    {
                                        SubmenuPossibility submenuPossibility = new SubmenuPossibility(SpellIllustration, "Horizon Thunder Sphere");
                                        PossibilitySection possibilitySection = new PossibilitySection("Horizon Thunder Sphere");
                                        possibilitySection.AddPossibility(new ActionPossibility(HorizonThunderSphereThreeAction));
                                        possibilitySection.AddPossibility(new ActionPossibility(HorizonThunderSphereSixAction));
                                        submenuPossibility.Subsections.Add(possibilitySection);
                                        return submenuPossibility;
                                    }


                                }.WithExpirationAtStartOfSourcesTurn(spellcaster, 2);

                                spellcaster.AddQEffect(SixActionThunderSphereEffect);

                                return;
                            };


                            if (spell.SpentActions == 6)
                            {
                                return;
                            }

                            //CheckResult AttackResult = CommonSpellEffects.RollCheck("Horizon Thunder Sphere", new ActiveRollSpecification(Checks.SpellAttack(), Checks.DefenseDC(Defense.AC)),spellcaster,target);

                            if (spell.SpentActions == 3 && result == CheckResult.Failure)
                            {

                                await CommonSpellEffects.DealBasicDamage(spell, spellcaster, target, CheckResult.Success, HitDamage, DamageKind.Electricity);

                            }
                            else if (spell.SpentActions == 2 || spell.SpentActions == 3)
                            {
                                await CommonSpellEffects.DealAttackRollDamage(spell, spellcaster, target, result, HitDamage, DamageKind.Electricity);
                                if (result == CheckResult.CriticalSuccess)
                                {
                                    target.AddQEffect(QEffect.Dazzled().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                                }
                            }

                        })
                         );


        return HorizonThunderSphereSpell;


    }

    public static void LoadMod()
    {



        Id = ModManager.RegisterNewSpell("Horizon Thunder Sphere", 1, ((spellId, spellcaster, spellLevel, inCombat, SpellInformation) => CombatAction(spellcaster, spellLevel, inCombat)

    ));

    }
}


