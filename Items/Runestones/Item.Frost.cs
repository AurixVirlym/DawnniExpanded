using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using System;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemFrost
{
  public static void LoadMod()
  {

    ItemName FrostRunestone = ModManager.RegisterNewItemIntoTheShop("frost runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "frost runestone", 8, 500, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Cold,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}This runestone is freezing to touch.{/i}\n\nYour Strikes deal an additional 1d6 cold damage. When you critically hit with a Strike, your target must succeed at a DC 24 Fortitude save or be slowed 1 until the end of your next turn.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                    {
                      return null;
                    }
                    else return (DiceFormula.FromText("1d6", "Frost Runestone"), DamageKind.Cold);
                  };

    qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
    {

      if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
        return;

      var Caster = QEffect.Owner;

      if (Target == null)
      {
        return;
      }

      if (Target.Alive == false)
      {
        return;
      }

      Target.Occupies.Overhead("Frost Runestone", Color.Yellow, Caster.Name + "'s frost runestone's critical effect activated against " + Target.Name + ".");

      CombatAction simple = CombatAction.CreateSimple(Caster, "Frost Runestone Critical Effect");
      simple.SpellLevel = Caster.MaximumSpellRank;
      simple.Traits.Add(Trait.Magical);
      simple.Traits.Add(Trait.Cold);
      simple.WithActionCost(0);
      CheckResult checkResult = CommonSpellEffects.RollSavingThrow(Target, simple, Defense.Fortitude, (Func<Creature, int>)(caster => 24));
      if (checkResult <= CheckResult.Failure)
      {
        QEffect FrostSlow = QEffect.Slowed(1).WithExpirationAtEndOfSourcesNextTurn(Caster);
        FrostSlow.CannotExpireThisTurn = true;
        Target.AddQEffect(FrostSlow);
      }

      return;

    };
    qfrune.Name = "Frost Runestone";
    qfrune.Description = "Your strikes deal extra cold damage.";
    qfrune.Innate = true;
  }

        )
        );

    ItemName FrostLesserRunestone = ModManager.RegisterNewItemIntoTheShop("frost runestone (lesser)", itemName =>
     new Item(itemName, IllustrationName.Rock, "frost runestone (lesser)", 3, 50, new Trait[]
    {
            Trait.Invested,
            Trait.Magical,
            Trait.Cold,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
    }).WithDescription("{i}This runestone is freezing to touch.{/i}\n\nYour Strikes deal an additional 1 cold damage. When you critically hit with a Strike, your target must succeed at a DC 18 Fortitude save or be slowed 1 until the end of your next turn.\n\n")
    .WithWornAt(ItemRunestone.WeaponRunestone)
    .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
    {
      qfrune.AddExtraStrikeDamage = (action, target) =>
                    {
                      if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                      {
                        return null;
                      }
                      else return (DiceFormula.FromText("1", "Frost Runestone"), DamageKind.Cold);
                    };

      qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
      {

        if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
          return;

        var Caster = QEffect.Owner;

        if (Target == null)
        {
          return;
        }

        if (Target.Alive == false)
        {
          return;
        }

        Target.Occupies.Overhead("Frost Runestone", Color.Yellow, Caster.Name + "'s frost runestone's critical effect activated against " + Target.Name + ".");

        CombatAction simple = CombatAction.CreateSimple(Caster, "Frost Runestone Critical Effect");
        simple.SpellLevel = Caster.MaximumSpellRank;
        simple.Traits.Add(Trait.Magical);
        simple.Traits.Add(Trait.Cold);
        simple.WithActionCost(0);
        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(Target, simple, Defense.Fortitude, (Func<Creature, int>)(caster => 18));
        if (checkResult <= CheckResult.Failure)
        {
          QEffect FrostSlow = QEffect.Slowed(1).WithExpirationAtEndOfSourcesNextTurn(Caster);
          FrostSlow.CannotExpireThisTurn = true;
          Target.AddQEffect(FrostSlow);
        }

        return;

      };
      qfrune.Name = "Frost Runestone";
      qfrune.Description = "Your strikes deal extra cold damage.";
      qfrune.Innate = true;
    }

          )
          );

  }
}