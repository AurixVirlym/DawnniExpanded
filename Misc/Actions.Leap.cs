using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Core.Animations.Movement;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ActionLeap
{

    public static void LoadMod()
    {


        ModManager.RegisterActionOnEachCreature(creature =>
        {
            // We add an effect to every single creature...
            creature.AddQEffect(
                new QEffect()
                {
                    ProvideActionIntoPossibilitySection = (qfself, possibilitySection1) =>
                        {
                            if (possibilitySection1.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                            {
                                return null;
                            }

                            CombatAction leapAction = new CombatAction(creature, IllustrationName.WarpStep, "Leap", new Trait[] { Trait.Move, Trait.Basic, Trait.Athletics, DawnniExpanded.DETrait },
                                "You take a careful, short jump. \n\nYou can Leap up to 10 feet horizontally if your Speed is at least 15 feet, or up to 15 feet horizontally if your Speed is at least 30 feet.\n\nYou land in the space where your Leap ends (meaning you can typically clear a 5-foot gap, or a 10-foot gap if your Speed is 30 feet or more).",
                                 new TileTarget((Creature caster, Tile tile) =>
                                {
                                    if (tile.IsTrulyGenuinelyFreeTo(caster))
                                    {
                                        int? nullable = caster.Occupies?.DistanceTo(tile);
                                        int num = 2;
                                        if (caster.Speed == 6)
                                        {
                                            num = 3;
                                        }
                                        if (caster.QEffects.Any(qf => qf.Name == "Powerful Leap"))
                                        {
                                            num += 1;
                                        }

                                        if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
                                            if (caster.Occupies.HasLineOfEffectToIgnoreLesser(tile) <= CoverKind.Lesser)
                                            {
                                                return true;
                                            }
                                            else return false;
                                    }
                                    return false;
                                }, null)

                                ).WithEffectOnChosenTargets(async (action, caster, target) =>
                                {
                                    QEffect LeapingFlying = QEffect.Flying();
                                    LeapingFlying.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                                    LeapingFlying.DoNotShowUpOverhead = true;

                                    caster.AddQEffect(LeapingFlying);

                                    await caster.MoveTo(target.ChosenTile, action, new MovementStyle()
                                    {
                                        Shifting = true,
                                        ShortestPath = true,
                                        MaximumSquares = 100
                                    });
                                })

                                .WithActionCost(1);

                            ActionPossibility leapActionPossibility = leapAction;



                            return leapActionPossibility;
                        }
                });
        });




    }
}