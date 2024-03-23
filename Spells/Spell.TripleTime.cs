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



public class SpellTripleTime
{

  public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/TripleTime.png");
  public static SpellId Id;
  public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
  {
    return Spells.CreateModern((Illustration)SpellIllustration, "Triple Time", new Trait[]
        {
            Trait.Cantrip,
            Trait.Uncommon,
            Trait.Emotion,
            Trait.Enchantment,
            Trait.Mental,
            Trait.Composition
        }, "You dance at a lively tempo, speeding your allies' movement.", "You and all allies in the area gain a +10-foot status bonus to all Speeds for 1 round.", (Target)Target.Emanation(12).WithIncludeOnlyIf((target, creature) => creature.FriendOf(spellcaster)), spellLevel, null)
        .WithSoundEffect(SfxName.PositiveMelody)
        .WithActionCost(1)
        .WithGoodness((Func<Target, Creature, Creature, float>)((t, a, d) => !d.EnemyOf(a) && !a.QEffects.Any(qf => qf.Name == "Inspire Courage") ? (float)4 : 0.0f))
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
            target.AddQEffect(new QEffect("Triple Time", "You have +10-foot status bonus to all Speeds", ExpirationCondition.ExpiresAtStartOfSourcesTurn, caster, SpellIllustration)
            {
              CountsAsABuff = true,
              BonusToAllSpeeds = ((Func<QEffect, Bonus>)(_ => new Bonus(2, BonusType.Status, "Triple Time"))),

            }.WithExpirationAtStartOfSourcesTurn(caster, EffectDuration));
          }

        }));



  }

  public static void LoadMod()
  {




    Id = ModManager.RegisterNewSpell("Triple Time", 0, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>

     CombatAction(spellcaster, spellLevel, inCombat)

);

  }
}


