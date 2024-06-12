using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder;
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
using Dawnsbury.Display.Text;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;



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
        string text = "";
        int num = (expert ? 20 : 15);
        if (creature.HasFeat(ArchetypeMedic.MedicDedicationFeat.FeatName))
        {
            text = (expert ? "+15" : "");
        }
        else text = (expert ? "+10" : "");

        CombatAction BattleMedAction = new CombatAction(creature, IllustrationName.HealersTools, "Battle Medicine" + (expert ? " (DC 20)" : ""), new Trait[4] {
            Trait.Healing,
            Trait.Manipulate,
            Trait.Basic,
            DawnniExpanded.DETrait },
                                  $"{{b}}Range{{/b}} touch\n{{b}}Requirements{{/b}}You must have a hand free.\n\nMake a Medicine check against DC {num}." + S.FourDegreesOfSuccess("The target regains 4d8" + text + " HP.", "The target regains 2d8" + text + " HP.", null, "The target takes 1d8 damage.") + "\n\nRegardless of your result, the target is then temporarily immune to your Battle Medicine for the rest of the day.",
                                      Target.AdjacentFriendOrSelf()
                                  .WithAdditionalConditionOnTargetCreature((a, d) =>
                                  {

                                      if (!a.HasFreeHand)
                                      {
                                          return Usability.CommonReasons.NoFreeHandForManeuver;
                                      }

                                      if (d.Damage == 0)
                                      {
                                          return Usability.NotUsableOnThisCreature("healthy");
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

                                  if (result == CheckResult.CriticalFailure)
                                  {
                                      await caster.DealDirectDamage(spell, DiceFormula.FromText("1d8", "Battle Medicine (critical failure)"), target, CheckResult.Failure, DamageKind.Slashing);
                                  }

                                  if (result >= CheckResult.Success)
                                  {
                                      DiceFormula diceFormula = ((result == CheckResult.CriticalSuccess) ? DiceFormula.FromText("4d8", "Battle Medicine (critical success)") : DiceFormula.FromText("2d8", "Battle Medicine"));
                                      if (expert)
                                      {
                                          diceFormula = diceFormula.Add(DiceFormula.FromText("10", "Battle Medicine (expert)"));

                                          if (creature.HasFeat(ArchetypeMedic.MedicDedicationFeat.FeatName))
                                          {
                                              diceFormula = diceFormula.Add(DiceFormula.FromText("5", "Medic Dedication (expert)"));
                                          }
                                      }

                                      target.Heal(diceFormula, spell);
                                      Sfxs.Play(SfxName.Healing);
                                  }

                              });

        BattleMedAction.WithActiveRollSpecification(
                   new ActiveRollSpecification(Checks.SkillCheck(Skill.Medicine), Checks.FlatDC(num)));


        return BattleMedAction;
    }

    public static Feat BattleMedicineTrueFeat;

    public static void LoadMod()
    {




        BattleMedicineTrueFeat = new TrueFeat(FeatName.BattleMedicine, 1,
              "You can patch up wounds, even in combat.",
               "{b}Range{/b} touch\n{b}Requirements{/b} You must have a hand free.\n\nMake a Medicine check against DC 15." + S.FourDegreesOfSuccess("The target regains 4d8 HP.", "The target regains 2d8 HP.", (string)null, "The target takes 1d8 damage.") + "\n\nRegardless of your result, the target is then temporarily immune to your Battle Medicine for the rest of the day.\n\nIf you're expert in Medicine, you can choose to make the check against DC 20. If you do, you heal 2d8+10 HP on a success instead (4d8+10 HP on a critical success).",
              new Trait[5] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, DawnniExpanded.DETrait }
              )
          .WithActionCost(1)
          .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Medicine) >= Proficiency.Trained, "You must be trained in Medicine.").WithPermanentQEffect("You can heal allies as an 'other maneuver'.", (qf => qf.ProvideActionIntoPossibilitySection = ((qfBattleMedicine, section) =>
    {
        if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
            return (Possibility)null;
        CharacterSheet persistentCharacterSheet = qfBattleMedicine.Owner.PersistentCharacterSheet;
        if (persistentCharacterSheet == null || persistentCharacterSheet.Calculated.GetProficiency(Trait.Medicine) < Proficiency.Expert)
            return (Possibility)new ActionPossibility(BattleMedAction(qfBattleMedicine.Owner, false));
        return (Possibility)new SubmenuPossibility((Illustration)IllustrationName.HealersTools, "Battle Medicine")
        {
            Subsections = {
            new PossibilitySection("Battle Medicine")
            {
              Possibilities = {
                (Possibility) (ActionPossibility) BattleMedAction(qfBattleMedicine.Owner, false),
                (Possibility) (ActionPossibility) BattleMedAction(qfBattleMedicine.Owner, true)
              }
            }
        }
        };
    })));


        /*.WithOnCreature((sheet, creature) =>
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


        });*/
        AllFeats.All.RemoveAll(feat => feat.FeatName == FeatName.BattleMedicine);
        ModManager.AddFeat(BattleMedicineTrueFeat);






    }
}