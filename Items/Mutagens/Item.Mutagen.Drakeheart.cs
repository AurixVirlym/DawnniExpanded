using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemMutagenDrakeheart
{

    public static void LoadMod()
    {
        ModdedIllustration illustrationDrakeheart = new ModdedIllustration("DawnniburyExpandedAssets/DrakeheartMutagen.png");
        ModdedIllustration illustrationFinalSurge = new ModdedIllustration("DawnniburyExpandedAssets/DrakeheartMutagen.png");

        ItemName DrakeheartMutagenLesser = ModManager.RegisterNewItemIntoTheShop("Drakeheart Mutagen (Lesser)", itemName =>

        new Item(itemName, illustrationDrakeheart, "Drakeheart Mutagen (Lesser)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "Your skin grows tough scales like a drake, your eyesight become sharp and your pupils slitted, and your limbs grow wiry and quick, but your mind and reflexes become slow. \n\n{b}Benefit{/b} You gain a +4 bonus to AC, a Dexterity cap of +2 (as usual, use your lowest Dexterity cap if you have more than one), and a +1 item bonus to Perception checks. If you're wearing armor, you still calculate your proficiency bonus to AC based on your proficiency in the armor you're wearing, even if the drakeheart mutagen has a higher item bonus. \n\nYou also gain the Final Surge action.\nActivate {icon:Action} {b}Final Surge{/b}\n{b}Effect{/b} You Stride twice. The drakeheart mutagen's duration ends.\n\n{b}Drawback{/b} You take a –1 penalty to Will saves and Reflex saves.\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                Item obj = new Item(illustrationDrakeheart, "Drakeheart Scales", new Trait[6]
                {
                    Trait.LightArmor,
                    Trait.MediumArmor,
                    Trait.HeavyArmor,
                    Trait.UnarmoredDefense,
                    Trait.Armor,
                    DawnniExpanded.DETrait
                }).WithArmorProperties(new ArmorProperties(4, 2, 0, 0, 0));

                QEffect DrakeHeartEffect = new QEffect("Drakeheart Mutagen", "You are benefiting from a Drakeheart Mutagen.", ExpirationCondition.Never, self, illustrationDrakeheart)
                {


                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                        {
                            if (defense == Defense.Will || defense == Defense.Reflex)
                            {
                                return new Bonus(-1, BonusType.Item, "Drakeheart Mutagen");
                            }
                            else if (defense == Defense.Perception)
                            {
                                return new Bonus(1, BonusType.Item, "Drakeheart Mutagen");
                            }
                            return null;
                        },

                    BonusToAttackRolls = (qf, attack, de) =>
                    {

                        if (attack.ActionId.Equals(ActionId.Seek))
                        {
                            return new Bonus(1, BonusType.Item, "Drakeheart Mutagen");
                        }
                        else return null;
                    },


                    ProvidesArmor = obj,
                    ProvideContextualAction = qfSelf => new ActionPossibility(new CombatAction(qfSelf.Owner, illustrationFinalSurge, "Final Surge", new Trait[2]
                        {
                        Trait.Move, DawnniExpanded.DETrait
                }, "Stride twice. This ends the Drakeheart Mutagen.", Target.Self()).WithActionCost(1).WithSoundEffect(SfxName.Footsteps).WithEffectOnSelf(async (CombatAction action, Creature self) =>
                {
                    if (!await self.StrideAsync("Choose where to Stride with Final Surge. (1/2)", allowCancel: true))
                    {
                        action.RevertRequested = true;
                    }
                    else
                    {
                        int num = await self.StrideAsync("Choose where to Stride with Final Surge. (2/2)", allowPass: true) ? 1 : 0;
                        qfSelf.ExpiresAt = ExpirationCondition.Immediately;
                    }
                }
                        )
                        )
                };

                TraitMutagens.PreventMutagenDrinking(DrakeHeartEffect);
                self.AddQEffect(DrakeHeartEffect);
                Sfxs.Play(SfxName.PotionUse2);
            }


        }

        );

        ItemName DrakeheartMutagenModerate = ModManager.RegisterNewItemIntoTheShop("Drakeheart Mutagen (Moderate)", itemName =>

        new Item(itemName, illustrationDrakeheart, "Drakeheart Mutagen (Moderate)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "Your skin grows tough scales like a drake, your eyesight become sharp and your pupils slitted, and your limbs grow wiry and quick, but your mind and reflexes become slow. \n\n{b}Benefit{/b} You gain a +5 bonus to AC, a Dexterity cap of +2 (as usual, use your lowest Dexterity cap if you have more than one), and a +2 item bonus to Perception checks. If you're wearing armor, you still calculate your proficiency bonus to AC based on your proficiency in the armor you're wearing, even if the drakeheart mutagen has a higher item bonus. \n\nYou also gain the Final Surge action.\nActivate {icon:Action} {b}Final Surge{/b}\n{b}Effect{/b} You Stride twice. The drakeheart mutagen's duration ends.\n\n{b}Drawback{/b} You take a –1 penalty to Will saves and Reflex saves.\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                Item obj = new Item((Illustration)illustrationDrakeheart, "Drakeheart Scales", new Trait[6]
                {
                    Trait.LightArmor,
                    Trait.MediumArmor,
                    Trait.HeavyArmor,
                    Trait.UnarmoredDefense,
                    Trait.Armor,
                    DawnniExpanded.DETrait
                }).WithArmorProperties(new ArmorProperties(5, 2, 0, 0, 0));

                QEffect DrakeHeartEffect = new QEffect("Drakeheart Mutagen", "You are benefiting from a Drakeheart Mutagen", ExpirationCondition.Never, self, illustrationDrakeheart)
                {

                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                        {
                            if (defense == Defense.Will || defense == Defense.Reflex)
                            {
                                return new Bonus(-1, BonusType.Item, "Drakeheart Mutagen");
                            }
                            else if (defense == Defense.Perception)
                            {
                                return new Bonus(2, BonusType.Item, "Drakeheart Mutagen");
                            }
                            return null;
                        },

                    BonusToAttackRolls = (qf, attack, de) =>
                    {

                        if (attack.ActionId.Equals(ActionId.Seek))
                        {
                            return new Bonus(2, BonusType.Item, "Drakeheart Mutagen");
                        }
                        else return null;
                    },

                    ProvidesArmor = obj,
                    ProvideContextualAction = qfSelf => new ActionPossibility(new CombatAction(qfSelf.Owner, illustrationFinalSurge, "Final Surge", new Trait[1]
                        {
                        Trait.Move
                }, "Stride twice. This ends the Drakeheart Mutagen.", Target.Self()).WithActionCost(1).WithSoundEffect(SfxName.Footsteps).WithEffectOnSelf(async (CombatAction action, Creature self) =>
                {
                    if (!await self.StrideAsync("Choose where to Stride with Final Surge. (1/2)", allowCancel: true))
                    {
                        action.RevertRequested = true;
                    }
                    else
                    {
                        int num = await self.StrideAsync("Choose where to Stride with Final Surge. (2/2)", allowPass: true) ? 1 : 0;
                        qfSelf.ExpiresAt = ExpirationCondition.Immediately;
                    }
                }
                        )
                        )
                };

                TraitMutagens.PreventMutagenDrinking(DrakeHeartEffect);
                self.AddQEffect(DrakeHeartEffect);

            }


        });

    }
}