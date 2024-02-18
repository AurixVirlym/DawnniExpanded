using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded;
public class DawnniExpanded
{
    public static Trait DETrait;
    public static Trait HomebrewTrait;

    [DawnsburyDaysModMainMethod]
    public static void LoadMod()
    {
        DETrait = ModManager.RegisterTrait(
            "DawnniEx",
            new TraitProperties("DawnniEx", true)
            );
        HomebrewTrait = ModManager.RegisterTrait(
            "Homebrew",
            new TraitProperties("Homebrew", true)
            );

        
        SpellAnimatedAssualt.LoadMod();
        SpellScorchingRay.LoadMod();
        SpellEndure.LoadMod();
        SpellFalseLife.LoadMod();
        SpellRousingSplash.LoadMod();
        FeatBattleMedicine.LoadMod();
        FeatPowerfulLeap.LoadMod();
        ActionLeap.LoadMod();
        ItemStaffofSpellPotency.LoadMod();
        TraitMutagens.LoadMod();
        ItemMutagens.LoadMod();
        GenerateHeightenedScrolls.LoadMod();
    }
}