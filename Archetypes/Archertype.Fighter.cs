using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;

using Dawnsbury.Core.CharacterBuilder.Selections.Options;

using System.Linq;
using Dawnsbury.Core.Mechanics;
using System;

using Dawnsbury.Core.CharacterBuilder.FeatsDb;

using System.Collections.Generic;




namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeFighter
{

  public static Feat FighterDedicationFeat;
  public static Trait FighterArchetypeTrait;
  public static void LoadMod()

  {


    FighterArchetypeTrait = ModManager.RegisterTrait(
        "FighterArchetype",
        new TraitProperties("FighterArchetype", false, "", false)
        {
        });

    FighterDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have spent time learning the art of warfare, increasing your skill with martial arms and at wearing armor. With further training, you can become a true combat specialist.",
            "You become trained in simple weapons and martial weapons.\n\nYou become trained in your choice of Acrobatics or Athletics.\n\nIf you are already trained in both of these skills, you instead become trained in a skill of your choice.\n\nYou become trained in fighter class DC.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, FighterArchetypeTrait })
            .WithCustomName("Fighter Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Strength) >= 14 && values.FinalAbilityScores.TotalScore(Ability.Dexterity) >= 14, "You must have at least 14 Strength and Dexterity.")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Fighter, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Fighter);

      if (sheet.GetProficiency(Trait.Fighter) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Fighter, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Martial) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Martial, Proficiency.Trained);
      }
      if (sheet.GetProficiency(Trait.Simple) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Simple, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Acrobatics) == Proficiency.Untrained || sheet.GetProficiency(Trait.Athletics) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Fighter Dedication Skill",
                "Fighter Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Acrobatics || ft.FeatName == FeatName.Athletics)

                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Fighter Dedication Skill",
                "Fighter Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
      }
    });

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
                4,
                "You gain the Attack of Opportunity reaction.",
                "{b}Attack of Opportunity {icon:Reaction}.{/b} When an adjacent creature provokes, you can make a melee Strike against the triggering creature as a reaction and possibly disrupt its action.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, FighterArchetypeTrait })
                .WithCustomName("Opportunist")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(FighterDedicationFeat), "You must have the feat Fighter Dedication feat.")
                .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) => cr.AddQEffect(QEffect.AttackOfOpportunity()))

        );

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
              4,
              "Your fighter training has made your more resilient.",
              "You gain 3 additional Hit Points for each fighter archetype class feat you have.\n\nAs you continue selecting fighter archetype class feats, you continue to gain additional Hit Points in this way.",
              new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, FighterArchetypeTrait })
              .WithCustomName("Fighter Resiliency")
              .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(FighterDedicationFeat), "You must have the Fighter Dedication feat.")
              .WithPrerequisite((CalculatedCharacterSheetValues values) =>
              values.Sheet.Class?.ClassTrait != Trait.Ranger &&
              values.Sheet.Class?.ClassTrait != Trait.Barbarian &&
              values.Sheet.Class?.ClassTrait != Trait.Monk

              , "You have a class granting more than Hit Points per level than 8 + your Constitution modifier")
              .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) =>
              {

                int ReslientHP = 3 * sheet.AllFeats.Count(x => x.HasTrait(FighterArchetypeTrait));
                cr.MaxHP += ReslientHP;

              }));

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to learn basic fighter maneuvers.",
            "You gain a 1st- or 2nd-level fighter feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, FighterArchetypeTrait })
            .WithCustomName("Basic Maneuver")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(FighterDedicationFeat), "You must have the feat Fighter Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Maneuver",
                  "Basic Maneuver feat",
                  -1,
                  (ft) =>
            {


              if (ft.HasTrait(Trait.Fighter) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
              {

                if (ft.CustomName == null)
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  else return false;

                }
                else
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  return false;
                }
              }
              return false;
            })
                  );
})

    );







    ModManager.AddFeat(FighterDedicationFeat);
  }
}