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
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;



namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeRanger
{

  public static Feat RangerDedicationFeat;
  public static Trait RangerArchetypeTrait;
  public static void LoadMod()

  {

    RangerArchetypeTrait = ModManager.RegisterTrait(
        "RangerArchetype",
        new TraitProperties("RangerArchetype", false, "", false)
        {
        });

    RangerDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have studied hunting, tracking, and wilderness survival, adding a ranger’s tools to your skill set.",
            "You become trained in Survival.\n\nif you were already trained in Survival, you instead become trained in another skill of your choice.\n\nYou become trained in Ranger class DC.\n\nYou can use the Hunt Prey action.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RangerArchetypeTrait })
            .WithCustomName("Ranger Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Dexterity) >= 14, "You must have at least 14 Dexterity.")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Ranger, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {
      sheet.AdditionalClassTraits.Add(Trait.Ranger);

      if (sheet.GetProficiency(Trait.Ranger) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Ranger, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Survival) == Proficiency.Untrained)
      {
        sheet.AddFeat(NewSkills.Survival, null);
      }
      else
      {

        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Ranger Dedication Skill1",
                "Ranger Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
      }



    }).WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) => cr.AddQEffect(Ranger.HuntPreyQEffect()))
    ;




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to learn how to perform Hunter's Tricks.",
            "You gain a 1st- or 2nd-level ranger feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RangerArchetypeTrait })
            .WithCustomName("Basic Hunter's Trick")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(RangerDedicationFeat), "You must have the Ranger Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Hunter's Trick",
                  "Basic Hunter's Trick feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Ranger) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "Your Ranger training has made your more resilient.",
            "You gain 3 additional Hit Points for each ranger archetype class feat you have.\n\nAs you continue selecting ranger archetype class feats, you continue to gain additional Hit Points in this way.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RangerArchetypeTrait })
            .WithCustomName("Ranger Resiliency")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(RangerDedicationFeat), "You must have the Ranger Dedication feat.")
            .WithPrerequisite((CalculatedCharacterSheetValues values) =>
            values.Sheet.Class?.ClassTrait != Trait.Monk &&
            values.Sheet.Class?.ClassTrait != Trait.Barbarian &&
            values.Sheet.Class?.ClassTrait != Trait.Fighter

            , "You have a class granting more than Hit Points per level than 8 + your Constitution modifier")
            .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) =>
            {

              int ReslientHP = 3 * sheet.AllFeats.Count(x => x.HasTrait(RangerArchetypeTrait));
              cr.MaxHP += ReslientHP;

            }));






    ModManager.AddFeat(RangerDedicationFeat);
  }
}