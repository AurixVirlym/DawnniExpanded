using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Creatures;
using System;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.StatBlocks.Description;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using System.Security.Cryptography.X509Certificates;
using System.Linq;




namespace Dawnsbury.Mods.DawnniExpanded;



public class SpellHeightenedFear
{



  static string HeightenText3rd(bool isHeightened, bool inCombat, string heightenedEffect)
  {
    if (isHeightened)
    {
      return "\n\nHeightened to spell level 3.";
    }

    if (inCombat)
    {
      return "";
    }

    return "\n\n" + heightenedEffect;
  }

  static string Heightenflavourfear(bool isHeightened)
  {
    if (isHeightened)
    {
      return "You plant fear in up to 5 targets.";
    }

    return "You plant fear in the target.";
  }

  public static CombatAction NewFear(Creature caster, int level, bool inCombat)
  {

    Target FearTargets = Target.Ranged(6);


    if (level >= 3)
    {
      FearTargets = Target.MultipleCreatureTargets(Target.Ranged(6), Target.Ranged(6), Target.Ranged(6), Target.Ranged(6), Target.Ranged(6))
        .WithMinimumTargets(1)
        .WithMustBeDistinct()
        .WithSimultaneousAnimation()
        .WithOverriddenTargetLine("up to 5 enemies", true);

    }

    CombatAction FearSpell = Spells.CreateModern(IllustrationName.Fear, "Fear", new Trait[9]
    {
            Trait.Emotion,
            Trait.Enchantment,
            Trait.Fear,
            Trait.Mental,
            Trait.Arcane,
            Trait.Divine,
            Trait.Occult,
            Trait.Primal,
            DawnniExpanded.DETrait
    },
    Heightenflavourfear(level >= 3), "The target makes a Will save.\n\n" +
     S.FourDegreesOfSuccess("The target is unaffected.", "The target is frightened 1.", "The target is frightened 2.", "The target is frightened 3 and fleeing for 1 round.") + HeightenText3rd(level >= 3, inCombat, "{b}Heightened (3rd){/b} You can target up to five creatures.")
     ,
     (Target)FearTargets,
      level,
      SpellSavingThrow.Standard(Defense.Will))
      .WithSoundEffect(SfxName.Fear)
      .WithGoodnessAgainstEnemy((Func<Target, Creature, Creature, float>)((t, a, d) =>
      level < 3 ? AICalcs.Fear(d) : a.Battle.AllCreatures.Count(cr => cr.DistanceTo(a) <= 6 && cr.EnemyOf(a)) >= 2 ? AICalcs.Fear(d) : 0))
       .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, checkResult) =>
    {
      int num;
      switch (checkResult)
      {
        case CheckResult.CriticalFailure:
          num = 3;
          break;
        case CheckResult.Failure:
          num = 2;
          break;
        case CheckResult.Success:
          num = 1;
          break;
        case CheckResult.CriticalSuccess:
          return;
        default:
          num = 0;
          break;
      }
      target.AddQEffect(QEffect.Frightened(num));
      if (checkResult != CheckResult.CriticalFailure)
        return;
      target.AddQEffect(QEffect.Fleeing(caster).WithExpirationAtStartOfSourcesTurn(caster, 1));
    }));

    return FearSpell;


  }



  public static void LoadMod()
  {


    ModManager.ReplaceExistingSpell(SpellId.Fear, 1, (spellcaster, level, inCombat, SpellInformation) => NewFear(spellcaster, level, inCombat));


  }

}




