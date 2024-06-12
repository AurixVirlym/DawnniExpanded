using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System.Collections.Generic;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Display.Controls.Listbox;





namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeWizard
{

  public static Feat WizardDedicationFeat;
  public static Trait WizardArchetypeTrait;

  public static Feat WizardBasicSpellcasting;
  public static Feat WizardArcaneSchoolSpell;

  private static Feat CreateArchetypeFocusSpell(String name, Trait schoolTrait, string flavorText, SpellId FocusSpell)
  {
    Spell modernSpellTemplate = AllSpells.CreateModernSpellTemplate(FocusSpell, Trait.Wizard);
    return new Feat(FeatName.CustomFeat, flavorText, "You learn the focus spell " + AllSpells.CreateModernSpellTemplate(FocusSpell, Trait.Wizard).ToSpellLink() + ".",
     new List<Trait> { DawnniExpanded.DETrait }, null)
    .WithOnSheet(delegate (CalculatedCharacterSheetValues values)
    {
      values.WizardSchool = schoolTrait;
      values.AddFocusSpellAndFocusPoint(Trait.Wizard, Ability.Intelligence, FocusSpell);
    })
  .WithCustomName(name)
  .WithRulesBlockForSpell(FocusSpell, Trait.Arcane)
  .WithIllustration(modernSpellTemplate.Illustration);
    //. modernSpellTemplate.Name.ToLower() 

  }


  public static void LoadMod()

  {

    WizardArchetypeTrait = ModManager.RegisterTrait(
        "WizardArchetype",
        new TraitProperties("WizardArchetype", false, "", false)
        {
        });

    WizardDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have dabbled in the arcane arts and, through discipline and academic study, learned how to cast a few spells.",
            "You cast spells like a wizard and gain the Cast a Spell activity.\n\nYou can prepare two cantrips each day from the arcane spell list \n\nYou're trained in spell attack rolls and spell DCs for arcane spells. \n\nYour key spellcasting ability for wizard archetype spells is inteligence, and they are arcane wizard spells.\n\nYou become trained in Arcana; if you were already trained in Arcana, you instead become trained in a skill of your choice.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, WizardArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait })
            .WithCustomName("Wizard Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Intelligence) >= 14, "You must have at least 14 inteligence")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Wizard, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Wizard);
      PreparedSpellSlots spellList = new PreparedSpellSlots(Ability.Intelligence, Trait.Arcane);
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Wizard:Cantrip1"));
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Wizard:Cantrip2"));
      sheet.PreparedSpells.Add(Trait.Wizard, spellList);



      if (sheet.GetProficiency(Trait.Wizard) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Wizard, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Arcana) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Wizard Dedication Skill",
                "Wizard Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Arcana
                )
                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Wizard Dedication Skill",
                "Wizard Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat
                )
                );
      }


    });




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You have learnt a basic arcana.",
            "You gain a 1st- or 2nd-level wizard feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, WizardArchetypeTrait })
            .WithCustomName("Basic Arcana")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WizardDedicationFeat), "You must have the Wizard Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Arcana",
                  "Basic Arcana feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Wizard) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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

    WizardBasicSpellcasting = new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain the basic spellcasting benefits for Wizard.",
            "You may prepare one 1st level spell slot per day.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, WizardArchetypeTrait })
            .WithCustomName("Basic Wizard Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WizardDedicationFeat), "You must have the Wizard Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
  PreparedSpellSlots spellList;
  if (sheet.PreparedSpells.TryGetValue(Trait.Wizard, out spellList) == false)
  {
    return;
  }
  spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(1, "Wizard:Spell1-1"));
});

    ModManager.AddFeat(WizardBasicSpellcasting);

    WizardArcaneSchoolSpell = new TrueFeat(FeatName.CustomFeat,
                4,
                "You specialize your studies into one school of magic.",
                "You may pick an arcane school and gain it's initial school spell. If you don't already have one, you gain a focus pool of 1 Focus Point, which you can Refocus by studying.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, WizardArchetypeTrait }, new List<Feat>()
  {
              CreateArchetypeFocusSpell("Abjuration School (Archetype)", Trait.Abjuration, "As an abjurer, you master the art of protection, strengthening defenses, preventing attacks, and even turning magic against itself. You understand that an ounce of prevention is worth a pound of cure.", SpellId.ProtectiveWard),
        CreateArchetypeFocusSpell("Conjuration School (Archetype)", Trait.Conjuration, "As a conjurer, you summon creatures and objects from places beyond, and use magic to transport to distant locales. You understand that the true key to victory is strength in numbers.", SpellId.AugmentSummoning),
        CreateArchetypeFocusSpell("Divination School (Archetype)", Trait.Divination, "As a diviner, you master remote viewing and prescience, learning information that can transform investigations, research, and battle strategies. You understand that knowledge is power.", SpellId.DivinersSight),
        CreateArchetypeFocusSpell("Enchantment School (Archetype)", Trait.Enchantment, "As an enchanter, you use magic to manipulate others' minds. You might use your abilities to subtly influence others or seize control over them. You understand that the mind surpasses matter.", SpellId.CharmingWords),
        CreateArchetypeFocusSpell("Evocation School (Archetype)", Trait.Evocation, "As an evoker, you revel in the raw power of magic, using it to create and destroy with ease. You can call forth elements, forces, and energy to devastate your foes or to assist you in other ways. You understand that the most direct approach is the most elegant.", SpellId.ForceBolt),
        CreateArchetypeFocusSpell("Illusion School (Archetype)", Trait.Illusion, "As an illusionist, you use magic to create images, figments, and phantasms to baffle your enemies. You understand that perception is reality.", SpellId.WarpedTerrain),
        CreateArchetypeFocusSpell("Necromancy School (Archetype)", Trait.Necromancy, "As a necromancer, you call upon the powers of life and death. While your school is often vilified for its association with raising the undead, you understand that control over life also means control over healing.", SpellId.CallOfTheGrave),
        CreateArchetypeFocusSpell("Transmutation School (Archetype)", Trait.Transmutation, "As a transmuter, you alter the physical properties of things, transforming creatures, objects, the natural world, and even yourself at your whim. You understand that change is inevitable.", SpellId.PhysicalBoost),
  })
                .WithCustomName("Arcane School Spell")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(WizardDedicationFeat), "You must have the Wizard Dedication feat.");


    ModManager.AddFeat(WizardArcaneSchoolSpell);








    ModManager.AddFeat(WizardDedicationFeat);
  }
}