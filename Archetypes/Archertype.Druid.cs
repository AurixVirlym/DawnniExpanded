using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System.Collections.Generic;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Display;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Data;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;






namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeDruid
{

  public static Feat DruidDedicationFeat;
  public static Trait DruidArchetypeTrait;

  public static Feat DruidMatureFeat;

  public static Feat DruidOrderSpell;

  public static Feat DruidBasicSpellcasting;





  public static void LoadMod()

  {


    DruidArchetypeTrait = ModManager.RegisterTrait(
        "DruidArchetype",
        new TraitProperties("DruidArchetype", false, "", false)
        {
        });

    Feat ToOrderArchetype(DruidicOrder order)
    {
      return new Feat(order.OrderFeatName, order.OrderFlavorText, order.OrderGrantedFeat.RulesText, new List<Trait>(), null);
    }

    List<Feat> list = DruidicOrder.CreateDruidicOrders().Select<DruidicOrder, Feat>((Func<DruidicOrder, Feat>)(order => ToOrderArchetype(order))).ToList<Feat>();

    DruidDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have entered a druidic circle and learned a few of the order's secrets, granting you primal power.",
            "You cast spells like a druid and gain the Cast a Spell activity.\n\nYou can prepare two cantrips each day from the primal spell list \n\nYou're trained in spell attack rolls and spell DCs for primal spells. \n\nYour key spellcasting ability for druid archetype spells is wisdom, and they are primal druid spells.\n\nChoose a druidic order. You become a member of that order and are also bound by its specific anathema, allowing you to take the order's feats.\n\nYou become trained in Nature, if you are already trained in Nature, instead become trained in a skill of your choice." + "\n\nYou don't gain any other abilities from your choice of order."
,
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, DruidArchetypeTrait, FeatArchetype.ArchetypeSpellcastingTrait },
            list)
            .WithCustomName("Druid Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Wisdom) >= 14, "You must have at least 14 wisdom")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Druid, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Druid);
      PreparedSpellSlots spellList = new PreparedSpellSlots(Ability.Wisdom, Trait.Primal);
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Druid:Cantrip1"));
      spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "Druid:Cantrip2"));
      sheet.PreparedSpells.Add(Trait.Druid, spellList);



      if (sheet.GetProficiency(Trait.Druid) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Druid, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Spell) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      }

      if (sheet.GetProficiency(Trait.Nature) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Druid Dedication Skill",
                "Druid Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Nature
                )
                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Druid Dedication Skill",
                "Druid Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat
                )
                );
      }


    });




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain a deeper understanding of the wilds.",
            "You gain a 1st- or 2nd-level druid feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, DruidArchetypeTrait })
            .WithCustomName("Basic Wilding")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DruidDedicationFeat), "You must have the Druid Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Wilding",
                  "Basic Wilding feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Druid) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
              {

                if (ft.CustomName == null)
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  else return false;

                }
                else
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  return false;
                }
              }
              return false;
            })
                  );
})

    );

    DruidBasicSpellcasting = new TrueFeat(FeatName.CustomFeat,
            4,
            "You gain the basic spellcasting benefits for Druid.",
            "You may prepare one 1st level spell slot per day.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, DruidArchetypeTrait })
            .WithCustomName("Basic Druid Spellcasting")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DruidDedicationFeat), "You must have the Druid Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{
  PreparedSpellSlots spellList;
  if (sheet.PreparedSpells.TryGetValue(Trait.Druid, out spellList) == false)
  {
    return;
  }
  spellList.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(1, "Druid:Spell1-1"));
});


    DruidOrderSpell = new TrueFeat(FeatName.CustomFeat,
                4,
                "You have delved deeper into the teaching of your order.",
                "You gain the initial order spell from your order. If you don't already have one, you gain a focus pool of 1 Focus Point.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, DruidArchetypeTrait })
                .WithCustomName("Order Spell")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DruidDedicationFeat), "You must have the Druid Dedication feat.").WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

      {
        Feat HasOrderSpell = null;
        SpellId OrderSpell = SpellId.None;

        foreach (Feat item in list)
        {
          HasOrderSpell = sheet.AllFeats.FirstOrDefault(ft => ft.FeatName == item.FeatName);
          if (HasOrderSpell != null)
          {
            IEnumerable<DruidicOrder> OrderstoTakeSpellFrom = DruidicOrder.CreateDruidicOrders();
            OrderSpell = OrderstoTakeSpellFrom.FirstOrDefault(ft => ft.OrderFeatName == HasOrderSpell.FeatName).OrderGrantedSpellId;
            break;
          }
        }

        if (OrderSpell != SpellId.None)
        {
          sheet.AddFocusSpellAndFocusPoint(Trait.Druid, Ability.Wisdom, OrderSpell);
        }
      }); ;



    DruidMatureFeat = new TrueFeat(FeatName.CustomFeat,
                    4,
                    "Your animal companions has grown up, becoming a mature animal companion and gaining additional capabilities.",
                    "The following increases are applied to your animal companion:"
                    + "\n\n- Strength, Dexterity, Constitution, and Wisdom modifiers increase by 1."
                    + "\n- Unarmed attack damage increases from one die to two dice."
                    + "\n- Proficiency rank for Perception and all saving throws increases to expert."
                    + "\n- Proficiency ranks in Intimidation, Stealth, and Survival increase to trained, and if it was already trained in one of those skills from its type, the proficiency rank in that skill increases to expert."
                    + "\n\nEven if you don't use the Command an Animal action, your animal companion can still use 1 action at the end of your turn.",
                    new Trait[] { Trait.Druid, DawnniExpanded.DETrait })
                    .WithCustomName("Mature Animal Companion (Druid)")
                    .WithEquivalent(values => values.AllFeats.Contains(ArchetypeBeastmaster.BeastMasterMatureFeat))
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(FeatName.AnimalCompanion), "You have selected an Animal Companion.")
                    .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                    {
                      sheet.RangerBenefitsToCompanion += (Action<Creature, Creature>)((companion, ranger) =>
                          {
                            companion.MaxHP += companion.Level;
                            companion.Abilities.Strength += 1;
                            companion.Abilities.Dexterity += 1;
                            companion.Abilities.Constitution += 1;
                            companion.Abilities.Wisdom += 1;
                            if (companion.UnarmedStrike.WeaponProperties.DamageDieCount == 1)
                            {
                              companion.UnarmedStrike.WeaponProperties.DamageDieCount += 1;
                            }


                            foreach (QEffect qf in companion.QEffects.Where<QEffect>(qf => qf.AdditionalUnarmedStrike != null))
                            {
                              if (qf.AdditionalUnarmedStrike.WeaponProperties.DamageDieCount == 1)
                              {
                                qf.AdditionalUnarmedStrike.WeaponProperties.DamageDieCount += 1;
                              }
                            }

                            companion.Perception += 2;
                            companion.Proficiencies.Set(Trait.Perception, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Fortitude, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Will, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Reflex, Proficiency.Expert);

                            if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Trained)
                            {
                              sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                            }
                            else if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Untrained)
                            {
                              sheet.SetProficiency(Trait.Survival, Proficiency.Trained);
                            }

                            if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Trained)
                            {
                              sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                            }
                            else if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Untrained)
                            {
                              sheet.SetProficiency(Trait.Intimidation, Proficiency.Trained);
                            }

                            if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Trained)
                            {
                              sheet.SetProficiency(Trait.Stealth, Proficiency.Expert);
                            }
                            else if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Untrained)
                            {
                              sheet.SetProficiency(Trait.Stealth, Proficiency.Trained);
                            }

                          });

                    }

                    ).WithPermanentQEffect("If you don't command your companion, they will act with 1 action at end of your turn.",
                    (qf => qf.EndOfYourTurn = (Func<QEffect, Creature, Task>)(async (qfSelf, you) =>
                        {
                          Creature animalCompanion = you.Battle.AllCreatures.FirstOrDefault<Creature>((Func<Creature, bool>)(cr => cr.QEffects.Any<QEffect>((Func<QEffect, bool>)(qf => qf.Id == QEffectId.RangersCompanion && qf.Source == you)) && cr.Actions.CanTakeActions()));

                          if (animalCompanion == null)
                          {

                            return;
                          }


                          if (!you.Actions.ActionHistoryThisTurn.Any<CombatAction>((Func<CombatAction, bool>)(ac => ac.Name == "Command your Animal Companion" || ac.ActionId == ActionId.Delay)))
                          {
                            you.Occupies.Overhead("Mature Companion.", Color.Green);
                            animalCompanion.AddQEffect(new QEffect()
                            {
                              ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn,
                              StartOfYourTurn = (Func<QEffect, Creature, Task>)(async (effect, creature) =>
                                    {
                                      creature.Actions.UseUpActions(1, ActionDisplayStyle.Summoned);
                                      return;
                                    })
                            });
                            await CommonSpellEffects.YourMinionActs(animalCompanion);

                          }

                        })
                    ));

    ModManager.AddFeat(DruidMatureFeat);
    ModManager.AddFeat(DruidOrderSpell);
    ModManager.AddFeat(DruidBasicSpellcasting);
    ModManager.AddFeat(DruidDedicationFeat);
  }
}