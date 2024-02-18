using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.Roller;
using System.Linq;
using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dawnsbury.Mods.DawnniExpanded;



public class SpellRousingSplash{


    public static void LoadMod()
    {
        ModManager.RegisterNewSpell("Rousing Splash", 1, (spellId, spellcaster, spellLevel, inCombat) =>
        {
            return Spells.CreateModern(new ModdedIllustration("DawnniburyExpandedAssets/RousingSplash.png"), 
                "Rousing Splash",
            new[] { Trait.Primal, Trait.Divine, Trait.Water, Trait.Cantrip , DawnniExpanded.DETrait }, 
                    "You cause a splash of cold water to descend on an ally's head, granting some temporary vigor.",
                    "The target gains "+ S.HeightenedVariable(spellLevel * 1, 1) + "d4 temporary Hit Points. The target is then temporarily immune to the temporary Hit Points from rousing splash for rest of the encounter.\n\nThe target can also attempt an immediate flat check to recover from a single source of persistent acid or fire damage." + S.HeightenText(spellLevel > 1, inCombat, "\n\n{b}Heightened (+1){/b} The negative damage to living creatures increases by 1d4."),
    
                    Target.RangedFriend(12),
                        1, 
                        null
                        ).WithActionCost(2)
                        .WithSoundEffect(SfxName.OceansBalm)
                        .WithEffectOnChosenTargets(async (CombatAction spell, Creature caster, ChosenTargets chosenTargets) =>
                        
                        {
                            Creature target = chosenTargets.ChosenCreature;
                            

                            (int, string) SplashTHP = DiceFormula.FromText(spell.SpellLevel.ToString() + "d4").Roll();
                            int item = SplashTHP.Item1;
                            StringBuilder stringBuilder = new StringBuilder(SplashTHP.Item2);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine("{b}= " + item + " Temporary HP{/b}");
                            int num = item - target.TemporaryHP;
                            

                            if (num > 0)
                            {
                                target.TemporaryHP = item;
                                target.Occupies.Overhead("+" + num, Color.ForestGreen, target.Name + " {Green}gains{/} " + num + " temporary HP.", "Temporary HP", stringBuilder.ToString());
                            } else {
                                target.Occupies.Overhead("+" + num, Color.ForestGreen, target.Name +  " doesn't gain any temporary HP as they already had more.", "Rousing Splash", stringBuilder.ToString());
                            }


                            target.AddQEffect(QEffect.ImmunityToTargeting(spell.ActionId));
                            target.QEffects.FirstOrDefault<QEffect>(qf => qf.Id == QEffectId.PersistentDamage && ((qf.Key.Substring("PersistentDamage:".Length) == "Fire") || qf.Key.Substring("PersistentDamage:".Length) == "Acid"))?.RollPersistentDamageRecoveryCheck();
                            
                        }

                        );
        });
    }
}
                
                
