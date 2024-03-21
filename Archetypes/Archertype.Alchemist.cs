using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using System.Linq;
using System;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder;

using Microsoft.Xna.Framework;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeAlchemist
{

  public static Feat AlchemistDedicationFeat;
  public static Feat ExpertAlchemyFeat;

  public static Trait InfusedTrait = ModManager.RegisterTrait(
        "Infused",
        new TraitProperties("Mutagen", true, "An infused item is expires at the end of an encounter and is removed ", true)
        {
        });

  public static ActionPossibility CreateAlchemyPossibilityItem(Item AlchemyItem, Creature creature)
  {
    return new ActionPossibility(new CombatAction(creature, AlchemyItem.Illustration, "Make " + AlchemyItem.Name, new Trait[] { Trait.Basic, Trait.Manipulate },
                              "{i}Spend one action to make this item and 1 infused reagent{/i}. The item only lasts until the end of the encounter.\n\n" + AlchemyItem.Description,
                                Target.Self()
                                .WithAdditionalRestriction((a) =>
                              {
                                if (!a.HasFreeHand)
                                {
                                  return "You need a free hand to use quick alchemy.";
                                }
                                else if (a.PersistentUsedUpResources.UsedUpActions.Count(x => x == "Used Infused Reagent.") >= a.Level)
                                {
                                  return "You have no infused reagents for the day.";
                                }
                                else return null;

                              }



                              ))
                              .WithSoundEffect(SfxName.PotionUse2)
                              .WithActionCost(1)
                              .WithEffectOnSelf(async (spell, caster) =>
                          {

                            AlchemyItem.Traits.Add(Trait.EncounterEphemeral);
                            AlchemyItem.Traits.Add(InfusedTrait);
                            caster.PersistentUsedUpResources.UsedUpActions.Add("Used Infused Reagent.");
                            caster.AddHeldItem(AlchemyItem);
                            string OverheadString = caster.Level - caster.PersistentUsedUpResources.UsedUpActions.Count(x => x == "Used Infused Reagent.") + " infused reagents left.";
                            caster.Occupies.Overhead(OverheadString, Color.White);


                          }
                          ), PossibilitySize.Half);
  }
  public static void LoadMod()

  {

    AlchemistDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You enjoy tinkering with alchemical formulas and substances in your spare time, and your studies have progressed beyond mere experimentation.",
            "You become trained in Crafting; if you were already trained in Crafting, you instead become trained in a skill of your choice." +
            "\b\bYou gain a infused reagents equal to your level and the Quick Alchemy{icon:Action} action which can be used to make any level 1 elixir or mutagen using 1 infused reagents. These items only last until the end of an encounter.\n\nInfused Reagents are restored after a long rest.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Alchemist Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.FinalAbilityScores.TotalScore(Ability.Intelligence) >= 14, "You must have at least 14 inteligence.")
            .WithOnSheet(sheet =>
            {

              if (sheet.GetProficiency(Trait.Crafting) == Proficiency.Untrained)
              {
                sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Alchemist Dedication Skill",
                "Alchemist Dedication skill",
                -1,
                (ft) => ft.CustomName == "Crafting")

                );
              }
              else
              {
                sheet.AddSelectionOption(
                  new SingleFeatSelectionOption(
                      "Alchemist Dedication Skill",
                      "Alchemist Dedication skill",
                      -1,
                      (ft) => ft is SkillSelectionFeat

                      ));
              }


            }).WithOnCreature((CalculatedCharacterSheetValues sheet, Creature creature) =>
        {



          creature.AddQEffect(new QEffect("Quick Alchemy", "You can quickly create alchemical items.")
          {


            ProvideActionIntoPossibilitySection = (QEffect qfself, PossibilitySection possibilitySection1) =>
            {
              if (possibilitySection1.PossibilitySectionId != PossibilitySectionId.MainActions)
              {
                return null;
              };

              int AlchemyLevel = 1;


              if (sheet.AllFeats.Contains(ExpertAlchemyFeat))
              {
                AlchemyLevel = 3;
              }

              PossibilitySection MutagenSection = new PossibilitySection("Mutagens");
              PossibilitySection ElixirSection = new PossibilitySection("Elixirs");

              foreach (Item AlchemyItem in Items.ShopItems.Where<Item>(item =>
              item.HasTrait(Trait.Elixir) && item.Level <= AlchemyLevel && !item.HasTrait(TraitMutagens.MutagenTrait))
            )
              {
                ElixirSection.AddPossibility(CreateAlchemyPossibilityItem(AlchemyItem, creature));
              }

              foreach (Item AlchemyItem in Items.ShopItems.Where<Item>((Func<Item, bool>)(item =>
              item.Level <= AlchemyLevel && item.HasTrait(TraitMutagens.MutagenTrait)))
            )
              {
                MutagenSection.AddPossibility(CreateAlchemyPossibilityItem(AlchemyItem, creature));
              }


              SubmenuPossibility submenuPossibility = new SubmenuPossibility(IllustrationName.InvisibilityPotion,
            "Quick Alchemy");
              submenuPossibility.Subsections.Add(ElixirSection);
              submenuPossibility.Subsections.Add(MutagenSection);

              return submenuPossibility;
            }

          })
            ;


        });

    ExpertAlchemyFeat = new TrueFeat(FeatName.CustomFeat,
    4,
    "You have learned how to brew stronger elixers.",
    "You may make alchemy items which are level 3 or below with Quick Alchemy.",
    new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Crafting) >= Proficiency.Expert, "You must be an expert in Crafting.")
    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(AlchemistDedicationFeat), "You must have the Alchemist Dedication feat.")
    .WithCustomName("Expert Alchemy");


    ModManager.AddFeat(AlchemistDedicationFeat);
    ModManager.AddFeat(ExpertAlchemyFeat);
  }
}