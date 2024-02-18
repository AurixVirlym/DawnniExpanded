using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Display.Text;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Microsoft.Xna.Framework;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Treasure;

namespace Dawnsbury.Mods.DawnniExpanded;

public class SpellScorchingRay
{
    public static void LoadMod()
    {
        ModdedIllustration illustrationScorchingRay = new ModdedIllustration("DawnniburyExpandedAssets/ScorchingRay.png");
        
        SpellId ScorchingRayid  = ModManager.RegisterNewSpell("ScorchingRay", 2, ((spellId, spellcaster, spellLevel, inCombat) =>
        {
            CreatureTarget creatureTarget = Target.Ranged(12);
            CombatAction ScorchingRaySpell = Spells.CreateModern(illustrationScorchingRay,
                "Scorching Ray",
            new[] { Trait.Fire, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait },
                    "You fire a ray of heat and flame.",
                    "Make a ranged spell attack roll against a single creature within 60ft." + S.FourDegreesOfSuccessReverse((string)null, (string)null, "The target takes 2d6 fire damage.", "Double damage.") + "\n\nFor each additional action you use when Casting the Spell, you can fire an additional ray at a different target, to a maximum of three rays targeting three different targets for 3 actions. These attacks each increase your multiple attack penalty, but you don't increase your multiple attack penalty until after you make all the spell attack rolls for scorching ray.\n\nIf you spend 2 or more actions Casting the Spell, the damage increases to 4d6 fire damage on a hit, and it still deals double damage on a critical hit." 
                    //+"\n\n{b}Heightened (+1){/b} The damage to each target increases by 1d6 for the 1-action version, or by 2d6 for the 2-action and 3-action versions."
                    ,
                    Target.DependsOnActionsSpent(
                        Target.MultipleCreatureTargets(creatureTarget).WithMustBeDistinct(),
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget).WithMustBeDistinct(),
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget, creatureTarget).WithMustBeDistinct()
                        ),
                        2,
                        null
                        ).WithActionCost(-1)
                        .WithSpellAttackRoll()
                        .WithSoundEffect(SfxName.FireRay)
                        .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
                        {
                            var _Damage = ((spellLevel) * 2) + "d6";

                            if (spell.SpentActions == 1)
                            {
                                _Damage = (2 + (spellLevel - 2)) + "d6";
                            }


                            await caster.Battle.SpawnOverairProjectileParticlesAsync(1, caster.Occupies, target.Occupies, Color.White, illustrationScorchingRay);
                            await CommonSpellEffects.DealAttackRollDamage(spell, caster, target, result, _Damage, DamageKind.Fire);

                        })).WithEffectOnChosenTargets(async (spell, caster, targets) =>
                        {
                            caster.Actions.AttackedThisManyTimesThisTurn += (spell.SpentActions - 1);
                        }
                         );
                        
             
                        return ScorchingRaySpell;

                
        }));   
        
}
}
                
                
