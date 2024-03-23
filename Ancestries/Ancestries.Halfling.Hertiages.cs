using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.CharacterBuilding;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using static Dawnsbury.Delegates;


namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{
    public class HaflingHertiages
    {
        public static IEnumerable<Feat> LoadFeats()

        {
            yield return new HeritageSelectionFeat(FeatName.CustomFeat,
                       "You're not quite like other Halflings.",
                       "You have two free ability boosts instead of an elf's normal ability boosts and flaw.")
                   .WithCustomName("Unusual Halfling")
                   .WithOnSheet((sheet =>
        {
            sheet.AbilityBoostsFabric.AbilityFlaw = new Ability?();
            sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
          {
            (AbilityBoost) new FreeAbilityBoost(),
            (AbilityBoost) new FreeAbilityBoost()
          };
        }));

            yield return new HeritageSelectionFeat(FeatName.CustomFeat,
                           "Your finely honed senses quickly clue you in to danger or trickery.",
                           "You gain a +1 circumstance bonus to your Perception DC, though not to your Perception checks.")
                       .WithCustomName("Observant Halfling")
                       .WithOnCreature((creature =>
            {
                creature.AddQEffect(new QEffect("Observant Halfling", "You have +1 circumstance bonus to your Perception DC.")
                {
                    BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                       {
                           if (defense == Defense.Perception)
                           {
                               return new Bonus(1, BonusType.Circumstance, "Observant Halfling");
                           }
                           return null;
                       },
                });
            }
            ));

            yield return new HeritageSelectionFeat(FeatName.CustomFeat,
                          "You hail from deep in a jungle or forest, and you've learned how to use your small size to wriggle through undergrowth, vines, and other obstacles.",
                          "You ignore difficult terrain")
                      .WithCustomName("Wildwood Halfling")
                      .WithOnCreature((creature =>
           {
               creature.AddQEffect(new QEffect("Wildwood Halfling", "You ignore difficult terrain.")
               {
                   Id = QEffectId.IgnoresDifficultTerrain
               }
               );
           }
           ));

            yield return new HeritageSelectionFeat(FeatName.CustomFeat,
                           "Your family line is known for keeping a level head and staving off fear when the chips were down, making them wise leaders and sometimes even heroes.",
                           "When you roll a success on a saving throw against an emotion effect, you get a critical success instead.")
                       .WithCustomName("Gusty Halfling")
                       .WithOnCreature((creature =>
            {
                creature.AddQEffect(new QEffect("Gutsy Halfling", "When you roll a success on a saving throw against an emotion effect, you get a critical success instead.", ExpirationCondition.Never, creature, (Illustration)IllustrationName.None)
                {
                    Innate = true,
                    AdjustSavingThrowResult = (Func<QEffect, CombatAction, CheckResult, CheckResult>)((self, action, initialResult) => action.HasTrait(Trait.Emotion) && initialResult == CheckResult.Success ? CheckResult.CriticalSuccess : initialResult)
                });
            }
            ));



        }
    }


}

