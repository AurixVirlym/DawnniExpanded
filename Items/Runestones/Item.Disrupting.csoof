using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemDisrupting
{
  public static void LoadMod()
  {

    ItemName DisruptingRunestone = ModManager.RegisterNewItemIntoTheShop("disrupting runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "disrupting runestone", 5, 150, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Necromancy,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}This runestone pulses with positive energy.{/i}\n\nYour melee Strikes deal an extra 1d6 positive damage to undead. When you critically hit with a melee Strike, the undead is also enfeebled 1 until the end of your next turn.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    if (!action.HasTrait(Trait.Melee))
                    {
                      return null;
                    }
                    else return (DiceFormula.FromText("1d6", "Disrupting Runestone"), DamageKind.Positive);
                  };

    qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
    {

      if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess || !hostileAction.HasTrait(Trait.Melee))
        return;

      var Caster = QEffect.Owner;

      if (Target == null)
      {
        return;
      }

      if (Target.Alive == false || !Target.HasTrait(Trait.Undead))
      {
        return;
      }

      Target.Occupies.Overhead("Disrupting Runestone", Color.White, Caster.Name + "'s disrupting runestone's critical effect activated against " + Target.Name + ".");
      QEffect CrushingEnfeeble = QEffect.Enfeebled(1).WithExpirationAtEndOfSourcesNextTurn(Caster);
      CrushingEnfeeble.CannotExpireThisTurn = true;
      Target.AddQEffect(CrushingEnfeeble);


      return;

    };
    qfrune.Name = "Disrupting Runestone";
    qfrune.Description = "Your melee strikes deal extra postiive damage to undead.";
    qfrune.Innate = true;
  }

        )
        );

  }
}