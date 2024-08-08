using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemFlaming
{
  public static void LoadMod()
  {

    ItemName FlamingRunestone = ModManager.RegisterNewItemIntoTheShop("flaming runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "flaming runestone", 8, 500, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Fire,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}This runestone flickers with flames.{/i}\n\nYour Strikes deal an additional 1d6 fire damage. When you critically hit with a Strike, your target takes 1d10 persistent fire damage.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                    {
                      return null;
                    }
                    else return (DiceFormula.FromText("1d6", "Flaming Runestone"), DamageKind.Fire);
                  };

    qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
    {

      if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
        return;

      var Caster = QEffect.Owner;

      if (Caster.HasEffect(QEffectId.AquaticCombat))
      {
        return;
      }

      if (Target == null)
      {
        return;
      }

      if (Target.Alive == false)
      {
        return;
      }

      Target.Occupies.Overhead("Flaming Runestone", Color.Red, Caster.Name + "'s flaming runestone's critical effect activated against " + Target.Name + ".");
      Target.AddQEffect(QEffect.PersistentDamage(DiceFormula.FromText("1d10", "Flaming runestone"), DamageKind.Fire));



      return;

    };
    qfrune.Name = "Flaming Runestone";
    qfrune.Description = "Your strikes deal extra fire damage.";
    qfrune.Innate = true;
  }

        )
        );

    ItemName FlamingLesserRunestone = ModManager.RegisterNewItemIntoTheShop("flaming runestone (lesser)", itemName =>
     new Item(itemName, IllustrationName.Rock, "flaming runestone (lesser)", 3, 50, new Trait[]
    {
            Trait.Invested,
            Trait.Magical,
            Trait.Fire,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
    }).WithDescription("{i}This runestone flickers with flames.{/i}\n\nYour Strikes deal an additional 1 fire damage. When you critically hit with a Strike, your target takes 2 persistent fire damage.\n\n")
    .WithWornAt(ItemRunestone.WeaponRunestone)
    .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
    {
      qfrune.AddExtraStrikeDamage = (action, target) =>
                    {
                      if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                      {
                        return null;
                      }
                      else return (DiceFormula.FromText("1", "Flaming Runestone"), DamageKind.Fire);
                    };

      qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
      {

        if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
          return;

        var Caster = QEffect.Owner;

        if (Caster.HasEffect(QEffectId.AquaticCombat))
        {
          return;
        }

        if (Target == null)
        {
          return;
        }

        if (Target.Alive == false)
        {
          return;
        }

        Target.Occupies.Overhead("Flaming Runestone", Color.Red, Caster.Name + "'s flaming runestone's critical effect activated against " + Target.Name + ".");
        Target.AddQEffect(QEffect.PersistentDamage(DiceFormula.FromText("2", "Flaming runestone"), DamageKind.Fire));



        return;

      };
      qfrune.Name = "Flaming Runestone";
      qfrune.Description = "Your strikes deal extra fire damage.";
      qfrune.Innate = true;
    }

          )
          );

  }
}