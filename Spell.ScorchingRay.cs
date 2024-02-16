using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Display.Text;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Microsoft.Xna.Framework;
using Dawnsbury.Display.Illustrations;

namespace Dawnsbury.Mods.DawnniExpanded;

public class SpellScorchingRay
{
    public static void LoadMod()
    {
        ModManager.RegisterNewSpell("ScorchingRay", 2, ((spellId, spellcaster, spellLevel, inCombat) =>
        {
            CreatureTarget creatureTarget = Target.Ranged(12);
            return Spells.CreateModern(new ModdedIllustration("DawnniburyExpandedAssets/ScorchingRay.png"), 
                "Scorching Ray",
            new[] { Trait.Fire, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait }, 
                    "You fire a ray of heat and flame.",
                    "Make a ranged spell attack roll against a single creature within 60ft." + S.FourDegreesOfSuccessReverse((string) null, (string) null, "The target takes 2d6 fire damage.", "Double damage.") + "\n\nFor each additional action you use when Casting the Spell, you can fire an additional ray at a different target, to a maximum of three rays targeting three different targets for 3 actions. These attacks each increase your multiple attack penalty, but you don't increase your multiple attack penalty until after you make all the spell attack rolls for scorching ray.\n\nIf you spend 2 or more actions Casting the Spell, the damage increases to 4d6 fire damage on a hit, and it still deals double damage on a critical hit.",
                    Target.DependsOnActionsSpent(
                        Target.MultipleCreatureTargets(creatureTarget).WithMustBeDistinct(),
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget).WithMustBeDistinct(), 
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget, creatureTarget).WithMustBeDistinct()
                        ),
                        2, 
                        null
                        ).WithActionCost(-1).WithSpellAttackRoll().WithSoundEffect(SfxName.FireRay).WithEffectOnEachTarget((Delegates.EffectOnEachTarget) (async (spell, caster, target, result) => 
                        {
                            var _Damage = ((spellLevel)*2) + "d6";

                            if (spell.SpentActions == 1){
                            _Damage = (2+(spellLevel-2)) + "d6";
                            }
                            
                    
                    await caster.Battle.SpawnOverairProjectileParticlesAsync(1, caster.Occupies, target.Occupies, Color.Tomato, IllustrationName.ProduceFlame);
                    await CommonSpellEffects.DealAttackRollDamage(spell, caster, target, result, _Damage, DamageKind.Fire);
                    
                    })).WithEffectOnChosenTargets((Delegates.EffectOnChosenTargets) (async (spell, caster, targets) =>
                     {
                        caster.Actions.AttackedThisManyTimesThisTurn += (spell.SpentActions - 1);
                    }
                         ));
        }));   
        
}
}
                
                
