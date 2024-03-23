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
using Dawnsbury.Core.Mechanics.Core;


namespace Dawnsbury.Mods.DawnniExpanded;



public class SpellRousingSplash
{

    public static Illustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/RousingSplash.png");

    public static void RollPersistentDamageRecoveryCheckDawnnni(QEffect qf, int DC = 15)
    {
        (CheckResult, string) tuple = Checks.RollFlatCheck(DC);
        CheckResult item = tuple.Item1;
        string item2 = tuple.Item2;
        string text = qf.Key.Substring("PersistentDamage:".Length).ToLower();
        string log = qf.Owner?.ToString() + " makes a recovery check against persistent " + text + " damage vs. DC" + DC + " (" + item2 + ")";
        if (item >= CheckResult.Success)
        {
            qf.ExpiresAt = ExpirationCondition.Immediately;
            qf.Owner.Occupies.Overhead("recovered", Color.Lime, log);
        }
        else
        {
            qf.Owner.Occupies.Overhead("not recovered", Color.Black, log);
        }
    }

    public static void LoadMod()

    {
        ActionId RousingSplashActionID = ModManager.RegisterEnumMember<ActionId>("RousingSplashActionID");
        QEffectId RousingSplashEffectID = ModManager.RegisterEnumMember<QEffectId>("RousingSplashQEffectID");

        ModManager.RegisterNewSpell("Rousing Splash", 0, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>
        {
            return Spells.CreateModern(SpellIllustration,
                "Rousing Splash",
            new[] { Trait.Primal, Trait.Divine, Trait.Water, Trait.Cantrip, DawnniExpanded.DETrait },
                    "You cause a splash of cold water to descend on an ally's head, granting some temporary vigor.",
                    "The target gains " + S.HeightenedVariable(spellLevel * 1, 1) + "d4 temporary Hit Points. The target is then temporarily immune to the temporary Hit Points from rousing splash for rest of the encounter.\n\nThe target can also attempt an immediate flat check to recover from a single source of persistent acid or fire damage, with the DC reduction to DC10 from appropriate assistance." + S.HeightenText(spellLevel, 1, inCombat, "\n\n{b}Heightened (+1){/b} The negative damage to living creatures increases by 1d4."),

                    Target.RangedFriend(12),
                        spellLevel,
                        null
                        ).WithActionCost(2)
                        .WithActionId(RousingSplashActionID)
                        .WithSoundEffect(SfxName.OceansBalm)
                        .WithEffectOnChosenTargets(async (CombatAction spell, Creature caster, ChosenTargets chosenTargets) =>

                        {
                            Creature target = chosenTargets.ChosenCreature;

                            if (!target.HasEffect(RousingSplashEffectID))
                            {

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
                                }
                                else
                                {
                                    target.Occupies.Overhead("+" + num, Color.ForestGreen, target.Name + " doesn't gain any temporary HP as they already had more.", "Rousing Splash", stringBuilder.ToString());
                                }


                                target.AddQEffect(new QEffect("Rousing Splash", "This creature may not gain more temporary HP from Rousing Splash", ExpirationCondition.Never, caster, SpellIllustration) { Id = RousingSplashEffectID });
                            }

                            foreach (QEffect PresistentFireAcid in target.QEffects.Where<QEffect>(qf => qf.Id == QEffectId.PersistentDamage && ((qf.Key.Substring("PersistentDamage:".Length) == "Fire") || qf.Key.Substring("PersistentDamage:".Length) == "Acid")))
                            {
                                RollPersistentDamageRecoveryCheckDawnnni(PresistentFireAcid, 10);
                            }

                        }

                        );
        });
    }
}


