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

public static class ArchetypeDuelist
{

    public static Feat DuelistDedicationFeat;

    public static Feat DuelingParryFeat;
    public static void LoadMod()
    
    {
        
        DuelistDedicationFeat = new TrueFeat(FeatName.CustomFeat, 
                2, 
                "Across the world, students in martial academies practice with their blades to master one-on-one combat. The libraries of such schools hold deep troves of information detailing hundreds of combat techniques, battle stances, and honorable rules of engagement. Those who gain admission to such schools might train in formalized duels—and that's certainly the more genteel route to take. However, others assert that there's no better place to try out dueling techniques than in the life-and-death struggles common to an adventurer's life.", 
                "You are always ready to draw your weapon and begin a duel, no matter the circumstances.\n\nYou gain the Quick Draw feat.", 
                new Trait[] {FeatArchetype.DedicationTrait,FeatArchetype.ArchetypeTrait,DawnniExpanded.DETrait})
                .WithCustomName("Duelist Dedication")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.LightArmor) >= Proficiency.Trained && values.GetProficiency(Trait.Simple) >= Proficiency.Trained,"You must be rained in light armor and simple weapons.")
                .WithOnSheet(sheet => sheet.GrantFeat(FeatName.QuickDraw));

        Feat DuelingParryFeat = new TrueFeat(FeatName.CustomFeat, 
                4, 
                "You can parry attacks against you with your one-handed weapon", 
                "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.", 
                new Trait[] {FeatArchetype.ArchetypeTrait,DawnniExpanded.DETrait})
                .WithOnCreature((sheet, creature) =>
      {

                        QEffect DuellingParryHolder = new QEffect(){
                        Name = "Dueling Parry Granter",
                        ProvideMainAction =(qfTechnical =>
                        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                            return new ActionPossibility
                      (new CombatAction(creature, new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"), "Dueling Parry", new Trait[] { Trait.Basic, DawnniExpanded.DETrait },
                                "{b}Requirements{/b}You are wielding only a single one-handed melee weapon and have your other hand or hands free.\n\nYou gain a +2 circumstance bonus to AC until the start of your next turn as long as you continue to meet the requirements.",
                                    Target.Self()
                                .WithAdditionalRestriction((a) =>
                                { 
                                  if(a.QEffects.Any((QEffect x) => x.Name == "Dueling Parry")){
                                    return "Already parrying.";
                                  };
                                    
                                    if (a.HasFreeHand)
                                    {
                                      
                                      if(a.HeldItems.FirstOrDefault() == null){
                                        return "Not holding a Melee Weapon";
                                      }
                                        if (!a.HeldItems.First().HasTrait(Trait.Melee))
                                        {
                                          return "Not holding a Melee Weapon";
                                        } else return null;
                                    
                                    } else return "No Free Hand.";

                                
                                })
                                )
                                .WithSoundEffect(SfxName.RaiseShield)
                                .WithActionCost(1)                 
                                .WithEffectOnSelf(async (spell, caster) =>
                            {
                              QEffect DuellingParryEffect = new QEffect()
                              {
                                Name = "Dueling Parry",
                                Owner = caster,
                                DoNotShowUpOverhead = true,
                                Description = "+2 circumstance bonus to AC until the start of your next turn.",
                                Illustration = new ModdedIllustration("DawnniburyExpandedAssets/DuelingParry.png"),
                                BonusToDefenses = (QEffect effect, CombatAction attack, Defense defense) => 
                                  {
                                      if (defense == Defense.AC)
                                      {
                                          return new Bonus(2, BonusType.Circumstance, "Dueling Parry");
                                      } else return null;
                                  },
                                ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn,
                                StateCheck = Qfduel => {

                                  if(Qfduel.Owner.HeldItems.FirstOrDefault() == null){
                                  Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                  } else
                                  if(!Qfduel.Owner.HasFreeHand || !Qfduel.Owner.HeldItems.First().HasTrait(Trait.Melee) || Qfduel.Owner.HasEffect(QEffectId.Unconscious) || Qfduel.Owner.HasEffect(QEffectId.Dying)  ){
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
                .WithCustomName("Dueling Parry (Duelist)")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(DuelistDedicationFeat) || values.Sheet.Class.ClassTrait == Trait.Fighter,"You must have the Duelist Dedidcation feat.")
                .WithEquivalent(values => values.AllFeats.Contains(FeatDuelingParry.FeatDuelingParryFighter));
                
               
        ModManager.AddFeat(DuelingParryFeat);
        ModManager.AddFeat(DuelistDedicationFeat);
    }
}