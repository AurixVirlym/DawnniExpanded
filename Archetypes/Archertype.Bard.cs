using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;






namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeBard
{

  public static Feat BardDedicationFeat;
  public static Trait BardArchetypeTrait;

  public static Feat BardBasicSpellcasting;

  public static Feat BardBasicFeat;
  public static void LoadMod()

  {

    BardArchetypeTrait = ModManager.RegisterTrait(
        "BardArchetype",
        new TraitProperties("BardArchetype", false, "", false)
        {
        });

    BardDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "A muse has called you to dabble in occult lore, allowing you to cast a few spells. The deeper you delve, the more powerful your performances become.",
            "You cast spells like a bard and gain the Cast a Spell activity.\n\nYou gain a spell repertoire with two common cantrips from the occult spell list. \n\nYou're trained in spell attack rolls and spell DCs for occult spells. \n\nYour key spellcasting ability for bard archetype spells is Charisma, and they are occult bard spells.\n\nYou become trained in Occultism and Performance; for each of these skills in which you were already trained, you instead become trained in a skill of your choice.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait })
            .WithCustomName("Bard Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >= 14, "You must have at least 14 Charizzma")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Bard, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

            {
              if (sheet.Sheet.Class?.ClassTrait == Trait.Bard) return; // Do nothing if you're already this class. This feat will be removed in the next cycle due to a failed prerequisite anyway.


              Trait spellList = Trait.Occult;


              sheet.SpellTraditionsKnown.Add(spellList);
              sheet.SpellRepertoires.Add(Trait.Bard, new SpellRepertoire(Ability.Charisma, spellList));
              sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardCantripsArchetype", "Cantrips", -1, Trait.Bard, Trait.Occult, 0, 2));

              sheet.AdditionalClassTraits.Add(Trait.Bard);


              if (sheet.GetProficiency(Trait.Bard) == Proficiency.Untrained)
              {
                sheet.SetProficiency(Trait.Bard, Proficiency.Trained);
              }

              if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
              {
                sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
              }

              if (sheet.GetProficiency(Trait.Occultism) == Proficiency.Untrained && sheet.GetProficiency(Trait.Performance) == Proficiency.Untrained)
              {
                sheet.AddSelectionOption(
                    new MultipleFeatSelectionOption(
                        "Bard Dedication Skill1",
                        "Bard Dedication skill",
                        -1,
                        (ft) => ft.FeatName == FeatName.Occultism || ft == NewSkills.Performance
                        , 2)
                        );
              }
              else if (sheet.GetProficiency(Trait.Occultism) == Proficiency.Untrained || sheet.GetProficiency(Trait.Performance) == Proficiency.Untrained)
              {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Bard Dedication Skill2",
                        "Bard Dedication skill",
                        -1,
                        (ft) => ft.FeatName == FeatName.Occultism || ft == NewSkills.Performance)

                        );
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Bard Dedication Skill3",
                        "Bard Dedication skill",
                        -1,
                        (ft) => ft is SkillSelectionFeat)

                        );
              }
              else
              {
                sheet.AddSelectionOption(
                    new MultipleFeatSelectionOption(
                        "Bard Dedication Skill4",
                        "Bard Dedication skill",
                        -1,
                        (ft) => ft is SkillSelectionFeat
                        , 2)
                        );
              }


            });




    ModManager.AddFeat(BardBasicFeat = new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to hear basic Muse's Whispers.",
            "You gain a 1st- or 2nd-level bard feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait })
            .WithCustomName("Basic Muse's Whispers")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat), "You must have the Bard Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Muse's Whispers",
                  "Basic Muse's Whispers feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Bard) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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
            6,
            "You are able to hear advanced Muse's Whispers.",
            "You gain one bard feat. For the purpose of meeting its prerequisites, your bard level is equal to half your character level.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait })
            .WithMultipleSelection()
            .WithCustomName("Advanced Muse's Whispers")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat), "You must have the Bard Dedication feat.")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardBasicFeat), "You must have the Basic Muse's Whispers feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Advanced Muse's Whispers",
                  "Advanced Muse's Whispers feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Bard) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
              {

                if (ft.CustomName == null)
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                  if (FeatwithLevel.Level <= (int)Math.Floor((double)sheet.CurrentLevel / 2))
                  {
                    return true;
                  }
                  else return false;

                }
                else
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

                  if (FeatwithLevel.Level <= (int)Math.Floor((double)sheet.CurrentLevel / 2))
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



    BardBasicSpellcasting = new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain the basic spellcasting benefits for Bard.",
            "Add a common 1st level occult spell to your repertorie and gain a 1st level spell slot."
            + "\n\nAt level 6, Add a common 2nd level occult spell to your repertorie and gain a 2nd level spell slot. You can select one spell from your repertoire as a signature spell."
            + "\n\nAt level 8, Add a common 3rd level occult spell to your repertorie and gain a 3rd level spell slot.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait })
            .WithCustomName("Basic Bard Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat), "You must have the Bard Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
  SpellRepertoire repertoire;
  if (sheet.SpellRepertoires.TryGetValue(Trait.Bard, out repertoire) == false)
  {
    return;
  }

  switch (sheet.CurrentLevel)
  {
    case 6:
    case 7:
      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype1", "1st level bard spell", -1, Trait.Bard, Trait.Occult, 1, 1));
      ++repertoire.SpellSlots[1];

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype2", "2nd level bard spell", -1, Trait.Bard, Trait.Occult, 2, 1));
      ++repertoire.SpellSlots[2];

      sheet.AddSelectionOption(new SignatureSpellSelectionOption("BardSignatureSpellArchetype1", "Signature Bard archetype spell", -1, 1, Trait.Bard));

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype3", "3rd level bard spell", 8, Trait.Bard, Trait.Occult, 3, 1));
      sheet.AddAtLevel(8, _ => ++repertoire.SpellSlots[3]);
      break;
    case 8:
      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype1", "1st level bard spell", -1, Trait.Bard, Trait.Occult, 1, 1));
      ++repertoire.SpellSlots[1];

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype2", "2nd level bard spell", -1, Trait.Bard, Trait.Occult, 2, 1));
      ++repertoire.SpellSlots[2];

      sheet.AddSelectionOption(new SignatureSpellSelectionOption("BardSignatureSpellArchetype1", "Signature Bard archetype spell", -1, 1, Trait.Bard));

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype3", "3rd level bard spell", -1, Trait.Bard, Trait.Occult, 3, 1));
      ++repertoire.SpellSlots[3];

      break;
    default:
      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype1", "1st level bard spell", -1, Trait.Bard, Trait.Occult, 1, 1));
      ++repertoire.SpellSlots[1];

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype2", "2nd level bard spell", 6, Trait.Bard, Trait.Occult, 2, 1));
      sheet.AddAtLevel(6, _ => ++repertoire.SpellSlots[2]);

      sheet.AddSelectionOption(new SignatureSpellSelectionOption("BardSignatureSpellArchetype1", "Signature Bard archetype spell", 6, 1, Trait.Bard));

      sheet.AddSelectionOption(new AddToSpellRepertoireOption("BardSpellsArchetype3", "3rd level bard spell", 8, Trait.Bard, Trait.Occult, 3, 1));
      sheet.AddAtLevel(8, _ => ++repertoire.SpellSlots[3]);
      break;
  }
});

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
              6,
              "",
              "You gain the counter performance composition spell. If you don't already have one, you gain a focus pool of 1 Focus Point.",
              new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait })
              .WithCustomName("Counter Perform")
              .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat), "You must have the Bard Dedication feat.")
              .WithOnSheet((values =>
       {
         if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
           return;
         values.AddFocusSpellAndFocusPoint(Trait.Sorcerer, Ability.Charisma, SpellCounterPerformance.Id);
       }))
       .WithIllustration(SpellCounterPerformance.SpellIllustration)
       .WithRulesBlockForSpell(SpellCounterPerformance.Id, Trait.Occult));

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
               8,
               "",
               "You gain the inspire courage composition cantrip.",
               new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BardArchetypeTrait })
               .WithCustomName("Inspirational Performance")
               .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat), "You must have the Bard Dedication feat.")
               .WithOnSheet((values =>
      {
        if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
          return;
        CombatAction InspireAction = SpellInspireCourage.CombatAction(null, values.MaximumSpellLevel, false);
        InspireAction.SpellId = SpellInspireCourage.Id;
        Spell InspireSpell = new Spell(InspireAction);
        SpellRepertoire repertoire = values.SpellRepertoires[Trait.Bard];
        repertoire?.SpellsKnown.Add(InspireSpell);

      }))
      .WithIllustration(SpellInspireCourage.SpellIllustration)
      .WithRulesBlockForSpell(SpellInspireCourage.Id, Trait.Occult));
    ModManager.AddFeat(BardDedicationFeat);
    ModManager.AddFeat(BardBasicSpellcasting);


  }
}