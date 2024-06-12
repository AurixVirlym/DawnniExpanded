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

    public static Trait ArchetypeSpellcastingTrait;

    public static Feat DedicationFeat;
    public static Feat ArchetypeFeat;

    public static Feat NoneFeat;
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

        ArchetypeSpellcastingTrait = ModManager.RegisterTrait(
            "ArchetypeSpellcasting",
            new TraitProperties("ArchetypeSpellcasting", false, "", false)
            {
            });




        {
            DedicationFeat = new TrueFeat(FeatName.CustomFeat,
                    2,
                    "Instead of a class feat, you gain an archetype dedication feat of your choice. You may have only one archetype.",
                    "You gain an archetype dedication feat.",
                    new Trait[] { ArchetypeTrait, DedicationTrait, Trait.ClassFeat, Trait.Bard, Trait.Sorcerer, Trait.Rogue, Trait.Fighter, Trait.Wizard, Trait.Monk, Trait.Investigator, Trait.Cleric, Trait.Kineticist, Trait.Psychic, Trait.Barbarian, Trait.Magus, Trait.Rogue, Trait.Ranger, DawnniExpanded.DETrait })
                    .WithCustomName("Archetype Dedication")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)



        {
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Archetype Dedication",
                    "Archetype Dedication feat",
                    -1,
                    (Feat ft) => ft.HasTrait(DedicationTrait) && ft.CustomName != "Archetype Dedication"));
        });

            ModManager.AddFeat(DedicationFeat);





            ArchetypeFeat = new TrueFeat(FeatName.CustomFeat,
                        4,
                        "Instead of a class feat, you gain an archetype feat of your choice for your dedication.",
                        "You gain an archetype feat.",
                        new Trait[] { ArchetypeTrait, Trait.ClassFeat, Trait.Bard, Trait.Sorcerer, Trait.Rogue, Trait.Fighter, Trait.Wizard, Trait.Monk, Trait.Investigator, Trait.Cleric, Trait.Kineticist, Trait.Psychic, Trait.Barbarian, Trait.Magus, Trait.Rogue, Trait.Ranger, DawnniExpanded.DETrait })
                        .WithMultipleSelection()
                        .WithCustomName("Archetype Feat")
                        .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Any(Ft => Ft.HasTrait(DedicationTrait)), "You must have a Dedication feat.")
                        .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Archetype",
                        "Archetype feat",
                        -1,
                        (Feat ft) => (ft.HasTrait(ArchetypeTrait) && !ft.HasTrait(DedicationTrait) && ft.CustomName != "Archetype Feat") || ft.CustomName == "None")
                        );

            });

            ModManager.AddFeat(ArchetypeFeat);

            NoneFeat = new TrueFeat(FeatName.CustomFeat,
                        1,
                        "",
                        "This feat exists as a option when you don't want to take a real option or can't.",
                        new Trait[] { DawnniExpanded.DETrait })
                        .WithMultipleSelection()
                        .WithCustomName("None");

            ModManager.AddFeat(NoneFeat);



        };

        ArchetypeMedic.LoadMod();
        ArchetypeFighter.LoadMod();
        ArchetypeMonk.LoadMod();
        ArchetypeRogue.LoadMod();
        ArchetypeRanger.LoadMod();
        ArchetypeBarbarian.LoadMod();
        ArchetypeSentinel.LoadMod();
        ArchetypeDuelist.LoadMod();
        ArchetypeBeastmaster.LoadMod();
        ArchetypeAlchemist.LoadMod();
        ArchetypeBard.LoadMod();
        ArchetypePsychic.LoadMod();
        ArchetypeWizard.LoadMod();
        ArchetypeCleric.LoadMod();
        ArchetypeSorcerer.LoadMod();
        ArchetypeDualWeaponWarrior.LoadMod();
        ArchetypeWrestler.LoadMod();
        ArchetypeDruid.LoadMod();
    }
}