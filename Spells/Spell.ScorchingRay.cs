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
using Dawnsbury.Core.Creatures;
using System.Runtime.Serialization.Formatters;

namespace Dawnsbury.Mods.DawnniExpanded;


public class SpellScorchingRay
{
    public static ModdedIllustration Spellillustration = new ModdedIllustration("DawnniburyExpandedAssets/ScorchingRay.png");
    public static SpellId Id;
    public static CombatAction MakeScorchingRaySpell(Creature caster, int spellLevel, bool inCombat)
    {
        {
            CreatureTarget creatureTarget = Target.Ranged(12);
            CombatAction ScorchingRaySpell = Spells.CreateModern(Spellillustration,
                "Scorching Ray",
            new[] { Trait.Fire, Trait.Attack, Trait.Evocation, Trait.Arcane, Trait.Primal, DawnniExpanded.DETrait },
                    "You fire a ray of heat and flame.",
                    "Make a ranged spell attack roll against a single creature within 60ft." + S.FourDegreesOfSuccessReverse((string)null, (string)null, "The target takes " + S.HeightenedVariable(spellLevel, 3) + "d6 fire damage.", "Double damage.") + "\n\nFor each additional action you use when Casting the Spell, you can fire an additional ray at a different target, to a maximum of three rays targeting three different targets for 3 actions. These attacks each increase your multiple attack penalty, but you don't increase your multiple attack penalty until after you make all the spell attack rolls for scorching ray.\n\nIf you spend 2 or more actions Casting the Spell, the damage increases to " + S.HeightenedVariable(((spellLevel) * 2), 4) + "d6 fire damage on a hit, and it still deals double damage on a critical hit."
                    + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "\n\n{b}Heightened (+1){/b} The damage to each target increases by 1d6 for the 1-action version, or by 2d6 for the 2-action and 3-action versions.")
                    ,
                    Target.DependsOnActionsSpent(
                        Target.MultipleCreatureTargets(creatureTarget).WithMustBeDistinct(),
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget).WithMustBeDistinct(),
                        Target.MultipleCreatureTargets(creatureTarget, creatureTarget, creatureTarget).WithMustBeDistinct()
                        ),
                        spellLevel,
                        null
                        ).WithActionCost(-1)
                        .WithGoodnessAgainstEnemy(((Target t, Creature a, Creature d) => (float)t.OwnerAction.ActionCost >= 2 ? t.OwnerAction.SpellLevel * 2 * 3.5f : t.OwnerAction.SpellLevel * 3.5f))
                        .WithSpellAttackRoll()
                        .WithSoundEffect(SfxName.FireRay)
                        .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
                        {
                            string _Damage = ((spellLevel) * 2) + "d6";

                            if (spell.SpentActions == 1)
                            {
                                _Damage = spellLevel + "d6";
                            }


                            await caster.Battle.SpawnOverairProjectileParticlesAsync(1, caster.Occupies, target.Occupies, Color.White, Spellillustration);
                            await CommonSpellEffects.DealAttackRollDamage(spell, caster, target, result, _Damage, DamageKind.Fire);

                        })).WithEffectOnChosenTargets(async (spell, caster, targets) =>
                        {
                            caster.Actions.AttackedThisManyTimesThisTurn += spell.SpentActions - 1;
                        }
                         );


            return ScorchingRaySpell;


        }
    }
    public static void LoadMod()
    {


        Id = ModManager.RegisterNewSpell("ScorchingRay", 2, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) => MakeScorchingRaySpell(spellcaster, spellLevel, inCombat)
        );

    }
}


