using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;





namespace Dawnsbury.Mods.DawnniExpanded;



public static class TraitMutagens
{
    public static Trait MutagenTrait;
    public static Trait PolymorphTrait;

    public static void PreventMutagenDrinking(QEffect qf)
    {
        qf.PreventTargetingBy = newAttack => newAttack.Traits.Contains(TraitMutagens.PolymorphTrait) == true && newAttack.ActionId == ActionId.Administer ? "Target is already under a Polymorph effect" : null;
        qf.PreventTakingAction = newAttack => newAttack.Traits.Contains(TraitMutagens.PolymorphTrait) == true && newAttack.ActionId == ActionId.Drink ? "Target is already under a Polymorph effect" : null;
        qf.CountsAsABuff = true;
    }


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