using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemWounding
{
  public static void LoadMod()
  {

    ItemName WoundingRunestone = ModManager.RegisterNewItemIntoTheShop("wounding runestone", itemName =>
    new Item(itemName, IllustrationName.Rock, "wounding runestone", 7, 340, new Trait[]
   {
            Trait.Invested,
            Trait.Magical,
            Trait.Necromancy,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
   }).WithDescription("{i}Wounding runestone are said to thirst for blood.{/i}\n\nYour melee Strikes which deal piercing or slashing damage deal an extra 1d6 persistent bleed.\n\n")
   .WithWornAt(ItemRunestone.WeaponRunestone)
   .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
   {

     qfrune.AfterYouDealDamageOfKind = async (Creature Caster, CombatAction hostileAction, DamageKind DamageKind, Creature Target) =>
      {

        if (!hostileAction.HasTrait(Trait.Strike) || !hostileAction.HasTrait(Trait.Melee) || DamageKind != DamageKind.Slashing || DamageKind != DamageKind.Piercing)
          return;

        if (Target == null)
        {
          return;
        }

        if (Target.Alive == false)
        {
          return;
        }

        Target.Occupies.Overhead("Wounding Runestone", Color.Red, Caster.Name + "'s wounding runestone activated against " + Target.Name + ".");
        Target.AddQEffect(QEffect.PersistentDamage(DiceFormula.FromText("1d6", "Wounding runestone"), DamageKind.Bleed));

        return;

      };
     qfrune.Name = "Wounding Runestone";
     qfrune.Description = "Your melee slashing or piericing strikes cause bleeding.";
     qfrune.Innate = true;
   }

         )
         );

  }
}