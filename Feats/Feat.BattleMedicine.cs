using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core;
using Dawnsbury.Core.Roller;
using Dawnsbury.Audio;
using Dawnsbury.Display.Illustrations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;


namespace Dawnsbury.Mods.DawnniExpanded;

public class FeatBattleMedicine


{
    public static async Task BattleMedicineAdjacentCreature(Creature self)
    {

        List<Option> options = new List<Option>();

        CombatAction trainedBM = BattleMedAction(self, false);
        CombatAction expertBM = BattleMedAction(self, true);

        trainedBM.WithActionCost(0);
        expertBM.WithActionCost(0);

        trainedBM.Owner = self;
        expertBM.Owner = self;

        GameLoop.AddDirectUsageOnCreatureOptions(trainedBM, options, noConfirmation: false);
        GameLoop.AddDirectUsageOnCreatureOptions(expertBM, options, noConfirmation: false);



        if (options.Count > 0)
        {
            if (options.Count == 1)
            {
                await options[0].Action();
                return;
            }

            await (await self.Battle.SendRequest(new AdvancedRequest(self, "Choose a creature to Strike.", options)
            {
                TopBarText = "Choose a creature to use Battle Medicine on.",
                TopBarIcon = new ModdedIllustration("DawnniburyExpandedAssets/BattleMedicine.png")
            })).ChosenOption.Action();
        }
    }

    public static CombatAction ExpertBattleMedAction;
    private static CombatAction BattleMedAction(Creature creature, bool expert = false)
    {

        CombatAction BattleMedAction = new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/BattleMedicine.png"), "Battle Medicine (Trained)", new Trait[] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, Trait.Basic, Trait.AttackDoesNotTargetAC, DawnniExpanded.DETrait },
                                  "Attempt a {b}DC 15 Medicine check{/b} targeting an adjacent ally or yourself. The target is then temporarily immune to your Battle Medicine until long rest.\n\n{b}Critical Success{/b} The target regains 4d8 Hit Points.\n{b}Success{/b} The target regains 2d8 Hit Points.\n{b}Critical Failure{b}: The target takes 1d8 daamge.",
                                      Target.AdjacentFriendOrSelf()
                                  .WithAdditionalConditionOnTargetCreature((a, d) =>
                                  {

                                      if (!a.HasFreeHand)
                                      {
                                          return Usability.CommonReasons.NoFreeHandForManeuver;
                                      }

                                      if (d.HP == d.MaxHP)
                                      {
                                          return Usability.NotUsableOnThisCreature("Target is full on HP.");
                                      }

                                      if (!a.PersistentUsedUpResources.UsedUpActions.Contains("BattleMedicine:" + d.Name))
                                      {
                                          return Usability.Usable;
                                      }
                                      else if (!a.PersistentUsedUpResources.UsedUpActions.Contains("BattleMedicineMedicArchetypePassed") && a.PersistentCharacterSheet.Calculated.AllFeats.Contains(ArchetypeMedic.MedicDedicationFeat))
                                      {
                                          return Usability.Usable;
                                      }

                                      return Usability.NotUsableOnThisCreature("Target has been already affected by your Battle Medicine today.");
                                  })
                                  )
                                  .WithSoundEffect(SfxName.Healing)
                                  .WithActionCost(1)
                                  .WithActiveRollSpecification(
                              new ActiveRollSpecification(Checks.SkillCheck(Skill.Medicine), Checks.FlatDC(15)))
                              .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                              {

                                  if (creature.PersistentUsedUpResources.UsedUpActions.Contains("BattleMedicine:" + target.Name) && caster.PersistentCharacterSheet.Calculated.AllFeats.Contains(ArchetypeMedic.MedicDedicationFeat))
                                  {
                                      creature.PersistentUsedUpResources.UsedUpActions.Add("BattleMedicineMedicArchetypePassed");
                                      caster.Occupies.Overhead("Medic Dedication bypasses Immunity", Color.White);
                                  }
                                  else
                                  {
                                      creature.PersistentUsedUpResources.UsedUpActions.Add("BattleMedicine:" + target.Name);
                                  };


                                  string HealModifier = "";

                                  if (expert == true)
                                  {
                                      HealModifier = "10";


                                      if (creature.HasFeat(ArchetypeMedic.MedicDedicationFeat.FeatName))
                                      {
                                          HealModifier = "+15";
                                      }
                                  }

                                  if (result == CheckResult.CriticalSuccess)
                                  {
                                      target.Heal("4d8" + HealModifier, spell);

                                  }
                                  else if (result == CheckResult.Success)
                                  {

                                      target.Heal("2d8" + HealModifier, spell);

                                  }
                                  else if (result == CheckResult.Failure)
                                  {

                                  }
                                  if (result == CheckResult.CriticalFailure)
                                  {

                                      DiceFormula damage = DiceFormula.FromText("1d8", spell.Name);
                                      await caster.DealDirectDamage(spell, damage, target, CheckResult.Success, DamageKind.Untyped);
                                  }
                              });

        if (expert == true)
        {
            BattleMedAction.Name = "Battle Medicine (Expert)";
            BattleMedAction.Description = "Attempt a {b}DC 20 Medicine check{/b} targeting an adjacent ally or yourself. The target is then temporarily immune to your Battle Medicine until long rest.\n\n{b}Critical Success{/b} The target regains 4d8+10 Hit Points.\n{b}Success{/b} The target regains 2d8+10 Hit Points.\n{b}Critical Failure{/b} The target takes 1d8 daamge.";
            BattleMedAction.WithActiveRollSpecification(
                       new ActiveRollSpecification(Checks.SkillCheck(Skill.Medicine), Checks.FlatDC(20)));
        }

        return BattleMedAction;
    }

    public static Feat BattleMedicineTrueFeat;

    public static void LoadMod()
    {
        BattleMedicineTrueFeat = new TrueFeat(FeatName.CustomFeat, 1,
                "You can patch up wounds, even in combat.",
                "Attempt a Medicine check with the same DC as for Treat Wounds and restore the corresponding amount of HP; this doesn't remove the wounded condition. As with Treat Wounds, you can attempt checks against higher DCs if you have the minimum proficiency rank.\n\nIf you're an expert in Medicine, you can instead attempt a DC 20 check to increase the Hit Points regained by 10.\n\nThe target is then temporarily immune to your Battle Medicine until long rest.\n\nIt is assumed you have a healer's kit as long as you have a hand free otherwise you may not use Treat Wounds." + " Expanded healer's tools adds a +1 item bonus to your check.",
                new[] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, DawnniExpanded.DETrait }
                )
            .WithActionCost(1)
            .WithCustomName("Battle Medicine{icon:Action}")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Medicine) >= Proficiency.Trained, "You must be trained in Medicine.")
            .WithOnCreature((sheet, creature) =>
            {


                creature.AddQEffect(new QEffect("Battle Medicine", "You can patch up wounds, even in combat")
                {

                    ProvideActionIntoPossibilitySection = (QEffect qfself, PossibilitySection possibilitySection1) =>
                     {
                         if (possibilitySection1.PossibilitySectionId != PossibilitySectionId.SkillActions)
                         {
                             return null;
                         }

                         CombatAction TrainedBattleMedAction = BattleMedAction(creature, false);
                         ActionPossibility TrainedBattleMed = TrainedBattleMedAction;


                         SubmenuPossibility submenuPossibility = new SubmenuPossibility(IllustrationName.Heal, "Battle Medicine");
                         PossibilitySection possibilitySection = new PossibilitySection("Battle Medicine");
                         possibilitySection.AddPossibility(TrainedBattleMed);

                         if (sheet.GetProficiency(Trait.Medicine) >= Proficiency.Expert)
                         {
                             CombatAction ExpertBattleMedAction = BattleMedAction(creature, true);
                             ActionPossibility ExpertBattleMed = ExpertBattleMedAction;
                             possibilitySection.AddPossibility(ExpertBattleMed);
                         }

                         submenuPossibility.Subsections.Add(possibilitySection);

                         return submenuPossibility;
                     }

                })
                    ;


            });

        ModManager.AddFeat(BattleMedicineTrueFeat);






    }
}