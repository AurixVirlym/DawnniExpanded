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

namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{

    public class HalflingAncestryFeat : TrueFeat
    {
        public HalflingAncestryFeat(string name, string flavorText, string rulesText)
            : base(FeatName.CustomFeat, 1, flavorText, rulesText, new[] { Trait.Ancestry, AncestryHalfling.ancestryTrait })
        {
            this
                .WithCustomName(name)
                .WithPrerequisite(sheet => sheet.Ancestries.Contains(AncestryHalfling.ancestryTrait), "You must be a Halfling.");
        }
    }

    public class AncestryHalfling
    {
        public static Trait ancestryTrait;
        public static Feat AncestryFeat;
        public static void LoadMod()
        {
            ancestryTrait = ModManager.RegisterTrait("Halfling", new TraitProperties("Halfling", true) { IsAncestryTrait = true });

            AncestryFeat = new AncestrySelectionFeat(
                        FeatName.CustomFeat,
                        "Optimistic and cheerful, blessed with uncanny luck, and driven by powerful wanderlust, halflings make up for their short stature with an abundance of bravado and curiosity.At once excitable and easygoing, they are the best kind of opportunists, and their passions favor joy over violence.Even in the jaws of danger, halflings rarely lose their sense of humor.\n\nMany taller people dismiss halflings due to their size or, worse, treat them like children.Halflings use these prejudices and misconceptions to their advantage, gaining access to opportunities and performing deeds of daring mischief or heroism. A halfling's curiosity is tempered by wisdom and caution, leading to calculated risks and narrow escapes. \n\nWhile their wanderlust and curiosity sometimes drive them toward adventure, halflings also carry strong ties to house and home, often spending above their means to achieve comfort in their homelife."

                        + "\n\n{b}Keen Eyes{/b} Your eyes are sharp, allowing you to make out small details about concealed or even invisible creatures that others might miss. You gain a +2 circumstance bonus when using the Seek action to find hidden or undetected creatures within 30 feet of you.",
                        new List<Trait> { Trait.Humanoid, ancestryTrait
        },
                        6,
                        5,
                        new List<AbilityBoost>()
                        {
                    new EnforcedAbilityBoost(Ability.Dexterity),
                    new EnforcedAbilityBoost(Ability.Wisdom),
                    new FreeAbilityBoost()
                        },
                        HaflingHertiages.LoadFeats().ToList())
                    .WithAbilityFlaw(Ability.Strength)
                    .WithCustomName("Halfling")
                    .WithOnSheet(sheet =>
                    {

                    })
                    .WithOnCreature(creature =>
                    {
                        creature.Traits.Add(Trait.Small);

                        new QEffect("Keen Eyes", "You Seek better.")
                        {
                            BonusToAttackRolls = (Func<QEffect, CombatAction, Creature, Bonus>)((qf, seek, defender) =>
                            {
                                if (defender == null)
                                    return (Bonus)null;
                                return seek != null && seek.ActionId == ActionId.Seek && (defender.DetectionStatus.Undetected || defender.DetectionStatus.HiddenTo.Contains(qf.Owner)) && defender.DistanceTo(qf.Owner) <= 6 ? new Bonus(2, BonusType.Circumstance, "Keen Eyes") : (Bonus)null;
                            })
                        };
                    });

            AncestryFeat.Subfeats.Add(VersatileHertiages.MakeVHfeat(AncestryFeat.CustomName));
            ModManager.AddFeat(AncestryFeat);




            ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 1, "You favor traditional halfling weapons, so you've learned how to use them more effectively.", "You have the trained proficiency with the halfling sling staff.\n\nFor the purpose of determining your proficiency, martial halfling weapons are simple weapons and advanced halfling weapons are martial weapons.", new Trait[]
  {
        ancestryTrait, DawnniExpanded.DETrait
}).WithOnSheet((sheet =>
  {
      sheet.Proficiencies.Set(ancestryTrait, Proficiency.Trained);
      sheet.Proficiencies.AddProficiencyAdjustment((Func<List<Trait>, bool>)(item => item.Contains(ancestryTrait) && item.Contains(Trait.Martial)), Trait.Simple);
      sheet.Proficiencies.AddProficiencyAdjustment((Func<List<Trait>, bool>)(item => item.Contains(ancestryTrait) && item.Contains(Trait.Advanced)), Trait.Martial);
  })).WithCustomName("Halfling Weapon Familiarity")
  );

            /*
                        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 1, "Your happy-go-lucky nature makes it seem like misfortune avoids you, and to an extent, that might even be true.", "{b}Frequency{/b} Once per day\n\nThe next skill check you make, you roll twice and take the higher result.", new Trait[]
                        {
                    ancestryTrait, DawnniExpanded.DETrait, Trait.Fortune
                      }).WithCustomName("Skilled Luck")
                      .WithOnCreature((sheet =>
                        {
                            sheet.AddQEffect(new QEffect()
                            {
                                ProvideMainAction = (Func<QEffect, Possibility>)(qf => (Possibility)new ActionPossibility(new CombatAction(qf.Owner, (Illustration)IllustrationName.BitOfLuck, "Skilled Luck", new Trait[3]
                    {
                      ancestryTrait, DawnniExpanded.DETrait, Trait.Fortune
                    }, "{b}Frequency{/b} Once per day\n\nThe next skill check you make, you roll twice and take the higher result.", Target.Self())
                    .WithActionCost(0).WithEffectOnSelf((async (spell, caster) =>
                    {
                        caster.AddQEffect(new QEffect("Skilled Luck", "The next skill check you make, you roll twice and take the higher result.", ExpirationCondition.Never, caster){
                            Illustration = IllustrationName.BitOfLuck,
                            ProvideFortuneEffect = (Func<bool, string>)(isSkillCheck => !isSkillCheck ? (string)null : effectName)
                        });
                        return;
                    }))

                     ))
                            }
                      )
                      }
                      )
                      )
                      );

            */



            ItemName FilcherFork = ModManager.RegisterNewItemIntoTheShop("Filcher's Fork", itemName =>
                    new Item(itemName, (Illustration)IllustrationName.Spear, "Filcher's Fork", 0, 1, DawnniExpanded.DETrait, Trait.Martial, Trait.Spear, Trait.Backstabber, Trait.DeadlyD6, Trait.Finesse, ancestryTrait, Trait.Thrown20Feet)
                    {
                        Description = "This halfling weapon looks like a long, two-pronged fork and is used as both a weapon and a cooking implement.\n\n",
                    }.WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)));

            ItemName HalflingSlingStaff = ModManager.RegisterNewItemIntoTheShop("Halfling Sling Staff", itemName =>
                    new Item(itemName, (Illustration)IllustrationName.Quarterstaff, "Halfling Sling Staff", 0, 5, DawnniExpanded.DETrait, Trait.Martial, ancestryTrait, Trait.Propulsive, Trait.TwoHanded, Trait.Reload1)
                    {
                        Description = "This staff ends in a Y - shaped split that cradles a sling.The length of the staff provides excellent leverage when used two-handed to fling rocks or bullets from the sling.\n\n",
                    }.WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning).WithRangeIncrement(16)));




        }




    }
}