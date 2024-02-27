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
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;





namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeBard
{

    public static Feat BardDedicationFeat;
    public static Trait BardArchetypeTrait;

    public static Feat BardBasicSpellcasting; 
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
                "You cast spells like a bard and gain the Cast a Spell activity.\n\nYou gain a spell repertoire with two common cantrips from the occult spell list. \n\nYou're trained in spell attack rolls and spell DCs for occult spells. \n\nYour key spellcasting ability for bard archetype spells is Charisma, and they are occult bard spells.\n\nYou become trained in Occultism and Performance; for each of these skills in which you were already trained, you instead become trained in a skill of your choice."+"\n\n{b}Focus Spells granted by classes such as ranger and monk break archetype spellcasting{/b}", 
                new Trait[] {FeatArchetype.DedicationTrait,FeatArchetype.ArchetypeTrait,DawnniExpanded.DETrait,BardArchetypeTrait})
                .WithCustomName("Bard Dedication")
                .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >=14 , "You must have at least 14 Charizzma")
                .WithPrerequisite(values => values.Sheet.Class.ClassTrait != Trait.Bard, "You already have this archetype as a main class.")
                .WithPrerequisite(values => 
                values.Sheet.Class.ClassTrait != Trait.Bard &&
                values.Sheet.Class.ClassTrait != Trait.Wizard &&
                values.Sheet.Class.ClassTrait != Trait.Magus &&
                values.Sheet.Class.ClassTrait != Trait.Sorcerer &&
                values.Sheet.Class.ClassTrait != Trait.Psychic &&
                values.Sheet.Class.ClassTrait != Trait.Cleric
                , "You may not take a spellcasting class archetype if your main class grants you spellcasting. (engine limits, sorry.)")
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                    
        {

        sheet.AdditionalClassTraits.Add(Trait.Bard);
        Trait spellList = Trait.Occult;
        sheet.SpellTraditionsKnown.Add(spellList);

        sheet.SpellRepertoires.Add(Trait.Bard, new SpellRepertoire(Ability.Charisma, spellList));
        sheet.AddSelectionOption((SelectionOption) new AddToSpellRepertoireOption("BardCantrips", "Cantrips", 2, Trait.Bard, spellList, 0, 2));
       
          
        

          if (sheet.GetProficiency(Trait.Bard) == Proficiency.Untrained){
            sheet.SetProficiency(Trait.Bard,Proficiency.Trained);
          }

          if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained){
            sheet.SetProficiency(Trait.Spell,Proficiency.Trained);
          }

           if (sheet.GetProficiency(Trait.Occultism) == Proficiency.Untrained && sheet.GetProficiency(Trait.Performance) == Proficiency.Untrained )
            {
            sheet.AddSelectionOption(
                new MultipleFeatSelectionOption(
                    "Bard Dedication Skill", 
                    "Bard Dedication skill", 
                    -1, 
                    (ft) => ft.FeatName == FeatName.Occultism || ft.CustomName == "Performance"
                    ,2)
                    );
          } else if (sheet.GetProficiency(Trait.Occultism) == Proficiency.Untrained || sheet.GetProficiency(Trait.Performance) == Proficiency.Untrained ){
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Bard Dedication Skill", 
                    "Bard Dedication skill", 
                    -1, 
                    (ft) => ft.FeatName == FeatName.Occultism || ft.CustomName == "Performance")
                    
                    );
                sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Bard Dedication Skill2", 
                    "Bard Dedication skill", 
                    -1, 
                    (ft) => ft is SkillSelectionFeat)
                    
                    );
          } else  {
            sheet.AddSelectionOption(
                new MultipleFeatSelectionOption(
                    "Bard Dedication Skill", 
                    "Bard Dedication skill", 
                    -1, 
                    (ft) => ft is SkillSelectionFeat
                    ,2)
                    );
          }
      
          
        });
        

          
            
            ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 
                    4, 
                    "You are able to hear basic Muse's Whispers.", 
                    "You gain a 1st- or 2nd-level bard feat.", 
                    new Trait[] {FeatArchetype.ArchetypeTrait,DawnniExpanded.DETrait,BardArchetypeTrait})
                    .WithCustomName("Basic Muse's Whispers")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat),"You must have the Bard Dedication feat.")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                    
        {
          
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Basic Muse's Whispers", 
                    "Basic Muse's Whispers feat", 
                    -1, 
                    (Feat ft) => {
                     if (ft.HasTrait(Trait.Bard) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait)){
                    
                    if (ft.CustomName == null){
                    TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);
                      
                    if (FeatwithLevel.Level <= 2){
                      return true;
                    } else return false;

                    } else {
                    TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);
                      
                    if (FeatwithLevel.Level <= 2){
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

            BardBasicSpellcasting =  new TrueFeat(FeatName.CustomFeat, 
                    4, 
                    "You gain the basic spellcasting benefits for Bard.", 
                    "Add a common 1st level occult spell to your repertorie and gain a 1st level spell slot.", 
                    new Trait[] {FeatArchetype.ArchetypeTrait,DawnniExpanded.DETrait,BardArchetypeTrait})
                    .WithCustomName("Basic Bard Spellcasting")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BardDedicationFeat),"You must have the Bard Dedication feat.")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                    
        {
          SpellRepertoire repertoire;
          if (sheet.SpellRepertoires.TryGetValue(Trait.Bard, out repertoire) == false)
          {
          return;
          }

          sheet.AddSelectionOption((SelectionOption) new AddToSpellRepertoireOption("BardSpells", "1st level bard spell", 4, Trait.Bard, Trait.Occult, 1, 1));
          repertoire.SpellSlots[1] += 1;     
        });

            ModManager.AddFeat(BardBasicSpellcasting);
                    
        
          
       
        
        
        
        ModManager.AddFeat(BardDedicationFeat);
    }
}