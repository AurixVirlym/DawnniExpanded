using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Display.Text;
using System;

using Dawnsbury.Core.Mechanics.Core;
using System.Linq;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;




namespace Dawnsbury.Mods.DawnniExpanded;

public class SpellFalseLife
{

    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/FalseLife.png");

    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat, SpellInformation spellInformation)
    {


        QEffect FalseLifeEffect = new QEffect()
        {
            Illustration = SpellIllustration,
            ExpiresAt = ExpirationCondition.Never,
            Description = "You are have temporary Hit Points from False Life.",
            Name = "False Life - ",
            DoNotShowUpOverhead = true,
            CountsAsABuff = true,

            YouAreDealtDamage = async (QEffect qEffect, Creature attacker, DamageStuff damageStuff, Creature you) =>
            {

                qEffect.Value -= Math.Min(damageStuff.Amount, qEffect.Value);
                if (qEffect.Value <= 0)
                {
                    qEffect.Value = 0;
                    qEffect.ExpiresAt = ExpirationCondition.Immediately;
                }

                return null;
            },

            EndOfCombat = async (QEffect qf, bool winstate) =>
            {
                qf.Owner.PersistentUsedUpResources.UsedUpActions.Add("FalseLife:" + qf.Value);
            },

        };

        CombatAction falseLife = Spells.CreateModern(SpellIllustration, "False Life", new Trait[]
            {
                Trait.Necromancy,
                Trait.Arcane,
                Trait.Occult,
                DawnniExpanded.DETrait
            }, "You ward yourself with shimmering magical energy.",
                "You gain " + S.HeightenedVariable(10 + (spellLevel - 2) * 3, 6) + " temporary Hit Points.\n\n{b}Special{/b} You can cast this spell as a free action at the beginning of the encounter if not casting from a scroll." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "{b}Heightened (+1){/b} The temporary Hit Points increase by 3.")
                , Target.Self(),
                spellLevel,
                 null)
                .WithSoundEffect(SfxName.Healing)
                .WithActionCost(2)
                .WithEffectOnEachTarget(async (CombatAction spell, Creature caster, Creature target, CheckResult result) =>
                            {
                                int FalseLifeTHP = 10 + (spell.SpellLevel - 2) * 3;
                                QEffect falseLifeEffect = FalseLifeEffect;

                                QEffect qEffect2 = target.QEffects.FirstOrDefault((QEffect qf) => qf.Name == "False Life - ");
                                if (qEffect2 != null)
                                {
                                    if (FalseLifeTHP > qEffect2.Value)
                                    {
                                        caster.GainTemporaryHP(FalseLifeTHP);
                                        qEffect2.Value = FalseLifeTHP;
                                    }

                                }
                                else
                                {
                                    falseLifeEffect.Source = caster;
                                    falseLifeEffect.Value = FalseLifeTHP;
                                    caster.GainTemporaryHP(FalseLifeTHP);
                                    caster.AddQEffect(falseLifeEffect);
                                }

                            });

        falseLife.WhenCombatBegins = delegate (Creature self)
        {
            string LastfalseLifeString = self.PersistentUsedUpResources.UsedUpActions.Find(word => word.Contains("FalseLife:"));
            int LastfalseLifeValue = 0;
            if (LastfalseLifeString != null)
            {
                string[] LastfalseLifeStringSplit = LastfalseLifeString.Split(':');
                QEffect falseLifeEffect = FalseLifeEffect;

                FalseLifeEffect.Source = self;
                FalseLifeEffect.Value = Int32.Parse(LastfalseLifeStringSplit[1]);
                self.GainTemporaryHP(FalseLifeEffect.Value);
                LastfalseLifeValue = FalseLifeEffect.Value;
                self.AddQEffect(falseLifeEffect);
                self.PersistentUsedUpResources.UsedUpActions.Remove(LastfalseLifeString);

            }
            if (LastfalseLifeValue < self.TemporaryHP || self.TemporaryHP == 0)
            {
                self.AddQEffect(new QEffect
                {
                    StartOfCombat = async delegate
                                    {
                                        if (await self.Battle.AskForConfirmation(self, SpellIllustration, "Do you want to cast {i}false life level " + falseLife.SpellLevel + " {/i} as a free action?", "Cast {i}false life{/i}"))
                                        {
                                            await self.Battle.GameLoop.FullCast(falseLife);
                                        }
                                    }
                });
            }
        };



        return falseLife;


    }

    public static void LoadMod()
    {





        Id = ModManager.RegisterNewSpell("False Life", 1, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) => CombatAction(spellcaster, spellLevel, inCombat, SpellInformation));

    }
}


