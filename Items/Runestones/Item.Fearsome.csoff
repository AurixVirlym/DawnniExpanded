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

public static class ItemFearsome
{
  public static void LoadMod()
  {

    ItemName FearsomeRune = ModManager.RegisterNewItemIntoTheShop("fearsome runestone", itemName =>
       new Item(itemName, IllustrationName.Rock, "fearsome runestone", 5, 160, new Trait[]
      {
            Trait.Invested,
            Trait.Magical,
            Trait.Emotion,
            Trait.Fear,
            Trait.Mental,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
      }).WithDescription("{i}This runestone glows with an errie purple glow.{/i}\n\nWhen you critically hit with a Strike, your target becomes frightened 1.\n\n")
      .WithWornAt(ItemRunestone.WeaponRunestone)
      .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
      {
        qfrune.AfterYouTakeAction = async (QEffect qf, CombatAction hostileAction) =>
        {

          if (!hostileAction.HasTrait(Trait.Strike) || hostileAction.CheckResult != CheckResult.CriticalSuccess)
            return;

          Creature Target = hostileAction.ChosenTargets.ChosenCreature;
          if (Target == null)
          {
            return;
          }

          if (Target.Alive == false)
          {
            return;
          }

          if (Target.IsImmuneTo(Trait.Emotion) || Target.IsImmuneTo(Trait.Fear) || Target.IsImmuneTo(Trait.Mental))
          {
            return;
          }

          Target.Occupies.Overhead("Fearsome Runestone", Color.Purple, qf.Owner.Name + "'s fearsome runestone's critical effect activated against " + Target.Name + ".");
          Target.AddQEffect(QEffect.Frightened(1));

          return;

        };
        qfrune.Name = "Fearsome Runestone";
        qfrune.Description = "Your crtical strikes frightened foes.";
        qfrune.Innate = true;
      }

            )
            );

  }
}