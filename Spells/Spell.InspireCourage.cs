using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.Creatures;
using System;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using System.Linq;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;





namespace Dawnsbury.Mods.DawnniExpanded;



public class SpellInspireCourage
{

  public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/InspireCourage.png");
  public static SpellId Id;
  public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
  {
    return Spells.CreateModern((Illustration)SpellIllustration, "Inspire Courage", new Trait[]
        {
            Trait.Cantrip,
            Trait.Uncommon,
            Trait.Emotion,
            Trait.Enchantment,
            Trait.Mental,
            Trait.Composition
        }, "You inspire your allies with words or tunes of encouragement.", "You and all allies in the area gain a +1 status bonus to attack rolls, damage rolls, and saves against fear effects for 1 round.", (Target)Target.Emanation(12).WithIncludeOnlyIf((target, creature) => creature.FriendOf(spellcaster)), spellLevel, null)
        .WithSoundEffect(SfxName.PositiveMelody)
        .WithActionCost(1)
        .WithGoodness((Func<Target, Creature, Creature, float>)((t, a, d) => !d.EnemyOf(a) && !a.QEffects.Any(qf => qf.Name == "Inspire Courage") ? (float)AICalcs.AttackBonusGoodnessForNPCs(a.Level, 1, 1) * 2 : 0.0f))
        .WithEffectOnChosenTargets((Delegates.EffectOnChosenTargets)(async (spell, caster, chosenTargets) =>
        {


          int EffectDuration = 1;

          if (spell.Name.Contains("Lingering Composition"))
          {
            CheckResult lingeringresult = CommonSpellEffects.RollCheck("Lingering Composition", new ActiveRollSpecification(Checks.SkillCheck(Skill.Performance), Checks.FlatDC(Bard.LevelBasedDC(caster.Level))), caster, caster);

            if (lingeringresult == CheckResult.CriticalSuccess)
            {
              EffectDuration = 4;

            }
            else if (lingeringresult == CheckResult.Success)
            {
              EffectDuration = 3;


            }
            else if (lingeringresult == CheckResult.Failure || lingeringresult == CheckResult.CriticalFailure)
            {
              EffectDuration = 1;
              caster.Spellcasting.FocusPoints += 1;
            }

          }

          caster.AddQEffect(new QEffect()
          {
            PreventTakingAction = (Func<CombatAction, string>)(ca => !ca.HasTrait(Trait.Composition) ? (string)null : "You may only cast one composition spell a turn."),
            ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn
          });

          foreach (Creature target in chosenTargets.ChosenCreatures)
          {
            if (target.EnemyOf(caster))
            {
              continue;
            }
            target.AddQEffect(new QEffect("Inspire Courage", "You have +1 status bonus to attack rolls, damage rolls and saves against fear effects.", ExpirationCondition.ExpiresAtStartOfSourcesTurn, caster, SpellIllustration)
            {

              BonusToAttackRolls = (((effect, action, arg3) => !action.HasTrait(Trait.Attack) ? (Bonus)null : new Bonus(1, BonusType.Status, "Inspire Courage"))),
              BonusToDamage = (((effect, action, arg3) => new Bonus(1, BonusType.Status, "Inspire Courage"))),
              BonusToDefenses = (((effect, action, defense) => !defense.IsSavingThrow() || action == null || !action.HasTrait(Trait.Fear) ? (Bonus)null : new Bonus(1, BonusType.Status, "Inspire Courage"))),
              CountsAsABuff = true,
            }.WithExpirationAtStartOfSourcesTurn(caster, EffectDuration));
          }

        }));



  }

  public static void LoadMod()
  {




    Id = ModManager.RegisterNewSpell("Inspire Courage", 0, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>

     CombatAction(spellcaster, spellLevel, inCombat)

);

  }
}


