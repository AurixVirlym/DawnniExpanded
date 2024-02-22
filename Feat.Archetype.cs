using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Selections.Selected;

using System;
using System.Linq;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class FeatArchetype
{
    public static Trait DedicationTrait;
    public static Trait ArchetypeTrait;
    public static void LoadMod()

    
    {
        DedicationTrait = ModManager.RegisterTrait(
            "Dedication",
            new TraitProperties("Dedication", true, "", false)
            {
            });
        
        ArchetypeTrait = ModManager.RegisterTrait(
            "Archetype",
            new TraitProperties("Archetype", true, "", false)
            {
            });
        
    
        {
            ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 
                    2, 
                    "Instead of a class feat, you gain an archetype dedication feat of your choice. You may have only one archetype.", 
                    "You gain an archetype dedication feat.", 
                    new Trait[] { ArchetypeTrait, DedicationTrait ,Trait.ClassFeat,Trait.Sorcerer,Trait.Rogue,Trait.Fighter,Trait.Wizard,Trait.Monk,Trait.Investigator,Trait.Cleric,Trait.Kineticist,Trait.Psychic,Trait.Barbarian,Trait.Magus,Trait.Rogue,Trait.Ranger,DawnniExpanded.DETrait})
                    .WithCustomName("Archetype Dedication")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                    
                    
        {
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Archetype Dedication", 
                    "Archetype Dedication feat", 
                    -1, 
                    (Feat ft) => ft.HasTrait(DedicationTrait) && ft.CustomName != "Archetype Dedication"));
        })
            
            );
        
        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 
                    4, 
                    "Instead of a class feat, you gain an archetype feat of your choice for your dedication.", 
                    "You gain an archetype feat.", 
                    new Trait[] { ArchetypeTrait, Trait.ClassFeat,Trait.Sorcerer,Trait.Rogue,Trait.Fighter,Trait.Wizard,Trait.Monk,Trait.Investigator,Trait.Cleric,Trait.Kineticist,Trait.Psychic,Trait.Barbarian,Trait.Magus,Trait.Rogue,Trait.Ranger,DawnniExpanded.DETrait})
                    .WithMultipleSelection()
                    .WithCustomName("Archetype Feat")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                    
        {
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Archetype", 
                    "Archetype feat", 
                    -1, 
                    (Feat ft) => ft.HasTrait(ArchetypeTrait) && !ft.HasTrait(DedicationTrait) && ft.CustomName != "Archetype Feat"));
        })
            
            );


    };

        ArchetypeMedic.LoadMod();
        ArchetypeFighter.LoadMod();
        ArchetypeMonk.LoadMod();
        ArchetypeRogue.LoadMod();
        ArchetypeRanger.LoadMod();
        ArchetypeBarbarian.LoadMod();
        ArchetypeSentinel.LoadMod();
        ArchetypeDuelist.LoadMod();
    }
}