using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemStaffoFire
{
    public static void LoadMod()
    {

        ItemName StaffofFire = ModManager.RegisterNewItemIntoTheShop("staff of fire", itemName =>
            new SpellStaff(itemName, IllustrationName.Quarterstaff, "staff of fire", 3, 60, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait, Trait.SpecificMagicWeapon, Trait.Simple, Trait.Staff, Trait.Club, Trait.WizardWeapon, Trait.TwoHanded, Trait.Evocation)
            {
                Description = "This staff resembles a blackened and burned length of ashen wood. It smells faintly of soot and glows as if lit by embers."
                + "\n\n{b}Activate{/b} Cast a Spell; {b}Effect{/b} You expend a number of charges from the staff to cast a spell from its list."
            + "\n\n{b}Cantrip{/b} Produce Flame"
            + "\n{b}1st{/b} Burning Hands"

            }
            .WithSpellTradition(Trait.Arcane)
            .WithSpellTradition(Trait.Primal)
            .AddSpelltoStaff(SpellId.ProduceFlame, 0)
            .AddSpelltoStaff(SpellId.BurningHands, 1)
            .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning))

            );

        ItemName GreaterStaffofFire = ModManager.RegisterNewItemIntoTheShop("staff of fire (greater)", itemName =>
        new SpellStaff(itemName, IllustrationName.Quarterstaff, "staff of fire (greater)", 450, 8, DawnniExpanded.DETrait, DawnniExpanded.HomebrewTrait, Trait.SpecificMagicWeapon, Trait.Simple, Trait.Staff, Trait.Club, Trait.WizardWeapon, Trait.TwoHanded, Trait.Evocation)
        {
            Description = "This staff resembles a blackened and burned length of ashen wood. It smells faintly of soot and glows as if lit by embers."
            + "\n\n{b}Activate{/b} Cast a Spell; {b}Effect{/b} You expend a number of charges from the staff to cast a spell from its list."
            + "\n\n{b}Cantrip{/b} Produce Flame"
            + "\n{b}1st{/b} Burning Hands"
            + "\n{b}2nd{/b} Burning Hands, Flaming Sphere"
            + "\n{b}3rd{/b} Flaming Sphere, Fireball"
            ,
        }
        .WithSpellTradition(Trait.Arcane)
        .WithSpellTradition(Trait.Primal)
        .AddSpelltoStaff(SpellId.ProduceFlame, 0)
        .AddSpelltoStaff(SpellId.BurningHands, 1)
        .AddSpelltoStaff(SpellId.BurningHands, 2)
        .AddSpelltoStaff(SpellId.FlamingSphere, 2)
        .AddSpelltoStaff(SpellId.FlamingSphere, 3)
        .AddSpelltoStaff(SpellId.Fireball, 3)
        .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning))

        );
    }
}