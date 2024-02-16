using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Possibilities;
using System;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.Creatures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics.Treasure;

namespace Dawnsbury.Mods.DawnniExpanded;
public class DawnniExpanded
{
    public static Trait DETrait;

    [DawnsburyDaysModMainMethod]
    public static void LoadMod()
    {
        DETrait = ModManager.RegisterTrait(
            "DawnniEx",
            new TraitProperties("DawnniEx", true)
            );

        SpellAnimatedAssualt.LoadMod();
        SpellScorchingRay.LoadMod();
        FeatBattleMedicine.LoadMod();
        FeatPowerfulLeap.LoadMod();
        ActionLeap.LoadMod();
        ItemStaffofSpellPotency.LoadMod();
        TraitMutagens.LoadMod();
        ItemMutagens.LoadMod();
    }
}