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


namespace Dawnsbury.Mods.DawnniExpanded;

public class FeatBattleMedicine
{
    public static void LoadMod()
    {

        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 1,
                "You can patch up wounds, even in combat.",
                "Attempt a Medicine check with the same DC as for Treat Wounds and restore the corresponding amount of HP; this doesn't remove the wounded condition. As with Treat Wounds, you can attempt checks against higher DCs if you have the minimum proficiency rank.\n\nIf you're an expert in Medicine, you can instead attempt a DC 20 check to increase the Hit Points regained by 10.\n\nThe target is then temporarily immune to your Battle Medicine until long rest.\n\nIt is assumed you have a healer's kit as long as you have a hand free otherwise you may not use Treat Wounds.",
                new[] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, DawnniExpanded.DETrait}
                )
            .WithActionCost(1)
            .WithCustomName("Battle Medicine")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Medicine) >= Proficiency.Trained, "You must be trained in Medicine.")
            .WithOnCreature((sheet, creature) =>
            {
                        
                    
                    creature.AddQEffect(new QEffect("Battle Medicine", "You can patch up wounds, even in combat")
                        {
                        
                       ProvideActionIntoPossibilitySection = (QEffect qfself,PossibilitySection possibilitySection1) =>
                        {
                        if(possibilitySection1.PossibilitySectionId != PossibilitySectionId.SkillActions)
                        {
                            return null;
                        }

                        CombatAction TrainedBattleMedAction = new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/BattleMedicine.png"), "Battle Medicine (Trained)", new Trait[] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, Trait.Basic, Trait.AttackDoesNotTargetAC,DawnniExpanded.DETrait },
                            "Attempt a {b}DC 15 Medicine check{/b} targeting an adjacent ally or yourself. The target is then temporarily immune to your Battle Medicine until long rest.\n\n{b}Critical Success{/b} The target regains 4d8 Hit Points.\n{b}Success{/b} The target regains 2d8 Hit Points.\n{b}Critical Failure{b}: The target takes 1d8 daamge.",
                             Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((a, d) => a.HasFreeHand ? Usability.Usable : Usability.CommonReasons.NoFreeHandForManeuver)
                            .WithAdditionalConditionOnTargetCreature((a, d) => !a.PersistentUsedUpResources.UsedUpActions.Contains("BattleMedicine:"+d.CreatureId) ? Usability.Usable : Usability.NotUsable("Target has Been already affected by your Battle Medicine today."))
                            )
                            .WithSoundEffect(SfxName.Healing)
                            .WithActionCost(1);
                        


                        CombatAction ExpertBattleMedAction = new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/BattleMedicine.png"), "Battle Medicine (Expert)", new Trait[] { Trait.General, Trait.Skill, Trait.Healing, Trait.Manipulate, Trait.Basic, Trait.AttackDoesNotTargetAC,DawnniExpanded.DETrait },
                             "Attempt a {b}DC 20 Medicine check{/b} targeting an adjacent ally or yourself. The target is then temporarily immune to your Battle Medicine until long rest.\n\n{b}Critical Success{/b} The target regains 4d8+10 Hit Points.\n{b}Success{/b} The target regains 2d8+10 Hit Points.\n{b}Critical Failure{/b} The target takes 1d8 daamge.",
                            Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((a, d) => a.HasFreeHand ? Usability.Usable : Usability.CommonReasons.NoFreeHandForManeuver)
                            .WithAdditionalConditionOnTargetCreature((a, d) => !a.PersistentUsedUpResources.UsedUpActions.Contains("BattleMedicine:"+d.CreatureId) ? Usability.Usable : Usability.NotUsable("Target has Been already affected by your Battle Medicine today.") )
                            )
                            .WithSoundEffect(SfxName.Healing)
                            .WithActionCost(1);

                        ActionPossibility TrainedBattleMed = TrainedBattleMedAction
                        .WithActiveRollSpecification(
                            new ActiveRollSpecification(Checks.SkillCheck(Skill.Medicine), Checks.FlatDC(15)))                     
                            .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                            {
                                

                                creature.PersistentUsedUpResources.UsedUpActions.Add("BattleMedicine:" + target.CreatureId);

                                if (result == CheckResult.CriticalSuccess)
                                {
                                    target.Heal("4d8", spell);
                                    
                                }
                                else if (result == CheckResult.Success)
                                {
                                    
                                    target.Heal("2d8", spell);
                                    
                                }
                                else if(result == CheckResult.Failure)
                                {
                            
                                }
                                if (result == CheckResult.CriticalFailure)
                                {

                                    DiceFormula damage = DiceFormula.FromText("1d8", spell.Name);
                                    await caster.DealDirectDamage(spell, damage, target, CheckResult.Success, DamageKind.Untyped);
                                }
                            });


                            ActionPossibility ExpertBattleMed = ExpertBattleMedAction
                        .WithActiveRollSpecification(
                            new ActiveRollSpecification(Checks.SkillCheck(Skill.Medicine), Checks.FlatDC(20)))                     
                            .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                            {

                               creature.PersistentUsedUpResources.UsedUpActions.Add("BattleMedicine:" + target.CreatureId);

                                if (result == CheckResult.CriticalSuccess)
                                {

                                    target.Heal("4d8+10", spell);
                                    
                                }
                                else if (result == CheckResult.Success)
                                {
                                
                                    target.Heal("2d8+10", spell);
                                    
                                }
                                else if(result == CheckResult.Failure)
                                {
;
                                }
                                if (result == CheckResult.CriticalFailure)
                                {
                                   
                                    DiceFormula damage = DiceFormula.FromText("1d8", spell.Name);
                                    await caster.DealDirectDamage(spell, damage, target, CheckResult.Success, DamageKind.Untyped);
                                }
                            });
                
                        SubmenuPossibility submenuPossibility = new SubmenuPossibility(IllustrationName.Heal,"Battle Medicine");
                            PossibilitySection possibilitySection = new PossibilitySection("Battle Medicine");
                            possibilitySection.AddPossibility(TrainedBattleMed);
                      
                        if (sheet.GetProficiency(Trait.Medicine) >= Proficiency.Expert){
                            possibilitySection.AddPossibility(ExpertBattleMed);
                        }

                        submenuPossibility.Subsections.Add(possibilitySection);

                        return submenuPossibility;
                        }
                        
                        })
                        ;

                
            }));

    }
}