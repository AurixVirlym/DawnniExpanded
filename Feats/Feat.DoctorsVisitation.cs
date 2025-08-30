using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded.Feats;

public class FeatDoctorsVisitation
{
    public static void LoadMod()
    {
        TrueFeat doctorsVisitation = new(ModManager.RegisterFeatName("DE_DoctorsVisitation", "Doctor's Visitation"), 4, "You move to provide immediate care to those who need it.",
            "Stride, then use Battle Medicine or Treat Poison. You can spend 2 actions for Doctor’s Visitation to instead Stride and then use Stabilize, Staunch Bleeding, or Treat Condition (if you have that feat).",
            [Trait.Flourish, DawnniExpanded.DETrait]);
        DoctorsVisitationLogic(doctorsVisitation);
        ModManager.AddFeat(doctorsVisitation);
    }
    public static void DoctorsVisitationLogic(TrueFeat feat)
    {
        feat.WithAvailableAsArchetypeFeat(Trait.Medic).WithActionCost(-3)
            .WithPermanentQEffect("Stride, then use Battle Medicine or Treat Poison. If you spend 2 actions, you can Stabilize, Staunch Bleeding, or Treat Condition.", qf =>
            {
                qf.Name = "Doctor's Visitation {icon:Action}–{icon:TwoActions}";
                qf.ProvideMainAction = _ =>
                {
                    return Possibilities.CreateSpellPossibility(new CombatAction(qf.Owner, new SideBySideIllustration(IllustrationName.FleetStep, IllustrationName.HealersTools), "Doctor's Visitation",
                        [Trait.Basic, Trait.Flourish, DawnniExpanded.DETrait], "{i}You move to provide immediate care to those who need it.{/i}\n\nStride, then use Battle Medicine or Treat Poison. You can spend 2 actions for Doctor’s Visitation to instead Stride and then use Stabilize, Staunch Bleeding, or Treat Condition (if you have that feat).",
                        Target.DependsOnActionsSpent(Target.Self(), Target.Self(), null!))
                        .WithCreateVariantDescription((actionCost, _) =>
                        {
                            return actionCost switch
                            {
                                1 => "Stride, then use Battle Medicine or Treat Poison.",
                                2 => "Stride, then use Stabilize, Staunch Bleeding or Treat Condition.",
                                _ => "exception"
                            };
                        })
                        .WithActionCost(-3).WithEffectOnChosenTargets(async (spell, caster, _) =>
                        {
                            if (!await caster.StrideAsync("Make a stride.", allowCancel: true, allowStep: false))
                            {
                                spell.RevertRequested = true;
                                return;
                            }
                            switch (spell.SpentActions)
                            {
                                case 1:
                                {
                                    Possibilities possibles = caster.Possibilities.Filter(ap =>
                                    {
                                        if (!ap.CombatAction.Name.Contains("Battle Medicine") &&
                                            !ap.CombatAction.Name.Contains("Treat Poison"))
                                            return false;
                                        ap.CombatAction.ActionCost = 0;
                                        ap.RecalculateUsability();
                                        return true;
                                    });
                                    List<Option> options = await caster.Battle.GameLoop.CreateActions(caster, possibles, null);
                                    switch (possibles.CreateActions(true).Count)
                                    {
                                        case 0:
                                            spell.RevertRequested = true;
                                            caster.Actions.UseUpActions(1, ActionDisplayStyle.UsedUp);
                                            return;
                                        case >= 1:
                                            await caster.Battle.GameLoop.OfferOptions(caster, options, true);
                                            break;
                                    }
                                    break;
                                }
                                case 2:
                                    {
                                    Possibilities possibles = caster.Possibilities.Filter(ap =>
                                    {
                                        if ((!ap.CombatAction.Name.Contains("Treat ") &&
                                             !ap.CombatAction.Name.Contains("Stabilize") &&
                                             !ap.CombatAction.Name.Contains("Staunch bleeding"))
                                            || ap.CombatAction.ActionId == ActionId.TreatPoison)
                                            return false;
                                        ap.CombatAction.ActionCost = 0;
                                        ap.RecalculateUsability();
                                        return true;
                                    });
                                    List<Option> options = await caster.Battle.GameLoop.CreateActions(caster, possibles, null);
                                    switch (possibles.CreateActions(true).Count)
                                    {
                                        case 0:
                                            spell.RevertRequested = true;
                                            caster.Actions.UseUpActions(1, ActionDisplayStyle.UsedUp);
                                            return;
                                        case >= 1:
                                            await caster.Battle.GameLoop.OfferOptions(caster, options, true);
                                            break;
                                    }
                                    break;
                                }
                            }
                        })).WithPossibilitySize(PossibilitySize.Full);
                };
            });
    }
}