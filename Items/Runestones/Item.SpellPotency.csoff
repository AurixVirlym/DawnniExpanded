using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemSpellPotency
{
  public static void LoadMod()
  {
    ItemName SpellpotencyRunestone = ModManager.RegisterNewItemIntoTheShop("spell potency runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "spell potency runestone", 2, 35, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}This runestone resonates in the presence of spells.{/i}\n\nYou have a +1 to spell attack rolls.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.BonusToAttackRolls = (effect, action, defense) => (action != null && action.HasTrait(Trait.Spell)) ? new Bonus(1, BonusType.Item, "Spell Potency Runestone") : null;
    qfrune.Name = "Spell Potency Runestone";
    qfrune.Description = "Your spells attacks are more accurate.";
    qfrune.Innate = true;
  }

        )
        );

    ItemName SpellFocusingRunestone = ModManager.RegisterNewItemIntoTheShop("spell focusing runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "spell focusing runestone", 4, 100, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}This runestone resonates in the presence of spells.{/i}\n\nYou have a +1 to spell DCs and attack rolls.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.BonusToAttackRolls = (effect, action, defense) => (action != null && action.HasTrait(Trait.Spell)) ? new Bonus(1, BonusType.Item, "Spell Focusing Runestone") : null;
    qfrune.BonusToSpellSaveDCs = (effect) => new Bonus(1, BonusType.Item, "Spell Focusing Runestone");
    qfrune.Name = "Spell Focusing Runestone";
    qfrune.Description = "Your spells are more potent.";
    qfrune.Innate = true;
  }

        )
        );

  }
}