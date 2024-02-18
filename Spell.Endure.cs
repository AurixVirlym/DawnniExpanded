using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Display.Text;
using System;


namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellEndure{


    
    public static void LoadMod()
    {

        ModManager.RegisterNewSpell("Endure", 1, (spellId, spellcaster, spellLevel, inCombat) =>
        {
            return Spells.CreateModern(new ModdedIllustration("DawnniburyExpandedAssets/Endure.png"), 
                "Endure",
            new[] { Trait.Arcane, Trait.Occult, Trait.Enchantment, Trait.Mental, Trait.DoesNotProvoke, DawnniExpanded.DETrait }, 
                    "You invigorate the touched creature's mind and urge it to press on.",
                    "You grant the touched creature " + S.HeightenedVariable(spellLevel * 4, 4) + " temporary Hit Points.\n",
                    Target.AdjacentFriendOrSelf(),
                        1, 
                        null
                        ).WithActionCost(1)
                        .WithSoundEffect(SfxName.Mental)
                        .WithEffectOnChosenTargets(async (CombatAction spell, Creature caster, ChosenTargets chosenTargets) =>
                        
                        {
                            Creature target = chosenTargets.ChosenCreature;
                            int EndureTHP = spellLevel*4;
                            target.GainTemporaryHP(EndureTHP);
                            
                            /*
                            QEffect EndureEffect = new QEffect("Endure", "", ExpirationCondition.ExpiresAtStartOfSourcesTurn, caster, IllustrationName.None)
                            {
                                Value = EndureTHP,
                                WhenExpires = qf =>
                                
                                {
                                    if (qf.Value > 0){
                                    qf.Owner.TemporaryHP -= qf.Value;
                                    qf.Owner.Battle.Log(qf.Owner?.ToString() + " loses " + qf.Value + " temporary HP from Endure ending.");
                                    };
                                },
                                
                                YouAreDealtDamage = async (QEffect qEffect, Creature attacker, DamageStuff damageStuff, Creature you) =>
                                {

                                    qEffect.Value -= Math.Min(damageStuff.Amount,qEffect.Value);
                                    if (qEffect.Value <= 0)
                                    {
                                        qEffect.Value = 0;
                                        qEffect.ExpiresAt = ExpirationCondition.Immediately;
                                    }
                                    
                                    return null;
                               

                                },
                            };

                            target.AddQEffect(EndureEffect);
                            */
                        }

                        );
        });
    }
}
                
                
