using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Mechanics.Core;
using System;
using System.Linq;
using Dawnsbury.Display.Text;




namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellCounterPerformance
{


    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/HymnOfHealing.png");
    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
    {

        CombatAction CounterPerformance = Spells.CreateModern(SpellIllustration,
                    "Counter Performance",
                new[] {
            Trait.Uncommon,
            Trait.Bard,
            Trait.Enchantment,
            Trait.Fortune,
            Trait.Mental,
            Trait.Focus,
            Trait.Composition,
            Trait.DoesNotProvoke,
            DawnniExpanded.HomebrewTrait,
            DawnniExpanded.DETrait },
                        "Your performance protects you and your allies.",
                        "{b}Trigger{/b} You or an ally within 60 feet rolls a saving throw against a mental effect." +
                        "\n\nRoll a Performance check against a Level Based DC. You and allies in the area gain bonuses to their saving throw depending on the result."
                        + S.FourDegreesOfSuccess("+4 bonus against the triggered saving throw.", "+2 bonus against the triggered saving throw.", "+1 bonus against the triggered saving throw.", null)
                       ,
                        Target.Uncastable(),
                            spellLevel,
                            null
                            )
                            .WithActionCost(-2)
                            .WithSoundEffect(SfxName.Harp);


        CounterPerformance.WhenCombatBegins = spellcaster => spellcaster.AddQEffect(new QEffect()
        {

            StateCheck = qf =>
            {
                int BonusToSave = 0;
                bool HasUsedPerformance = false;

                QEffect CounterPerformanceEffect = new QEffect()
                {
                    ExpiresAt = ExpirationCondition.Ephemeral,
                    BeforeYourSavingThrow = async (QEffect effect, CombatAction hostilespell, Creature owner) =>
                {

                    if (!hostilespell.HasTrait(Trait.Mental) || spellcaster.Spellcasting.FocusPoints <= 0 || spellcaster.Actions.CanTakeReaction() == false || spellcaster.Actions.IsReactionUsedUp == true)
                    {
                        return;
                    }

                    if (HasUsedPerformance == false)
                    {

                        if (!await owner.Battle.AskForConfirmation(owner, SpellIllustration, "You're about to make a saving throw against " + hostilespell.Name + ".\nUse Counter Performance of " + spellcaster.Name + "?", "Use Counter Performance"))
                        {
                            return;
                        }

                        --spellcaster.Spellcasting.FocusPoints;
                        spellcaster.Actions.UseUpReaction();

                        Sfxs.Play(SfxName.Harp);

                        CheckResult lingeringresult = CommonSpellEffects.RollCheck("Counter Performance", new ActiveRollSpecification(Checks.SkillCheck(Skill.Performance), Checks.FlatDC(Bard.LevelBasedDC(spellcaster.Level))), spellcaster, spellcaster);

                        if (lingeringresult == CheckResult.CriticalSuccess)
                        {
                            BonusToSave = 4;

                        }
                        else if (lingeringresult == CheckResult.Success)
                        {
                            BonusToSave = 2;


                        }
                        else if (lingeringresult == CheckResult.Failure || lingeringresult == CheckResult.CriticalFailure)
                        {
                            BonusToSave = 1;
                        }
                        HasUsedPerformance = true;
                    }

                    effect.ExpiresAt = ExpirationCondition.Immediately;
                    owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                    {

                        BonusToDefenses = (Func<QEffect, CombatAction, Defense, Bonus>)((qf, caster, df) => new Bonus(BonusToSave, BonusType.Untyped, "Counter Performance"))
                    }
                    );

                    spellcaster.Actions.UseUpReaction();
                    return;
                },

                };

                foreach (Creature target in spellcaster.Battle.AllCreatures.Where<Creature>(cr => !cr.EnemyOf(spellcaster) && cr.DistanceTo(spellcaster) <= 12).ToList<Creature>())
                {
                    target.AddQEffect(CounterPerformanceEffect);

                }

            }


        }
            )
        ;

        return CounterPerformance;
    }




    public static void LoadMod()
    {

        Id = ModManager.RegisterNewSpell("Counter Performance", 1, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>
        CombatAction(spellcaster, spellLevel, inCombat)
    );
    }

}


