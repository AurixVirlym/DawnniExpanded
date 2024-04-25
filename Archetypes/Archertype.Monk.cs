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
using Dawnsbury.Core.Mechanics.Treasure;




namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeMonk
{

  public static Feat MonkDedicationFeat;
  public static Trait MonkArchetypeTrait;
  public static void LoadMod()

  {

    MonkArchetypeTrait = ModManager.RegisterTrait(
        "MonkArchetype",
        new TraitProperties("MonkArchetype", false, "", false)
        {
        });

    MonkDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "Monastic training has taught you martial arts and allowed you to hone your mind, body, and spirit to new heights.",
            "You become trained in unarmed attacks and gain the powerful fist class feature.\n\nIf you are already trained in both of these skills, you instead become trained in a skill of your choice.\n\nYou become trained in monk class DC.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, MonkArchetypeTrait })
            .WithCustomName("Monk Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Strength) >= 14 && values.FinalAbilityScores.TotalScore(Ability.Dexterity) >= 14, "You must have at least 14 Strength and Dexterity.")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Monk, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Monk);

      if (sheet.GetProficiency(Trait.Monk) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Monk, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Unarmed) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Simple, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Acrobatics) == Proficiency.Untrained || sheet.GetProficiency(Trait.Athletics) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Monk Dedication Skill",
                "Monk Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Acrobatics || ft.FeatName == FeatName.Athletics)

                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Monk Dedication Skill",
                "Monk Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
      }
    })
    .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature creature) =>
  {
    creature.WithUnarmedStrike(Item.ImprovedFist());
    creature.AddQEffect(new QEffect()
    {
      Id = QEffectId.PowerfulFist
    });
  });

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
              4,
              "Your Monk training has made your more resilient.",
              "You gain 3 additional Hit Points for each monk archetype class feat you have.\n\nAs you continue selecting monk archetype class feats, you continue to gain additional Hit Points in this way.",
              new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, MonkArchetypeTrait })
              .WithCustomName("Monk Resiliency")
              .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(MonkDedicationFeat), "You must have the Monk Dedication feat.")
              .WithPrerequisite((CalculatedCharacterSheetValues values) =>
              values.Sheet.Class?.ClassTrait != Trait.Ranger &&
              values.Sheet.Class?.ClassTrait != Trait.Barbarian &&
              values.Sheet.Class?.ClassTrait != Trait.Fighter

              , "You have a class granting more than Hit Points per level than 8 + your Constitution modifier")
              .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) =>
              {

                int ReslientHP = 3 * sheet.AllFeats.Count(x => x.HasTrait(MonkArchetypeTrait));
                cr.MaxHP += ReslientHP;

              }));

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to learn basic monk katas.",
            "You gain a 1st- or 2nd-level monk feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, MonkArchetypeTrait })
            .WithCustomName("Basic Kata")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(MonkDedicationFeat), "You must have the Monk Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Kata",
                  "Basic Kata feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Monk) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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







    ModManager.AddFeat(MonkDedicationFeat);
  }
}