using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System.Collections.Generic;
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


        List<Trait> ClassTraits = new List<Trait>();

        Trait[] DedicationFeatTraits = new Trait[] { ArchetypeTrait, DedicationTrait, Trait.ClassFeat, DawnniExpanded.DETrait, SpellHexes.ClassTrait };

        Trait[] ArchetypeFeatTraits = new Trait[] { ArchetypeTrait, Trait.ClassFeat, DawnniExpanded.DETrait, SpellHexes.ClassTrait };

        AllFeats.All.ForEach(ft =>
        { // Loop through all feats.
            if (ft is ClassSelectionFeat classFeat)
            { // If the feat is a classFeat,
                ClassTraits.Add(classFeat.ClassTrait);
            }
        });

        ArchetypeFeatTraits = ArchetypeFeatTraits.Concat(ClassTraits).ToArray();
        DedicationFeatTraits = DedicationFeatTraits.Concat(ClassTraits).ToArray();

        {
            DedicationFeat = new TrueFeat(FeatName.CustomFeat,
                    2,
                    "Instead of a class feat, you gain an archetype dedication feat of your choice. You may have only one archetype.",
                    "You gain an archetype dedication feat.",
                    DedicationFeatTraits)
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


            ArchetypeFeat = new TrueFeat(FeatName.CustomFeat,
                        4,
                        "Instead of a class feat, you gain an archetype feat of your choice for your dedication.",
                        "You gain an archetype feat.",
                       ArchetypeFeatTraits)
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


            NoneFeat = new TrueFeat(FeatName.CustomFeat,
                        1,
                        "",
                        "This feat exists as a option when you don't want to take a real option or can't.",
                        new Trait[] { DawnniExpanded.DETrait })
                        .WithMultipleSelection()
                        .WithCustomName("None");




        };

        ModManager.AddFeat(ArchetypeFeat);
        ModManager.AddFeat(DedicationFeat);
        ModManager.AddFeat(NoneFeat);

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
        ArchetypeFamiliarMaster.LoadMod();
    }
}