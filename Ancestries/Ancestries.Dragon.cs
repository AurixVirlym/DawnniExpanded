using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core;
using Dawnsbury.Core.Roller;
using Dawnsbury.Audio;
using Dawnsbury.Display.Illustrations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.ThirdParty.SteamApi;
using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{

    public class DragonAncestryFeat : TrueFeat
    {
        public DragonAncestryFeat(string name, string flavorText, string rulesText)
            : base(FeatName.CustomFeat, 1, flavorText, rulesText, new[] { Trait.Ancestry, AncestryDragon.ancestryTrait, DawnniExpanded.DETrait })
        {
            this
                .WithCustomName(name)
                .WithPrerequisite(sheet => sheet.Ancestries.Contains(AncestryDragon.ancestryTrait), "You must be a Dragon.");
        }
    }

    public class AncestryDragon
    {

        public static int GetClassOrSpellDC(Creature caster)
        {

            int ClassDC = caster.Proficiencies.Get(caster.PersistentCharacterSheet.Class.ClassTrait).ToNumber(caster.ProficiencyLevel) + caster.Abilities.Get(caster.Abilities.KeyAbility) + 10;

            List<int> SpellDCs = new List<int>();


            if (caster.Spellcasting != null)
            {
                foreach (SpellcastingSource Spellsource in caster.Spellcasting.Sources)
                {
                    SpellDCs.Add(Spellsource.GetSpellSaveDC());
                }
            }



            SpellDCs.Add(ClassDC);

            return SpellDCs.Max();
        }

        public static QEffect AncestryBreathWeapon(
         string appearance,
         Target target,
         Defense savingThrow,
         DamageKind damageKind,
         DiceFormula damage,
         SfxName soundEffect)
        {

            return new QEffect()
            {
                ProvideMainAction = (Func<QEffect, Possibility>)(qf =>
                (Possibility)new ActionPossibility(new CombatAction(qf.Owner, (Illustration)IllustrationName.BreathWeapon, "Breath Weapon (Ancestry)", new Trait[3]
                {
        Trait.Dragon,
        Trait.Evocation,
        DamageKindExtensions.DamageKindToTrait(damageKind)
                }, target.ToDescription() + "\n\nYou exhale a draconic breath and deal " + damage.ToString() + " " + EnumHumanizeExtensions.Humanize((Enum)damageKind).ToLower() + " damage  to each target (basic DC " + GetClassOrSpellDC(qf.Owner).ToString() + " " + savingThrow.ToString() + " save mitigates)." + "You use this ability once per encounter or if you are level 4 or above you instead can't use Breath Weapon again for 1d4 rounds.", target)
                {
                    ShortDescription = ("You exhale " + appearance + " and deal " + damage.ToString() + " damage to each target (basic DC " + GetClassOrSpellDC(qf.Owner).ToString() + " " + savingThrow.ToString() + " save mitigates)." + "You use this ability once per encounter or if you are level 4 or above you instead can't use Breath Weapon again for 1d4 rounds.")
                }.WithActionCost(2)
                .WithProjectileCone((Illustration)IllustrationName.BreathWeapon, 25, ProjectileKind.Cone)
                .WithSavingThrow(new SavingThrow(savingThrow, (Func<Creature, int>)(_ => GetClassOrSpellDC(qf.Owner))))
                .WithGoodnessAgainstEnemy((Func<Target, Creature, Creature, float>)((tg, a, d) => damage.ExpectedValue))
                .WithSoundEffect(soundEffect)
                .WithEffectOnEachTarget(async (spell, caster, defender, result) =>
                await CommonSpellEffects.DealBasicDamage(spell, caster, defender, result, damage, damageKind))
                .WithEffectOnChosenTargets(async (a, d) =>
                {

                    if (a.Level >= 4)
                    {
                        a.AddQEffect(QEffect.CannotUseForXRound("Breath Weapon (Ancestry)", a, DiceFormula.FromText("1d4").Roll().Item1 + 1));
                    }
                    else
                    {
                        a.AddQEffect(new QEffect() { PreventTakingAction = newAttack => newAttack.Name == "Breath Weapon (Ancestry)" ? "You already used your breath this encounter" : null });
                    }

                }
                )))

            };
        }
        public static Trait ancestryTrait = Trait.Dragon;
        public static Feat AncestryFeat;

        public static Feat DragonResistance = new DragonAncestryFeat("Draconic Resistances", "You have revitalized the magical pathways that protect your body from the type of damage you would normally use for your breath weapon.", "You gain resistance equal to half your level (minimum 1) to your heritage's associated damage type.").WithOnCreature((creature =>
                    {
                        if (creature.PersistentCharacterSheet.Heritage == null || creature.PersistentCharacterSheet.Heritage is not DragonHertiageFeat)
                        {
                            return;
                        }

                        DragonHertiageFeat feat = (DragonHertiageFeat)creature.PersistentCharacterSheet.Heritage;

                        creature.AddQEffect(QEffect.DamageResistance(feat.AssociatedDamage, Math.Max(creature.Level / 2, 1)));
                    }
                    ));

        public static Feat DragonExtraUnarmedAttack;

        public static Feat DragonBreath;
        public static Feat DraconicCantrip;
        public static void LoadMod()
        {

            AncestryFeat = new AncestrySelectionFeat(
                        FeatName.CustomFeat,
                        "Ancient beyond measure and mighty as legend, dragons awe, frighten, and inspire other ancestries the world over. To some cultures, dragons are the very symbol of power. To others, the heralds of rulership. But to dragons, it is simply who they are. \n\nThis simple truth colors the relationship between dragons and other ancestries at a fundamental level. But not all dragons live like those in the pages of human storybooks, sleeping in a cavern full of treasure until some foolhardy adventurer dares to challenge them for their hoard. \n\nSometimes the adventurer is the dragon!\n\nInstead of a fist unarmed attack, you have a jaws unarmed attack that deals 1d6 piercing damage and a claw unarmed attack that deals 1d4 slashing damage and has the agile and finesse traits. Both unarmed attacks are in the brawling weapon group.",
                        new List<Trait> {Trait.Dragon
        },
                        8,
                        5,
                        new List<AbilityBoost>()
                        {
                    new EnforcedAbilityBoost(Ability.Strength),
                    new FreeAbilityBoost()
                        },
                        DragonHertiages.LoadFeats().ToList())
                    .WithCustomName("Dragon")

                    .WithOnSheet(sheet =>
                    {

                    })
                    .WithOnCreature(creature =>
                    {
                        creature.WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d6", DamageKind.Piercing));
                        creature.WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.DragonClaws, "claws", "1d4", DamageKind.Slashing, Trait.Agile, Trait.Finesse));
                    });


            DragonBreath = new DragonAncestryFeat("Dragon Breath", "You breathe in deeply and release the energy stored within you in a powerful exhalation.", "Your dragon breath is a 30-foot line, a 15-foot cone, or a 5-foot burst within 30 feet, depending on your heritage, and deals 2d4 damage of a type depending on your heritage. \n\nEach creature in the area must attempt a basic Reflex saving throw against the higher of your class DC or spell DC.\n\nIf the damage type is poison, the saving throw is Fortitude, and if the  damage type is mental, the saving throw is Will. \n\nYou can't use this ability again for 10 minutes; starting at level 3, you instead can't use the ability again for 1d4 rounds.").WithOnCreature((creature =>
                    {
                        if (creature.PersistentCharacterSheet.Heritage == null || creature.PersistentCharacterSheet.Heritage is not DragonHertiageFeat || creature.PersistentCharacterSheet.Class.ClassTrait == null)
                        {
                            return;
                        }

                        DragonHertiageFeat hertiage = (DragonHertiageFeat)creature.PersistentCharacterSheet.Heritage;



                        Defense BreathSavingThrow = Defense.Reflex;
                        if (hertiage.AssociatedDamage == DamageKind.Mental)
                        {
                            BreathSavingThrow = Defense.Will;
                        }
                        else if (hertiage.AssociatedDamage == DamageKind.Poison)
                        {
                            BreathSavingThrow = Defense.Fortitude;
                        }


                        creature.AddQEffect(AncestryBreathWeapon("", hertiage.BreathTargeting, BreathSavingThrow, hertiage.AssociatedDamage, DiceFormula.FromText("2d6"), SfxName.BeastRoar)
                        );
                    }
                ));

            DragonBreath.Traits.Add(Trait.Evocation);

            DragonExtraUnarmedAttack = new DragonAncestryFeat("Additional Unarmed Attack", "You’ve directed magic through a part of your body, honing it into a powerful unarmed attack.", "You gain that additional attack as an unarmed attack in the brawling weapon group. Choose one of the additional attacks available to your heritage.\n\n• A horn unarmed attack deals 1d4 piercing damage and has the deadly d6 and finesse traits.\n• A tail unarmed attack deals 1d4 bludgeoning damage and has the finesse, sweep and trip traits.\n• A wing unarmed attack deals 1d4 bludgeoning damage and has the backswing, finesse, and shove traits.").WithOnCreature((creature =>
        {
            if (creature.PersistentCharacterSheet.Heritage == null || creature.PersistentCharacterSheet.Heritage is not DragonHertiageFeat)
            {
                return;
            }

            DragonHertiageFeat feat = (DragonHertiageFeat)creature.PersistentCharacterSheet.Heritage;

        }
        ));

            DragonExtraUnarmedAttack.Subfeats = new List<Feat>();

            DragonExtraUnarmedAttack.Subfeats.Add(new Feat(FeatName.CustomFeat, "", "A horn unarmed attack deals 1d4 piercing damage and has the deadly d6 and finesse traits.", new List<Trait>(), null)
            .WithCustomName("Dragon Horn")
            .WithOnCreature(creature =>
                        {
                            creature.WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "horn", "1d4", DamageKind.Piercing, Trait.Finesse, Trait.DeadlyD8));
                        })
            .WithPrerequisite(sheet =>
            {
                if (sheet.Sheet.Heritage == null || sheet.Sheet.Heritage is not DragonHertiageFeat)
                {
                    return false;
                }
                DragonHertiageFeat hertiage = (DragonHertiageFeat)sheet.Sheet.Heritage;

                if (hertiage.ExtraUnarmed1 != "horn" && hertiage.ExtraUnarmed2 != "horn")
                {
                    return false;
                }
                return true;
            }, "You must have a draconic hertiage that has this additional unarmed attack.")
    );

            DragonExtraUnarmedAttack.Subfeats.Add(new Feat(FeatName.CustomFeat, "", "A tail unarmed attack deals 1d4 bludgeoning damage and has the finesse, sweep and trip traits.", new List<Trait>(), null)
                    .WithCustomName("Dragon Tail")
                    .WithOnCreature(creature =>
                                {
                                    creature.WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Horn, "horn", "1d4", DamageKind.Bludgeoning, Trait.Finesse, Trait.Sweep, Trait.Trip));
                                })
                    .WithPrerequisite(sheet =>
                    {
                        if (sheet.Sheet.Heritage == null || sheet.Sheet.Heritage is not DragonHertiageFeat)
                        {
                            return false;
                        }
                        DragonHertiageFeat hertiage = (DragonHertiageFeat)sheet.Sheet.Heritage;

                        if (hertiage.ExtraUnarmed1 != "tail" && hertiage.ExtraUnarmed2 != "tail")
                        {
                            return false;
                        }
                        return true;
                    }, "You must have a draconic hertiage that has this additional unarmed attack.")
            );

            DragonExtraUnarmedAttack.Subfeats.Add(new Feat(FeatName.CustomFeat, "", "A wing unarmed attack deals 1d4 bludgeoning damage and has the backswing, finesse, and shove traits.", new List<Trait>(), null)
                        .WithCustomName("Dragon Wing")
                        .WithOnCreature(creature =>
                                    {
                                        creature.WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Wing, "wing", "1d4", DamageKind.Bludgeoning, Trait.Finesse, Trait.Shove, Trait.Backswing));
                                    })
                        .WithPrerequisite(sheet =>
                        {
                            if (sheet.Sheet.Heritage == null || sheet.Sheet.Heritage is not DragonHertiageFeat)
                            {
                                return false;
                            }
                            DragonHertiageFeat hertiage = (DragonHertiageFeat)sheet.Sheet.Heritage;

                            if (hertiage.ExtraUnarmed1 != "wing" && hertiage.ExtraUnarmed2 != "wing")
                            {
                                return false;
                            }
                            return true;
                        }, "You must have a draconic hertiage that has this additional unarmed attack.")
                );

            DraconicCantrip = new DragonAncestryFeat("Draconic Ancestry", "You're able to use some of your innate magic, which you can use to cast a cantrip from the tradition associated with your heritage.", "Choose a cantrip from the spell list corresponding to the tradition indicated in your heritage.You can cast that cantrip as an innate spell.\n\nAs normal, cantrips are heightened to half your level, rounded up.\n\nYour spellcasting ability for that spell is either the mental ability score you gained from the fixed ability score boost from the dragon ancestry, or Charisma if you gained a physical ability score boost from your fixed ability boost.").WithOnCreature((Action<Creature>)(creature =>
            {

                if (creature.PersistentCharacterSheet.Heritage == null || creature.PersistentCharacterSheet.Heritage is not DragonHertiageFeat || creature.PersistentCharacterSheet.Class.ClassTrait == null)
                {
                    return;
                }

                DragonHertiageFeat hertiage = (DragonHertiageFeat)creature.PersistentCharacterSheet.Heritage;

                creature.GetOrCreateSpellcastingSource(SpellcastingKind.Innate, Trait.Dragon, Ability.Charisma, hertiage.SpellList);

            }));



            ModManager.AddFeat(AncestryFeat);
            ModManager.AddFeat(DragonResistance);
            ModManager.AddFeat(DragonBreath);
            ModManager.AddFeat(DragonExtraUnarmedAttack);



        }




    }
}