using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using System.Collections.Generic;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;


namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeSorcerer
{

  public static Feat SorcererDedicationFeat;
  public static Trait SorcererArchetypeTrait;

  public static Feat SorcererBasicSpellcasting;
  public static void LoadMod()

  {


    ArchetypeBloodline bloodline1 = new ArchetypeBloodline(FeatName.DraconicBloodline, "The blood of dragons flows through your veins. These beasts are both fearsome in combat and skilled at magic.", Trait.Arcane, SpellId.DragonClaws, SpellId.Shield, SpellId.TrueStrike, SpellId.ResistEnergy);
    ArchetypeBloodline bloodline2 = bloodline1;
    bloodline2.OnSheet = bloodline2.OnSheet + (Action<CalculatedCharacterSheetValues>)(values => values.AddSelectionOption((SelectionOption)new SingleFeatSelectionOption("DraconicAncestorType", "Ancestor dragon type", -1, (Func<Feat, bool>)(feat => feat.HasTrait(Trait.AncestorDragonTypeFeat)))));


    bloodline1.RulesText += "\n• Special: You will select a dragon type as your ancestor dragon. Your claws will deal extra damage of that type.";


    IEnumerable<Feat> LoadBloodlines()
    {
      yield return new ArchetypeBloodline(FeatName.ImperialBloodline, "You trace your lineage to a powerful ancestor from before the Time of the Broken Sun. Your ancestor mastered magic to such a degree that flashes of their power burn their way to you through time itself.", Trait.Arcane, SpellId.UnravelingBlast, SpellId.AncientDust, SpellId.MagicMissile, SpellId.Invisibility);
      yield return new ArchetypeBloodline(FeatName.AngelicBloodline, "One of your forebears hailed from a celestial realm, or your ancestors' devotion led to their lineage being blessed.", Trait.Divine, SpellId.AngelicHalo, SpellId.DivineLance, SpellId.Heal, SpellId.SpiritualWeapon);
      yield return new ArchetypeBloodline(FeatName.DemonicBloodline, "Demons have been on our plane forever, even before the Night of the Shooting Stars, and one of your ancestors felt their power firsthand, and you are still burdened by that corruption.", Trait.Divine, SpellId.GluttonsJaw, SpellId.AcidSplash, SpellId.Fear, SpellId.HideousLaughter);
      yield return new ArchetypeBloodline(FeatName.InfernalBloodline, "Your ancestor somehow made a pact with the Unquenchable Inferno herself and gained power that persists through generations.", Trait.Divine, SpellId.RejuvenatingFlames, SpellId.ProduceFlame, SpellId.BurningHands, SpellId.FlamingSphere);

      yield return new ArchetypeBloodline(FeatName.HagBloodline, "A hag long ago cursed your family, or you are a descendant of a hag or changeling, and their accursed corruption infests your blood and soul.", Trait.Occult, SpellId.JealousHex, SpellId.Daze, SpellId.Fear, SpellId.TouchOfIdiocy);
      yield return bloodline2;
    };

    List<Feat> bloodlinelist = LoadBloodlines().ToList();

    SorcererArchetypeTrait = ModManager.RegisterTrait(
        "SorcererArchetype",
        new TraitProperties("SorcererArchetype", false, "", false)
        {
        });

    SorcererDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
           "You coax the magic power in your blood to manifest, accessing magic others don't expect you to have.",
            "Choose a bloodline. You become trained in a skills of your choice.\n\nYou cast spells like a sorcerer and gain the Cast a Spell activity.\n\nYou gain a spell repertoire with two common cantrips from the spell list associated with your bloodline.\n\nYou're trained in spell attack rolls and spell DCs for your tradition's spells. \n\nYour key spellcasting ability for sorcerer archetype spells is Charisma, and they are sorcerer spells of your bloodline's tradition.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, SorcererArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait }, bloodlinelist)
            .WithCustomName("Sorcerer Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >= 14, "You must have at least 14 Charisma")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Sorcerer, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {



      sheet.AdditionalClassTraits.Add(Trait.Sorcerer);


      if (sheet.GetProficiency(Trait.Sorcerer) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Sorcerer, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      }


      sheet.AddSelectionOption(
          new SingleFeatSelectionOption(
              "Sorcerer Dedication Skill",
              "Sorcerer Dedication skill",
              -1,
              (ft) => ft is SkillSelectionFeat)
              );



    });




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to harness your blood's potency.",
            "You gain a 1st- or 2nd-level sorcerer feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, SorcererArchetypeTrait })
            .WithCustomName("Basic Blood Potency")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(SorcererDedicationFeat), "You must have the Sorcerer Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Blood Potency",
                  "Basic Blood Potency feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Sorcerer) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
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

    SorcererBasicSpellcasting = new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain the basic spellcasting benefits for Sorcerer.",
            "Add a common 1st level spell of your bloodline's tradition to your repertorie and gain a 1st level spell slot.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, SorcererArchetypeTrait })
            .WithCustomName("Basic Sorcerer Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(SorcererDedicationFeat), "You must have the Sorcerer Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
    
  SpellRepertoire repertoire;
  if (sheet.SpellRepertoires.TryGetValue(Trait.Sorcerer, out repertoire) == false)
  {
    return;
  }


  sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("SorcererSpellsArchetype", "1st level sorcerer spell", -1, Trait.Sorcerer, repertoire.SpellList, 1, 1));
  repertoire.SpellSlots[1] += 1;
});


    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 4, "You able to cast the magic innate to your bloodline", "You gain your bloodline's initial bloodline spell. If you don't already have one, you also gain a focus pool of 1 Focus Point, which you can Refocus without any special effort.", new Trait[3]
     {
        FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, SorcererArchetypeTrait
     }).WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(SorcererDedicationFeat), "You must have the Sorcerer Dedication feat.")
     .WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet =>
     {
       if (!sheet.SpellRepertoires.ContainsKey(Trait.Sorcerer))
       { return; }
       ArchetypeBloodline bloodlineArchetype = (ArchetypeBloodline)sheet.AllFeats.FirstOrDefault(ft => ft is ArchetypeBloodline);

       if (bloodlineArchetype != null)
       {

         sheet.AddFocusSpellAndFocusPoint(Trait.Sorcerer, Ability.Charisma, bloodlineArchetype.focusSpell);
       };

     })).WithCustomName("Basic Bloodline Spell"));


    ModManager.AddFeat(SorcererDedicationFeat);
    ModManager.AddFeat(SorcererBasicSpellcasting);


  }
}