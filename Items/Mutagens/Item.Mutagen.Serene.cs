using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemMutagenSerene
{

    public static void LoadMod()
    {
        ModdedIllustration illustrationSerene = new ModdedIllustration("DawnniburyExpandedAssets/SereneMutagen.png");

        ItemName SereneMutagenLesser = ModManager.RegisterNewItemIntoTheShop("Serene Mutagen (Lesser)", itemName =>
        new Item(itemName, illustrationSerene, "Serene Mutagen (Lesser)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "You gain inner serenity, focused on fine details and steeled against mental assaults, but you find violence off-putting.\n\n{b}Benefit{/b} You gain a +1 item bonus to Will saves and Perception, Medicine, Nature, Religion, and Survival checks. This bonus improves to +2 when you attempt Will saves against mental effects. \n\n{b}Drawback{/b} You take a –1 penalty to attack rolls and save DCs of offensive spells, and a –1 penalty to all weapon, unarmed attack, and spell damage.\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect SereneMutagenEffect = new QEffect("Serene Mutagen", "You are benefiting from a Serene Mutagen", ExpirationCondition.Never, self, illustrationSerene)
                {



                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                    {
                        if (attack != null && defense == Defense.Will && attack.HasTrait(Trait.Mental) == true)
                        {
                            return new Bonus(2, BonusType.Item, "Serene Mutagen");
                        }
                        else if (defense == Defense.Will)
                        {
                            return new Bonus(1, BonusType.Item, "Serene Mutagen");
                        }
                        else if (defense == Defense.Perception)
                        {
                            return new Bonus(1, BonusType.Item, "Serene Mutagen");
                        }
                        else return null;

                    },

                    BonusToAttackRolls = (qf, attack, target) =>
                    {

                        if (attack.Action.Traits.Contains(Trait.Attack))
                        {
                            return new Bonus(-1, BonusType.Item, "Serene Mutagen");
                        }
                        else if (attack.ActionId.Equals(ActionId.Seek))
                        {
                            return new Bonus(1, BonusType.Item, "Serene Mutagen");
                        }
                        else return null;
                    },

                    BonusToSpellSaveDCs = ((effect) => new Bonus(-1, BonusType.Item, "Serene Mutagen")),

                    BonusToDamage = (qf, attack, target) =>
                    {

                        return new Bonus(-1, BonusType.Item, "Serene Mutagen");
                    },

                    BonusToSkillChecks = (skill, action, defender) =>
                    {
                        if (skill == Skill.Medicine || skill == Skill.Nature || skill == Skill.Religion || skill == Skill.Survival)
                        {
                            return new Bonus(1, BonusType.Item, "Serene Mutagen");
                        }
                        else return null;
                    },
                };


                TraitMutagens.PreventMutagenDrinking(SereneMutagenEffect);
                self.AddQEffect(SereneMutagenEffect);

            }


        }

            );

        ItemName SereneMutagenModerate = ModManager.RegisterNewItemIntoTheShop("Serene Mutagen (Moderate)", itemName =>
    new Item(itemName, illustrationSerene, "Serene Mutagen (Moderate)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
    {
        Description = "You gain inner serenity, focused on fine details and steeled against mental assaults, but you find violence off-putting.\n\n{b}Benefit{/b} You gain a +2 item bonus to Will saves and Perception, Medicine, Nature, Religion, and Survival checks. This bonus improves to +3 when you attempt Will saves against mental effects. \n\n{b}Drawback{/b} You take a –1 penalty to attack rolls and save DCs of offensive spells, and a –2 penalty to all weapon, unarmed attack, and spell damage.\n\n",

        DrinkableEffect = (CombatAction ca, Creature self) =>
        {

            QEffect SereneMutagenEffect = new QEffect("Serene Mutagen", "You are benefiting from a Serene Mutagen", ExpirationCondition.Never, self, illustrationSerene)
            {



                BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                {
                    if (attack != null && defense == Defense.Will && attack.HasTrait(Trait.Mental) == true)
                    {
                        return new Bonus(3, BonusType.Item, "Serene Mutagen");
                    }
                    else if (defense == Defense.Will)
                    {
                        return new Bonus(2, BonusType.Item, "Serene Mutagen");
                    }
                    else if (defense == Defense.Perception)
                    {
                        return new Bonus(2, BonusType.Item, "Serene Mutagen");
                    }
                    else return null;

                },

                BonusToAttackRolls = (qf, attack, target) =>
                {

                    if (attack.Action.Traits.Contains(Trait.Attack))
                    {
                        return new Bonus(-1, BonusType.Item, "Serene Mutagen");
                    }
                    else if (attack.ActionId.Equals(ActionId.Seek))
                    {
                        return new Bonus(2, BonusType.Item, "Serene Mutagen");
                    }
                    else return null;
                },

                BonusToSpellSaveDCs = (effect) => new Bonus(-1, BonusType.Item, "Serene Mutagen"),

                BonusToDamage = (qf, attack, target) =>
                {

                    return new Bonus(-2, BonusType.Item, "Serene Mutagen");
                },

                BonusToSkillChecks = (skill, action, defender) =>
                {
                    if (skill == Skill.Medicine || skill == Skill.Nature || skill == Skill.Religion || skill == Skill.Survival)
                    {
                        return new Bonus(2, BonusType.Item, "Serene Mutagen");
                    }
                    else return null;
                },
            };

            TraitMutagens.PreventMutagenDrinking(SereneMutagenEffect);
            self.AddQEffect(SereneMutagenEffect);

        }


    }

        );

    }
}