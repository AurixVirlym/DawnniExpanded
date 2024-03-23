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

public class SpellInnerRadianceTorrent
{

    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/InnerRadianceTorrent.png");
    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat, SpellInformation spellInformation)
    {

        CombatAction casterInnerRadianceTorrentSpell = Spells.CreateModern(SpellIllustration,
                "Inner Radiance Torrent",
            new[] { Trait.Force, Trait.Necromancy, Trait.Occult, Trait.Divine, Trait.Light, DawnniExpanded.DETrait },
                    "You gradually manifest your spiritual energy into your cupped hands before firing off a storm of bolts and beams.",
                    "This storm deals " + S.HeightenedVariable((spellLevel - 1) * 4, 4) + "d4 force damage to all creatures in a line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round. The number of actions you spend when Casting this Spell determines the area. "
                    + "\r\n\r\n{icon:TwoActions} The line is 60 feet long."
                    + "\r\n{icon:ThreeActions} The line is 120 feet long."
                    + "\r\n{b}Two Rounds{/b} The line is 120 feet long. If you spend 3 actions casting the spell, you can avoid finishing the spell and spend another 3 actions on your next turn to empower the spell even further. If you choose to do so, the damage dealt by this spell increases by 4d4, and you enter a shining state for 1 minute, causing you to glow with light and deal " + S.HeightenedVariable((spellLevel - 1) * 1, 1) + " force damage to creatures adjacent to you at the start of your turn."
                    + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1.")

                    ,
                     new DependsOnActionsSpentTarget(
                    null,
                    Target.Line(12),
                    Target.Line(24),
                    Target.Self()
                    ),
                        spellLevel,
                        SpellSavingThrow.Basic(Defense.Reflex)
                        ).WithActionCost(-5)
                        .WithCreateVariantDescription((int actionCost, SpellVariant _) => actionCost switch
                    {
                        2 => "This storm deals " + S.HeightenedVariable((spellLevel - 1) * 4, 4) + "d4 force damage to all creatures in a 60ft line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round."
                    + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1."),
                        3 => "This storm deals " + S.HeightenedVariable((spellLevel - 1) * 4, 4) + "d4 force damage to all creatures in a 120ft line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1."),
                        6 => "This storm deals " + S.HeightenedVariable((spellLevel) * 4, 4) + "d4 force damage to all creatures in a 120ft line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round. You enter a shining state for 1 minute, causing you to glow with light and deal " + S.HeightenedVariable((spellLevel - 1) * 1, 1) + " force damage to creatures adjacent to you at the start of your turn." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1."),
                        _ => null,
                    })
                        .WithSoundEffect(SfxName.DivineLance)
                        .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, spellcaster, target, result) =>
                        {
                            string HitDamage = (spellLevel - 1) * 4 + "d4";
                            string BurstDmage = (spellLevel) * 4 + "d4";

                            CombatAction casterInnerRadianceTorrentSixAction = Spells.CreateModern(SpellIllustration,
                            "Fully Charged Inner Radiance Torrent",
                            new[] { Trait.Force, Trait.Necromancy, Trait.Occult, Trait.Divine, Trait.Light, DawnniExpanded.DETrait },
                                "You gradually manifest your spiritual energy into your cupped hands before firing off a storm of bolts and beams.",
                                "This storm deals " + S.HeightenedVariable((spellLevel) * 4, 4) + "d4 force damage to all creatures in a 120ft line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round. You enter a shining state for 1 minute, causing you to glow with light and deal " + S.HeightenedVariable((spellLevel - 1) * 1, 1) + " force damage to creatures adjacent to you at the start of your turn." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1.")
                                ,
                                Target.Line(24)
                                ,
                                spellLevel,
                                SpellSavingThrow.Basic(Defense.Reflex)
                                ).WithActionCost(3)
                                .WithSoundEffect(SfxName.DivineLance)
                                .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell1, caster, target1, result) =>
                                {

                                    string HitDamage1 = (spellLevel - 1) * 4 + "d4";
                                    string BurstDmage1 = (spellLevel) * 4 + "d4";


                                    await CommonSpellEffects.DealBasicDamage(spell1, caster, target1, result, BurstDmage1, DamageKind.Force);

                                    if (result == CheckResult.CriticalFailure)
                                    {
                                        target1.AddQEffect(QEffect.Blinded().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                                    }


                                    caster.QEffects.First((QEffect qf) => qf.Name == "Two Round Inner Radiant Torrent").ExpiresAt = ExpirationCondition.Immediately;


                                }



                                )).WithEffectOnSelf(async (spell, caster) =>
                            {
                                string ShiningDmg = (spellLevel - 1).ToString();

                                QEffect casterInnerRadianceTorrentSparks = new QEffect("Shining State",
                                        "Deal " + ShiningDmg + " force damage to creatures adjacent to you at the start of your turn.",
                                        ExpirationCondition.Never,
                                        spellcaster, SpellIllustration)
                                {
                                    StartOfYourTurn = (async (effect, creature) =>
                                    {
                                        foreach (Creature enemy in creature.Battle.AllCreatures.Where<Creature>(Enemy =>
                                         creature.IsAdjacentTo(Enemy)))
                                        {
                                            await CommonSpellEffects.DealBasicDamage(spell, caster, enemy, CheckResult.Failure, ShiningDmg, DamageKind.Force);
                                        }
                                        return;
                                    })
                                };

                                if (spellcaster.HasEffect(casterInnerRadianceTorrentSparks))
                                {
                                    return;
                                }
                                caster.AddQEffect(casterInnerRadianceTorrentSparks);

                            });

                            casterInnerRadianceTorrentSixAction.Owner = spellcaster;
                            casterInnerRadianceTorrentSixAction.SpellcastingSource = spell.SpellcastingSource;

                            CombatAction casterInnerRadianceTorrentThreeAction = Spells.CreateModern(SpellIllustration,
                        "Three Action Inner Radiant Torrent",
                        new[] { Trait.Force, Trait.Necromancy, Trait.Occult, Trait.Divine, Trait.Light, DawnniExpanded.DETrait },
                            "You gradually manifest your spiritual energy into your cupped hands before firing off a storm of bolts and beams.",
                                "This storm deals " + S.HeightenedVariable((spellLevel - 1) * 4, 4) + "d4 force damage to all creatures in a 120ft line. Creatures in the area must attempt a basic Reflex save. On a critical failure, they're also blinded for 1 round." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The initial damage, as well as the additional damage for the 2-round casting time, each increase by 4d4, and the damage to adjacent creatures dealt while in your shining state increases by 1.")
                            ,
                            Target.Line(24)
                            ,
                            spell.SpellLevel,
                            SpellSavingThrow.Basic(Defense.Reflex)
                            ).WithActionCost(0)
                            .WithSoundEffect(SfxName.DivineLance)
                            .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell1, caster, target1, result) =>
                            {

                                string HitDamage1 = (spellLevel - 1) * 4 + "d4";
                                string BurstDmage1 = (spellLevel) * 4 + "d4";

                                await CommonSpellEffects.DealBasicDamage(spell1, caster, target1, result, HitDamage1, DamageKind.Force);

                                if (result == CheckResult.CriticalFailure)
                                {
                                    target1.AddQEffect(QEffect.Blinded().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                                }



                            }))
                            .WithEffectOnSelf(async (spell1, caster) =>
                            {
                                caster.QEffects.FirstOrDefault((QEffect qf) => qf.Name == "Two Round Inner Radiant Torrent").ExpiresAt = ExpirationCondition.Immediately;
                            });

                            casterInnerRadianceTorrentThreeAction.Owner = spellcaster;
                            casterInnerRadianceTorrentThreeAction.SpellcastingSource = spell.SpellcastingSource;

                            if (spell.SpentActions == 6)
                            {

                                QEffect SixActionThunderSphereEffect =
                                new QEffect("Two Round Inner Radiant Torrent",
                                "You are charging an Inner Radiant Torrent.",
                                ExpirationCondition.Never,
                                spellcaster, IllustrationName.None)
                                {

                                    DoNotShowUpOverhead = true,
                                    ProvideContextualAction = qfUserQEffect =>
                                    {
                                        SubmenuPossibility submenuPossibility = new SubmenuPossibility(SpellIllustration, "Inner Radiant Torrent");
                                        PossibilitySection possibilitySection = new PossibilitySection("Inner Radiant Torrent");
                                        possibilitySection.AddPossibility(new ActionPossibility(casterInnerRadianceTorrentThreeAction));
                                        possibilitySection.AddPossibility(new ActionPossibility(casterInnerRadianceTorrentSixAction));
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

                            await CommonSpellEffects.DealBasicDamage(spell, spellcaster, target, result, HitDamage, DamageKind.Force);


                            if (result == CheckResult.CriticalFailure)
                            {
                                target.AddQEffect(QEffect.Blinded().WithExpirationAtStartOfSourcesTurn(spellcaster, 1));
                            }

                        })
                         );


        return casterInnerRadianceTorrentSpell;


    }

    public static void LoadMod()
    {


        Id = ModManager.RegisterNewSpell("Inner Radiant Torrent", 2, ((spellId, spellcaster, spellLevel, inCombat, SpellInformation) => CombatAction(spellcaster, spellLevel, inCombat, SpellInformation)

    ));

    }
}


