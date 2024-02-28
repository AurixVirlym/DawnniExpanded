using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Display.Illustrations;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemMutagenBestial
{
    public static void LoadMod()
    {
        ModdedIllustration illustrationBestial = new ModdedIllustration("DawnniburyExpandedAssets/BestialMutagen.png");

        ItemName BestialMutagenLesser = ModManager.RegisterNewItemIntoTheShop("Bestial Mutagen (Lesser)", itemName =>

        new Item(itemName, illustrationBestial, "Bestial Mutagen (Lesser)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "Your features transform into something bestial and you take on muscle mass, but your lumbering form is clumsy.\n\n{b}Benefit{/b} You gain a +1 item bonus to Athletics checks and unarmed attack rolls. You gain a claw unarmed attack with the agile trait which deals 1d4 slashing damage and a jaws unarmed attack which deals 1d6 piercing damage.\n\n{b}Drawback{/b} You take a –1 penalty to AC and a –2 penalty to Reflex saves.\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect BestialMutagenEffect = new QEffect("Bestial Mutagen", "You are benefiting from a Bestial Mutagen", ExpirationCondition.Never, self, illustrationBestial)
                {

                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                        {
                            if (defense == Defense.AC)
                            {
                                return new Bonus(-1, BonusType.Item, "Bestial Mutagen");
                            }
                            else if (defense == Defense.Reflex)
                            {
                                return new Bonus(-2, BonusType.Item, "Bestial Mutagen");
                            }
                            else return null;
                        },
                    BonusToAttackRolls = (qf, attack, de) =>
                    {

                        if (attack.HasTrait(Trait.Attack) && attack.HasTrait(Trait.Unarmed))
                        {
                            return new Bonus(1, BonusType.Item, "Bestial Mutagen");
                        }
                        else return null;
                    },

                    BonusToSkills = (Skill skill) =>
                    {
                        if (skill == Skill.Athletics)
                        {
                            return new Bonus(1, BonusType.Item, "Bestial Mutagen");
                        }
                        else return null;
                    },

                    AdditionalUnarmedStrike = CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d4", DamageKind.Slashing, Trait.Agile, DawnniExpanded.DETrait),
                };

                QEffect BestialMutagenEffectbite = new QEffect("Bestial Mutagen", "You are benefiting from a Bestial Mutagen", ExpirationCondition.Never, self, IllustrationName.None)
                {
                    AdditionalUnarmedStrike = CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "jaw", "1d6", DamageKind.Bludgeoning, DawnniExpanded.DETrait),
                    DoNotShowUpOverhead = true,

                };

                TraitMutagens.PreventMutagenDrinking(BestialMutagenEffect);

                self.AddQEffect(BestialMutagenEffect);
                self.AddQEffect(BestialMutagenEffectbite);
            }


        }

        );


        ItemName BestialMutagenModerate = ModManager.RegisterNewItemIntoTheShop("Bestial Mutagen (Moderate)", itemName =>

        new Item(itemName, illustrationBestial, "Bestial Mutagen (Moderate)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "Your features transform into something bestial and you take on muscle mass, but your lumbering form is clumsy.\n\n{b}Benefit{/b} You gain a +1 item bonus to Athletics checks and unarmed attack rolls. You gain a claw unarmed attack with the agile trait which deals 1d6 slashing damage and a jaws unarmed attack which deals 1d8 piercing damage.\n\n{b}Drawback{/b} You take a –1 penalty to AC and a –2 penalty to Reflex saves.\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect BestialMutagenEffect = new QEffect("Bestial Mutagen", "You are benefiting from a Bestial Mutagen", ExpirationCondition.Never, self, illustrationBestial)
                {

                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                        {
                            if (defense == Defense.AC)
                            {
                                return new Bonus(-1, BonusType.Item, "Bestial Mutagen");
                            }
                            else if (defense == Defense.Reflex)
                            {
                                return new Bonus(-2, BonusType.Item, "Bestial Mutagen");
                            }
                            else return null;
                        },
                    BonusToAttackRolls = (qf, attack, de) =>
                    {

                        if (attack.HasTrait(Trait.Attack) && attack.HasTrait(Trait.Unarmed))
                        {
                            return new Bonus(2, BonusType.Item, "Bestial Mutagen");
                        }
                        else return null;
                    },

                    BonusToSkills = (Skill skill) =>
                    {
                        if (skill == Skill.Athletics)
                        {
                            return new Bonus(2, BonusType.Item, "Bestial Mutagen");
                        }
                        else return null;
                    },

                    AdditionalUnarmedStrike = CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claw", "1d6", DamageKind.Slashing, Trait.Agile, DawnniExpanded.DETrait),
                };

                QEffect BestialMutagenEffectbite = new QEffect("Bestial Mutagen", "You are benefiting from a Bestial Mutagen", ExpirationCondition.Never, self, IllustrationName.None)
                {
                    AdditionalUnarmedStrike = CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "jaw", "1d8", DamageKind.Bludgeoning, DawnniExpanded.DETrait),
                    DoNotShowUpOverhead = true,

                };

                TraitMutagens.PreventMutagenDrinking(BestialMutagenEffect);
                self.AddQEffect(BestialMutagenEffect);
                self.AddQEffect(BestialMutagenEffectbite);
            }


        }

        );




    }
}