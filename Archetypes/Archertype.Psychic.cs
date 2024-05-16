using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System.Collections.Generic;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;





namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypePsychic
{

  public static Feat CreateArchetypeConsciousMind(String name, string flavorText, SpellId cantrip)
  {
    return new Feat(FeatName.CustomFeat, flavorText, "You know a psi cantrips. Your psi cantrips are more powerful than normal, and you can make them even more powerful by spending a focus point to cast them \"amped\".\n\n{b}Psi cantrip{/b} " + AllSpells.CreateModernSpellTemplate(cantrip, Trait.Psychic).ToSpellLink(), new List<Trait> { DawnniExpanded.DETrait }, null).WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
    {
      SpellRepertoire spellRepertoire = sheet.SpellRepertoires[Trait.Psychic];
      spellRepertoire.SpellsKnown.Add(AllSpells.CreateModernSpell(cantrip, null, sheet.MaximumSpellLevel, inCombat: false, new SpellInformation()
      {
        PsychicAmpInformation = new PsychicAmpInformation(),
        ClassOfOrigin = Trait.Psychic
      }));

    }).WithCustomName(name);
  }

  public static Feat PsychicDedicationFeat;
  public static Trait PsychicArchetypeTrait;
  public static void LoadMod()

  {

    PsychicArchetypeTrait = ModManager.RegisterTrait(
        "PsychicArchetype",
        new TraitProperties("PsychicArchetype", false, "", false)
        {
        });

    PsychicDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You've awoken the latent abilities of your mind, taking your first steps into wielding psychic magic.",
            "You become trained in Occultism; if you were already trained in Occultism, you become trained in a skill of your choice.\n\nYou cast spells like a psychic and gain the Cast a Spell activity; as you don't have a subconscious mind, your thought components are simple intentions.\n\nChoose a conscious mind. You gain a spell repertoire with one standard psi cantrip from your conscious mind, which you cast as a psi cantrip.\n\nYou gain the normal benefits and the amp for this psi cantrip, but not any other benefits from the conscious mind.\n\nIf you don't have one, you gain a focus pool of 1 Focus Point, which you can use to amp your psi cantrips, and you can Refocus by meditating on your new powers.\n\nIf you already have a focus pool, increase the number of points in your pool by 1.\n\nYou're trained in occult spell attack rolls and occult spell DCs.\n\nYour key spellcasting ability for psychic archetype spells is the ability you used to qualify for the archetype, and they are occult psychic spells.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, PsychicArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait }, new List<Feat>()
  {
      CreateArchetypeConsciousMind("The Distance Grasp (Archetype)", "Motion characterizes the physical—a boulder falls, creatures move, the world turns. You believe the truest form of mind over matter is therefore to move things as well, wielding telekinesis as an arm that can grasp the furthest and finest of objects.", SpellId.TelekineticProjectile),
      CreateArchetypeConsciousMind("The Infinite Eye (Archetype)", "The true strength of the mind lies in the knowledge it contains, with each new observation contributing to the totality of its experiences. To grow your experiences—and with them, your power—you devote yourself to observing as much as possible, casting your senses through space and time with clairvoyance and precognition.\r\n",SpellId.Guidance),
      CreateArchetypeConsciousMind("The Tangible Dream (Archetype)", "You pull colors and shapes from the depth of your mind, projecting impossible creations into the world as tapestries of astral thread or sculptures of force and light.", SpellId.Shield)
  }


            )
            .WithCustomName("Psychic Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >= 14 || values.FinalAbilityScores.TotalScore(Ability.Intelligence) >= 14, "You must have at least Intelligence 14, or Charisma 14.")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Psychic, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {
        if (sheet.Sheet.Class?.ClassTrait == Trait.Psychic) return; // Do nothing if you're already this class. This feat will be removed in the next cycle due to a failed prerequisite anyway.

      Ability CastingAbility = Ability.Intelligence;

      if (sheet.FinalAbilityScores.TotalScore(Ability.Intelligence) < sheet.FinalAbilityScores.TotalScore(Ability.Charisma))
      {
        CastingAbility = Ability.Charisma;
      }

      Trait spellList = Trait.Occult;
      sheet.SpellTraditionsKnown.Add(spellList);
      sheet.SpellRepertoires.Add(Trait.Psychic, new SpellRepertoire(CastingAbility, spellList));
      ++sheet.FocusPointCount;


      sheet.AdditionalClassTraits.Add(Trait.Psychic);

      if (sheet.GetProficiency(Trait.Psychic) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Psychic, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Occultism) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Psychic Dedication Skill",
                "Psychic Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Occultism
                )
                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Psychic Dedication Skill",
                "Psychic Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat
                )
                );
      }


    });

    ModManager.AddFeat(PsychicDedicationFeat);


    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to form a basic thoughtform.",
            "You gain a 1st- or 2nd-level psychic feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, PsychicArchetypeTrait })
            .WithCustomName("Basic Thoughtform")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(PsychicDedicationFeat), "You must have the Psychic Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Thoughtform",
                  "Basic Thoughtform feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Psychic) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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
            "You gain the basic spellcasting benefits for Psychic.",
            "Add a common 1st level occult spell to your repertorie and gain a 1st level spell slot.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, PsychicArchetypeTrait })
            .WithCustomName("Basic Psychic Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(PsychicDedicationFeat), "You must have the Psychic Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
  SpellRepertoire repertoire;
  if (sheet.SpellRepertoires.TryGetValue(Trait.Psychic, out repertoire) == false)
  {
    return;
  }
  sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("1st level spell", "1st level psychic spell", -1, Trait.Psychic, Trait.Occult, 1, 1));
  repertoire.SpellSlots[1] += 1;
})
);








  }
}