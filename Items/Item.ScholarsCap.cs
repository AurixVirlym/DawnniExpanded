using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemScholarsHat
{
    public static void LoadMod()
    {


        Trait Headwear = ModManager.RegisterTrait(
                   "Headwear",
                   new TraitProperties("Headwear", false)
                   {
                   }
                   );

        ItemName ScholarsHat = ModManager.RegisterNewItemIntoTheShop("scholar's cap", itemName =>
           new Item(itemName, new ModdedIllustration("DawnniburyExpandedAssets/ScholarsCap.png"), "scholar's cap", 3, 60, new Trait[5]
          {
            Trait.Invested,
            Trait.Magical,
            Trait.Enchantment,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
          }).WithDescription("{i}This hat makes you feel smarter, and that's all you need to succeed sometimes.{/i}\n\nYou have a +1 item bonus on Recall Weakness check")
          .WithWornAt(Headwear)
          .WithPermanentQEffectWhenWorn((qfhat, item) =>
              qfhat.BonusToSkillChecks = (skill, action, target) => action.ActionId == FeatRecallWeakness.ActionID ? new Bonus(1, BonusType.Item, "Scholar's Cap") : (Bonus)null
                )
                );

        ItemName GreaterScholarsHat = ModManager.RegisterNewItemIntoTheShop("scholar's hat (greater)", itemName =>
            new Item(itemName, new ModdedIllustration("DawnniburyExpandedAssets/ScholarsCapGreater.png"), "scholar's cap (greater)", 9, 600, new Trait[5]
            {
                        Trait.Invested,
                        Trait.Magical,
                        Trait.Enchantment,
                        DawnniExpanded.DETrait,
                        DawnniExpanded.HomebrewTrait
            }).WithDescription("{i}This hat makes you feel REALLY smart, and that's all you need to succeed.{/i}\n\nYou have a +2 item bonus on Recall Weakness check")
            .WithWornAt(Headwear)
            .WithPermanentQEffectWhenWorn((qfhat, item) =>
                qfhat.BonusToSkillChecks = (skill, action, target) => action.ActionId == FeatRecallWeakness.ActionID ? new Bonus(2, BonusType.Item, "Scholar's Cap") : (Bonus)null
                    )
                    );
    }
}