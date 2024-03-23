using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemStaffofSpellPotency
{
    public static void LoadMod()
    {

        ItemName StaffofPotency = ModManager.RegisterNewItemIntoTheShop("Staff Of Spell Potency +1", itemName =>
            new Item(itemName, (Illustration)IllustrationName.Quarterstaff, "Staff Of Spell Potency +1", 2, 35, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait, Trait.SpecificMagicWeapon, Trait.Simple, Trait.Club, Trait.WizardWeapon)
            {
                Description = "While you hold the {i}Staff Of Spell Potency +1{/i}, you have a +1 to spell attack rolls.",
            }.WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning))
            );

        ModManager.RegisterActionOnEachCreature(creature =>
        {
            // We add an effect to every single creature...
            creature.AddQEffect(
                new QEffect()
                {
                    StateCheck = (qf) =>
                    {
                        var StaffofPotencyHolder = qf.Owner;
                        if (StaffofPotencyHolder.HeldItems.Any(heldItem => heldItem.ItemName == StaffofPotency))
                        {
                            // ...and that creature is currently holding an apple of power, we give it another ephemeral effect (which will expire at state-check so if the creature stops holding an apple of power,
                            // it will be lost during the next state-check):
                            StaffofPotencyHolder.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                BonusToAttackRolls = (effect, action, defense) => (action != null && action.HasTrait(Trait.Spell)) ? new Bonus(1, BonusType.Item, "Staff Of Spell Potency") : null

                            });
                        }
                    }
                });
        });

        ItemName StaffofPotencyfocusing = ModManager.RegisterNewItemIntoTheShop("Staff Of Spell Potency +1 Focusing", itemName =>
            new Item(itemName, (Illustration)IllustrationName.Quarterstaff, "Staff Of Spell Potency +1 Focusing", 4, 100, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait, Trait.SpecificMagicWeapon, Trait.Simple, Trait.Club, Trait.WizardWeapon)
            {
                Description = "While you hold the {i}Staff Of Spell Potency +1 Focusing{/i}, you have a +1 to spell DCs and attack rolls.",
            }.WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning))
);

        ModManager.RegisterActionOnEachCreature(creature =>
        {
            // We add an effect to every single creature...
            creature.AddQEffect(
                new QEffect()
                {
                    StateCheck = (qf) =>
                    {
                        var StaffofPotencyHolder = qf.Owner;
                        if (StaffofPotencyHolder.HeldItems.Any(heldItem => heldItem.ItemName == StaffofPotencyfocusing))
                        {
                            // ...and that creature is currently holding an apple of power, we give it another ephemeral effect (which will expire at state-check so if the creature stops holding an apple of power,
                            // it will be lost during the next state-check):
                            StaffofPotencyHolder.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                BonusToAttackRolls = ((effect, action, defense) => (action != null && action.HasTrait(Trait.Spell)) ? new Bonus(1, BonusType.Item, "Staff Of Spell Potency") : null),
                                BonusToSpellSaveDCs = ((effect) => new Bonus(1, BonusType.Item, "Staff Of Spell Potency"))
                            });
                        }
                    }
                });
        });

    }
}