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
using Dawnsbury.Core.Mechanics.Treasure;






namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeCleric
{

  public static Feat ClericDedicationFeat;
  public static Trait ClericArchetypeTrait;

  public static Feat ClericBasicSpellcasting;





  public static void LoadMod()

  {
    AllFeats.All.RemoveAll(feat => feat.FeatName == FeatName.DomainInitiate);
    ModManager.AddFeat(new TrueFeat(FeatName.DomainInitiate, 1, "Your deity bestows a special spell related to their powers.", "Select one domain—a subject of particular interest to you within your religion—from your deity's list. You gain an initial domain spell for that domain, a spell unique to the domain and not available to other clerics.", new Trait[1]
      {
        Trait.Cleric
      }).WithMultipleSelection().WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet =>
      {

        sheet.AddSelectionOption((SelectionOption)new SingleFeatSelectionOption("Domain", "Domain", -1, (Func<Feat, bool>)(ft =>
        {
          if (!ft.HasTrait(Trait.ClericDomain))
            return false;
          DeitySelectionFeat deity = sheet.Deity;

          if (deity == null)
          {
            ArchetypeDeitySelectionFeat deityArchetype = (ArchetypeDeitySelectionFeat)sheet.AllFeats.FirstOrDefault(ft => ft is ArchetypeDeitySelectionFeat);
            return deityArchetype != null && ((IEnumerable<FeatName>)deityArchetype.AllowedDomains).Contains<FeatName>(ft.FeatName);
          }

          return deity != null && ((IEnumerable<FeatName>)deity.AllowedDomains).Contains<FeatName>(ft.FeatName);
        })));
      })));


    static IEnumerable<Feat> LoadDeities()
    {
      yield return (Feat)new ArchetypeDeitySelectionFeat("Archetype Deity: The Oracle", "The Oracle is the most widely worshipped and revered deity on Our Point of Light, especially among the civilized folk. Well-aware that they are floating on a small insignificant plane through an endless void, the inhabitants of the plane put their faith in the Oracle, hoping that she would guide them and protect them from incomprehensible threats.\n\nAnd the Oracle, unlike most other deities, doesn't speak with her followers through signs or riddles. Instead, she maintains an open channel of verbal communication at her Grand Temple, though the temple's archclerics only rarely grant permission to ordinary folk to petition the Oracle.", "{b}• Edicts{/b} Care for each other, enjoy your life, protect civilization\n{b}• Anathema{/b} Travel into the Void, summon or ally yourself with demons", NineCornerAlignmentExtensions.All(), new FeatName[2]
      {
        FeatName.HealingFont,
        FeatName.HarmfulFont
      }, new FeatName[4]
      {
        FeatName.DomainHealing,
        FeatName.DomainTravel,
        FeatName.DomainLuck,
        FeatName.DomainSun
      }, ItemName.Morningstar, new SpellId[1]
      {
        SpellId.Invisibility
      });
      yield return (Feat)new ArchetypeDeitySelectionFeat("Archetype Deity: The Blooming Flower", "The only deity to have granted any spells during the Time of the Broken Sun, the Blooming Flower is perhaps responsible for the survival of civilization on Our Point of Light. She was instrumental in allowing the civilized folk to beat back the undead and summon food to carry us though the long night. Ever since then, she has become associated with sun and light and is respected for her role in the survival of Our Point of Light even by her enemies.\n\nIn spring, during the Day of Blossoms, she makes all plants on the plane blossom at the same time, covering every field, meadow and forest in a beautiful spectacle of bright colors. This causes winter to end and ushers in a one-week celebration during which all combat is not just forbidden, but impossible.", "{b}• Edicts{/b} Grow yourself, protect and enjoy nature, destroy the undead\n{b}• Anathema{/b} Break the stillness of forests, steal food, desecrate ancient sites", ((IEnumerable<NineCornerAlignment>)NineCornerAlignmentExtensions.All()).Except<NineCornerAlignment>((IEnumerable<NineCornerAlignment>)new NineCornerAlignment[3]
      {
        NineCornerAlignment.NeutralEvil,
        NineCornerAlignment.ChaoticEvil,
        NineCornerAlignment.LawfulEvil
      }).ToArray<NineCornerAlignment>(), new FeatName[1]
      {
        FeatName.HealingFont
      }, new FeatName[4]
      {
        FeatName.DomainSun,
        FeatName.DomainMoon,
        FeatName.DomainLuck,
        FeatName.DomainHealing
      }, ItemName.Shortbow, new SpellId[1]
      {
        SpellId.ColorSpray
      });
      yield return (Feat)new ArchetypeDeitySelectionFeat("Archetype Deity: The Thundering Tsunami", "A chaotic force of destruction, the Thundering Tsunami is best known for the Nights where Ocean Walks, unpredictable events that happen once in a generation when waves wash over our seaside settlements, and leave behind hundreds of destructive water elementals that wreak further havoc before they're killed or dry out and perish.\n\nDespite this destruction and despite being the most worshipped by evil water cults, the Thundering Tsunami is not evil herself and scholars believe that her destructive nights are a necessary component of the world's lifecycle, a release valve for pressure which would otherwise necessarily cause the plane itself to self-destruct.", "{b}• Edicts{/b} Build durable structures, walk at night, learn to swim\n{b}• Anathema{/b} Dive deep underwater, live on hills or inland, approach the edges of the world", ((IEnumerable<NineCornerAlignment>)NineCornerAlignmentExtensions.All()).Except<NineCornerAlignment>((IEnumerable<NineCornerAlignment>)new NineCornerAlignment[3]
      {
        NineCornerAlignment.LawfulGood,
        NineCornerAlignment.LawfulNeutral,
        NineCornerAlignment.LawfulEvil
      }).ToArray<NineCornerAlignment>(), new FeatName[2]
      {
        FeatName.HealingFont,
        FeatName.HarmfulFont
      }, new FeatName[4]
      {
        FeatName.DomainMoon,
        FeatName.DomainCold,
        FeatName.DomainTravel,
        FeatName.DomainDestruction
      }, ItemName.Warhammer, new SpellId[1]
      {
        SpellId.HideousLaughter
      });
      yield return (Feat)new ArchetypeDeitySelectionFeat("Archetype Deity: The Unquenchable Inferno", "The Unquenchable Inferno is the eternal fire that burns within the plane itself. The Inferno never answers any {i}commune{/i} rituals or other divinations, but each time a fire elemental dies, it releases a memory. This could be a single sentence or an hour-long recitation, and depending on the age and power of the elemental, it could be trivial minutiae of the elemental's life or important discussions on the nature of the cosmos.\n\nThe Keeper-Monks of the Ring of Fire consider fire to be the living memory of this plane and are funding expeditions to capture, kill and listen to elder fire elementals everywhere so that more fundamental truths may be revealed and shared.", "{b}• Edicts{/b} be prepared, battle your enemies, learn Ignan\n{b}• Anathema{/b} burn books, gain fire immunity", NineCornerAlignmentExtensions.All(), new FeatName[2]
      {
        FeatName.HealingFont,
        FeatName.HarmfulFont
      }, new FeatName[4]
      {
        FeatName.DomainSun,
        FeatName.DomainFire,
        FeatName.DomainDestruction,
        FeatName.DomainDeath
      }, ItemName.Earthbreaker, new SpellId[1]
      {
        SpellId.BurningHands
      });
      yield return (Feat)new ArchetypeDeitySelectionFeat("Archetype Deity: The Cerulean Sky", "Perhaps the most calm deity of them all, the Cerulean Sky manifests as the dome that shields us from the dangers of the Void during the day, but shows us the beauty of the Points of Light at night. It's the Cerulean Sky who connects leylines, draws water into clouds, and suffuses the land with both positive and negative energies, balancing the plane. She is the guardian of inanimate forces.\n\nBut perhaps because of her cold detachment and incomprehensible logic, over time she paradoxically became to be viewed more as the goddess of the night than of the daytime sky.", "{b}• Edicts{/b} Contemplate the sky, explore the world, fly\n{b}• Anathema{/b} Stay inside, create smoke, delve underground", NineCornerAlignmentExtensions.All(), new FeatName[2]
      {
        FeatName.HealingFont,
        FeatName.HarmfulFont
      }, new FeatName[4]
      {
        FeatName.DomainMoon,
        FeatName.DomainCold,
        FeatName.DomainTravel,
        FeatName.DomainUndeath
      }, ItemName.Falchion, new SpellId[1]
      {
        SpellId.ShockingGrasp
      });
    }

    List<Feat> dietylist = LoadDeities().ToList();

    ClericArchetypeTrait = ModManager.RegisterTrait(
        "ClericArchetype",
        new TraitProperties("ClericArchetype", false, "", false)
        {
        });

    ClericDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You are an ordained priest of your deity and have even learned how to cast a few divine spells. Though your main training lies elsewhere, your religious calling provides you divine gifts.",
            "You cast spells like a cleric and gain the Cast a Spell activity.\n\nYou can prepare two cantrips each day from the divine spell list \n\nYou're trained in spell attack rolls and spell DCs for divine spells. \n\nYour key spellcasting ability for cleric archetype spells is wisdom, and they are divine cleric spells.\n\nChoose a deity as you would if you were a cleric. You become bound by that deity's anathema\n\nYou become trained in Religion and your deity's associated skill; for each of these skills in which you were already trained, you instead become trained in a skill of your choice." + "\n\nYou don't gain any other abilities from your choice of deity."
,
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, ClericArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait },
            dietylist)
            .WithCustomName("Cleric Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Wisdom) >= 14, "You must have at least 14 wisdom")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Cleric, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Cleric);
      PreparedSpellSlots spellList = new PreparedSpellSlots(Ability.Wisdom, Trait.Divine);
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Cleric:Cantrip1"));
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Cleirc:Cantrip2"));
      sheet.PreparedSpells.Add(Trait.Cleric, spellList);



      if (sheet.GetProficiency(Trait.Cleric) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Cleric, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Religion) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Cleric Dedication Skill",
                "Cleric Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Religion
                )
                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Cleric Dedication Skill",
                "Cleric Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat
                )
                );
      }


    });




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You start to understand your faith's dogma",
            "You gain a 1st- or 2nd-level cleric feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, ClericArchetypeTrait })
            .WithCustomName("Basic Dogma")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(ClericDedicationFeat), "You must have the Cleric Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Dogma",
                  "Basic Dogma feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Cleric) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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

    ClericBasicSpellcasting = new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain the basic spellcasting benefits for Cleric.",
            "You may prepare one 1st level spell slot per day.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, ClericArchetypeTrait })
            .WithCustomName("Basic Cleric Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(ClericDedicationFeat), "You must have the Cleric Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
  PreparedSpellSlots spellList;
  if (sheet.PreparedSpells.TryGetValue(Trait.Cleric, out spellList) == false)
  {
    return;
  }
  spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(1, "Cleric:Spell1-1"));
});

    ModManager.AddFeat(ClericBasicSpellcasting);
    ModManager.AddFeat(ClericDedicationFeat);
  }
}