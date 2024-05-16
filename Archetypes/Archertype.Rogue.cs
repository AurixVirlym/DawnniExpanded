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



namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeRogue
{

  public static Feat RogueDedicationFeat;
  public static Trait RogueArchetypeTrait;
  public static void LoadMod()

  {

    RogueArchetypeTrait = ModManager.RegisterTrait(
        "RogueArchetype",
        new TraitProperties("RogueArchetype", false, "", false)
        {
        });

    RogueDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You’ve learned to sneak, steal, and disable traps. With time and luck, you'll become capable of moving through the shadows, striking unseen, and escaping without notice.",
            "You gain a skill feat and the rogue’s surprise attack class feature.\n\nYou become trained in light armor.\n\nIn addition, you become trained in Stealth or Thievery plus one skill of your choice.\n\nIf you are already trained in both Stealth and Thievery, you become trained in an additional skill of your choice\n\nYou become trained in Rogue class DC.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RogueArchetypeTrait })
            .WithCustomName("Rogue Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Dexterity) >= 14, "You must have at least 14 Dexterity.")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Rogue, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Rogue);

      if (sheet.GetProficiency(Trait.Rogue) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Rogue, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.LightArmor) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.LightArmor, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Stealth) == Proficiency.Untrained || sheet.GetProficiency(Trait.Thievery) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Rogue Dedication Skill1",
                "Rogue Dedication Stealth or Thievery",
                -1,
                (ft) => ft.FeatName == FeatName.Stealth || ft.FeatName == FeatName.Thievery)

                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Rogue Dedication Skill1",
                "Rogue Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
      }

      sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Rogue Dedication Skill2",
                "Rogue Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
    });

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
                4,
                "You gain the sneak attack class feature.",
                "Except it deals 1d4 damage. You don't increase the number of dice as you gain levels.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RogueArchetypeTrait })
                .WithCustomName("Sneak Attacker")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(RogueDedicationFeat), "You must have the Rogue Dedication feat.")
                .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) => cr.AddQEffect(QEffect.SneakAttack("1d4")))

        );



    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to learn how to perform Rogue Trickery.",
            "You gain a 1st- or 2nd-level rogue feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, RogueArchetypeTrait })
            .WithCustomName("Basic Trickery")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(RogueDedicationFeat), "You must have the Rogue Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Trickery",
                  "Basic Trickery feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Rogue) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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







    ModManager.AddFeat(RogueDedicationFeat);
  }
}