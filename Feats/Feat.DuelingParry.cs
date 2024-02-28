using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Possibilities;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class FeatDuelingParry
{
  public static Feat FeatDuelingParryFighter;
  public static void LoadMod()

  {
    FeatDuelingParryFighter = new TrueFeat(FeatName.CustomFeat,
                2,
                "You can parry attacks against you with your one-handed weapon",
                "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.",
                new Trait[] { Trait.Fighter, DawnniExpanded.DETrait })
                .WithOnCreature((sheet, creature) =>
      {

        QEffect DuellingParryHolder = new QEffect()
        {
          Name = "Dueling Parry Granter",
          ProvideMainAction = (qfTechnical =>
          {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return new ActionPossibility
                    (new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"), "Dueling Parry", new Trait[] { Trait.Basic, DawnniExpanded.DETrait },
                              "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.",
                                  Target.Self()
                              .WithAdditionalRestriction((a) =>
                              {

                                if (a.QEffects.Any((QEffect x) => x.Name == "Dueling Parry"))
                                {
                                  return "Already parrying.";
                                };

                                if (a.HasFreeHand)
                                {
                                  if (a.HeldItems.FirstOrDefault() == null)
                                  {
                                    return "Not holding a Melee Weapon";
                                  }
                                  if (!a.HeldItems.First().HasTrait(Trait.Melee))
                                  {
                                    return "Not holding a Melee Weapon";
                                  }
                                  else return null;

                                }
                                else return "No Free Hand.";


                              })
                              )
                              .WithSoundEffect(SfxName.RaiseShield)
                              .WithActionCost(1)
                              .WithEffectOnSelf(async (spell, caster) =>
                          {
                            QEffect DuellingParryEffect = new QEffect()
                            {
                              Name = "Dueling Parry",
                              DoNotShowUpOverhead = true,
                              Owner = caster,
                              Description = "+2 circumstance bonus to AC until the start of your next turn.",
                              Illustration = new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"),
                              BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) =>
                                {
                                  if (defense == Defense.AC)
                                  {
                                    return new Bonus(2, BonusType.Circumstance, "Dueling Parry");
                                  }
                                  else return null;
                                },
                              ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn,
                              StateCheck = Qfduel =>
                              {
                                if (Qfduel.Owner.HeldItems.FirstOrDefault() == null)
                                {
                                  Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                }
                                else if (!Qfduel.Owner.HasFreeHand || !Qfduel.Owner.HeldItems.First().HasTrait(Trait.Melee) || Qfduel.Owner.HasEffect(QEffectId.Unconscious) || Qfduel.Owner.HasEffect(QEffectId.Dying))
                                {
                                  Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                }

                              }
                            };
                            caster.AddQEffect(DuellingParryEffect);

                          }));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
          }
        )

        };
        creature.AddQEffect(DuellingParryHolder);
      })
                .WithCustomName("Dueling Parry{icon:Action}")
                .WithEquivalent(values => values.AllFeats.Contains(ArchetypeDuelist.DuelingParryFeat));



    ModManager.AddFeat(FeatDuelingParryFighter);
  }
}