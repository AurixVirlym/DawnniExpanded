using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemCorrosive
{
  public static void LoadMod()
  {

    ItemName CorrosiveRunestone = ModManager.RegisterNewItemIntoTheShop("corrosive runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "corrosive runestone", 8, 500, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Acid,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}Acid sizzles across the surface of this runestone.{/i}\n\nYour Strikes deal an additional 1d6 acid damage. When you critically hit with a Strike, your target takes 1d8 persistent acid damage.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    return (DiceFormula.FromText("1d6", "Corrosive Runestone"), DamageKind.Acid);
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

      Target.Occupies.Overhead("Corrosive Runestone", Color.Red, Caster.Name + "'s corrosive runestone's critical effect activated against " + Target.Name + ".");
      Target.AddQEffect(QEffect.PersistentDamage(DiceFormula.FromText("1d8", "Corrosive runestone"), DamageKind.Acid));



      return;

    };
    qfrune.Name = "Corrosive Runestone";
    qfrune.Description = "Your strikes deal extra acid damage.";
    qfrune.Innate = true;
  }

        )
        );

    ItemName CorrosiveLesserRunestone = ModManager.RegisterNewItemIntoTheShop("corrosive runestone (lesser)", itemName =>
     new Item(itemName, IllustrationName.Rock, "corrosive runestone (lesser)", 3, 50, new Trait[]
    {
            Trait.Invested,
            Trait.Magical,
            Trait.Acid,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
    }).WithDescription("{i}Acid sizzles across the surface of this runestone.{/i}\n\nYour Strikes deal an additional 1 acid damage. When you critically hit with a Strike, your target takes 1 persistent acid damage.\n\n")
    .WithWornAt(ItemRunestone.WeaponRunestone)
    .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
    {
      qfrune.AddExtraStrikeDamage = (action, target) =>
                    {
                      return (DiceFormula.FromText("1", "Corrosive Runestone"), DamageKind.Acid);
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

        Target.Occupies.Overhead("Corrosive Runestone", Color.Red, Caster.Name + "'s corrosive runestone's critical effect activated against " + Target.Name + ".");
        Target.AddQEffect(QEffect.PersistentDamage(DiceFormula.FromText("1", "Corrosive runestone"), DamageKind.Acid));



        return;

      };
      qfrune.Name = "Corrosive Runestone";
      qfrune.Description = "Your strikes deal extra acid damage.";
      qfrune.Innate = true;
    }

          )
          );

  }
}