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
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Display;

namespace Dawnsbury.Mods.DawnniExpanded;


public class SpellRayofEnfeeblement
{
    public static ModdedIllustration Spellillustration = new ModdedIllustration("DawnniburyExpandedAssets/RayofEnfeeblement.png");
    public static SpellId Id;
    public static CombatAction MakeRayofEnfeeblementSpell(Creature caster, int spellLevel, bool inCombat)
    {
        {
            CombatAction RayofEnfeeblementSpell = Spells.CreateModern(Spellillustration,
                "Ray of Enfeeblement",
            new[] { Trait.Attack, Trait.Necromancy, Trait.Arcane, Trait.Divine, Trait.Occult, DawnniExpanded.DETrait },
                    "A ray with the power to sap a foe's strength flashes from your hand.",
                    "Attempt a ranged spell attack against the target. If you succeed, that creature attempts a Fortitude save in order to determine the spell's effect. If you critically succeed on your attack roll, use the outcome for one degree of success worse than the result of its save." + S.FourDegreesOfSuccess(
                    "The target is unaffected.",
                    "The target becomes enfeebled 1.",
                    "The target becomes enfeebled 2.",
                    "The target becomes enfeebled 3."),
                    Target.Ranged(6),
                        spellLevel,
                        null
                        ).WithActionCost(2)
                        .WithGoodnessAgainstEnemy((Target t, Creature a, Creature d) => (float)d.Abilities.Strength >= 3 ? AICalcs.AttackBonusGoodnessForNPCs(d.Level, 2, 2, true) : 0f)
                        .WithSpellAttackRoll()
                        .WithSoundEffect(SfxName.Fear)
                        .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (CombatAction spell, Creature caster, Creature target, CheckResult result) =>
                        {
                            CheckResult checkResult = CommonSpellEffects.RollSpellSavingThrow(target, spell, Defense.Fortitude);

                            if (result <= CheckResult.Failure)
                            {
                                return;
                            };

                            if (result >= CheckResult.CriticalSuccess && checkResult != CheckResult.CriticalFailure)
                            {
                                checkResult = checkResult.WorsenByOneStep();
                                target.Occupies.Overhead("Critical hit reduces save result!!", Color.Gold, caster.Name + "'s critical hit reduces the saving throw result of " + target.Name + " by one step to a " + checkResult.HumanizeTitleCase2() + ".");
                            };

                            if (checkResult == CheckResult.Success)
                            {
                                target.AddQEffect(QEffect.Enfeebled(1));
                            }
                            else if (checkResult == CheckResult.Failure)
                            {
                                target.AddQEffect(QEffect.Enfeebled(2));
                            }
                            if (checkResult == CheckResult.CriticalFailure)
                            {
                                target.AddQEffect(QEffect.Enfeebled(3));
                            }

                        }));


            return RayofEnfeeblementSpell;


        }
    }
    public static void LoadMod()
    {


        Id = ModManager.RegisterNewSpell("RayofEnfeeblement", 1, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) => MakeRayofEnfeeblementSpell(spellcaster, spellLevel, inCombat)
        );

    }
}


