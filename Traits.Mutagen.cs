using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Roller;




namespace Dawnsbury.Mods.DawnniExpanded;

public static class TraitMutagens
{
    public static Trait MutagenTrait;
    public static Trait PolymorphTrait;
    public static void LoadMod()
    {
        MutagenTrait = ModManager.RegisterTrait(
            "Mutagen",
            new TraitProperties("Mutagen", true, "An elixir with the mutagen trait temporarily transmogrifies the subject's body and alters its mind. A mutagen always conveys one or more beneficial effects paired with one or more detrimental effects. Mutagens are polymorph effects, meaning you can benefit from only one at a time.", true)
            {
            });
        PolymorphTrait = ModManager.RegisterTrait(
            "Polymorph",
            new TraitProperties("Polymorph", true, "These effects transform the target into a new form. A target can't be under the effect of more than one polymorph effect at a time.", false)
            {
            });
    }
}