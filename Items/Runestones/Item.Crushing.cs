using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Microsoft.Xna.Framework;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemCrushing
{
  public static void LoadMod()
  {

    ItemName CrushingRunestone = ModManager.RegisterNewItemIntoTheShop("crushing runestone", itemName =>
       new Item(itemName, IllustrationName.Rock, "crushing runestone", 3, 50, new Trait[]
      {
            Trait.Invested,
            Trait.Magical,
            Trait.Necromancy,
            Trait.Uncommon,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
      }).WithDescription("{i}This runestone empowers your strength, causing your attacks to leave your foe staggered.{/i}\n\nWhen you critically hit with a Strike that deals bludgeoning damage, your target becomes clumsy 1 and enfeebled 1 until the end of your next turn.\n\n")
      .WithWornAt(ItemRunestone.WeaponRunestone)
      .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
      {
        qfrune.AfterYouDealDamageOfKind = async (Creature Caster, CombatAction hostileAction, DamageKind DamageKind, Creature Target) =>
        {

          if (!hostileAction.HasTrait(Trait.Strike) || hostileAction.CheckResult != CheckResult.CriticalSuccess || DamageKind != DamageKind.Bludgeoning)
            return;

          if (Target == null)
          {
            return;
          }

          if (Target.Alive == false)
          {
            return;
          }

          Target.Occupies.Overhead("Crushing Runestone", Color.White, Caster.Name + "'s crushing runestone's critical effect activated against " + Target.Name + ".");
          QEffect CrushingEnfeeble = QEffect.Enfeebled(1).WithExpirationAtEndOfSourcesNextTurn(Caster);
          CrushingEnfeeble.CannotExpireThisTurn = true;
          QEffect CrushingClumsy = QEffect.Clumsy(1).WithExpirationAtEndOfSourcesNextTurn(Caster);
          CrushingClumsy.CannotExpireThisTurn = true;
          Target.AddQEffect(CrushingEnfeeble);
          Target.AddQEffect(CrushingClumsy);

          return;

        };

        qfrune.Description = "Your crtical blunt strikes leave your foes staggered.";

      }

            )
            );

    ItemName CrushingGreaterRunestone = ModManager.RegisterNewItemIntoTheShop("crushing runestone (greater)", itemName =>
         new Item(itemName, IllustrationName.Rock, "crushing runestone (greater)", 9, 650, new Trait[]
        {
            Trait.Invested,
            Trait.Magical,
            Trait.Necromancy,
            Trait.Uncommon,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
        }).WithDescription("{i}This runestone immensely empowers your strength, causing your attacks to leave your foe staggered.{/i}\n\nWhen you critically hit with a Strike that deals bludgeoning damage, your target becomes clumsy 2 and enfeebled 2 until the end of your next turn.\n\n")
        .WithWornAt(ItemRunestone.WeaponRunestone)
        .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
        {
          qfrune.AfterYouDealDamageOfKind = async (Creature Caster, CombatAction hostileAction, DamageKind DamageKind, Creature Target) =>
          {

            if (!hostileAction.HasTrait(Trait.Strike) || hostileAction.CheckResult != CheckResult.CriticalSuccess || DamageKind != DamageKind.Bludgeoning)
              return;

            if (Target == null)
            {
              return;
            }

            if (Target.Alive == false)
            {
              return;
            }

            Target.Occupies.Overhead("Crushing Runestone", Color.White, Caster.Name + "'s crushing runestone (greater)' critical effect activated against " + Target.Name + ".");
            QEffect CrushingEnfeeble = QEffect.Enfeebled(2).WithExpirationAtEndOfSourcesNextTurn(Caster);
            CrushingEnfeeble.CannotExpireThisTurn = true;
            QEffect CrushingClumsy = QEffect.Clumsy(2).WithExpirationAtEndOfSourcesNextTurn(Caster);
            CrushingClumsy.CannotExpireThisTurn = true;
            Target.AddQEffect(CrushingEnfeeble);
            Target.AddQEffect(CrushingClumsy);

            return;

          };
          qfrune.Name = "Crushing Runestone";
          qfrune.Description = "Your crtical blunt strikes leave your foes staggered.";
          qfrune.Innate = true;
        }

              )
              );

  }
}