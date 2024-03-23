using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Intelligence;




namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellHymnOfHealing
{


    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/HymnOfHealing.png");

    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
    {

        return Spells.CreateModern(SpellIllustration,
                    "Hymn of Healing",
                new[] {
            Trait.Uncommon,
            Trait.Healing,
            Trait.Necromancy,
            Trait.Positive,
            Trait.Focus,
            Trait.Composition,
            Trait.DoesNotProvoke,
            DawnniExpanded.DETrait },
                        "Your divine singing mends wounds and provides a temporary respite from harm.",
                        "The target heals " + S.HeightenedVariable(spellLevel * 2, 2) + " hit points."
                        + "\n\nWhen you Cast the Spell and the first time each round you Sustain the Spell, the target gains " + S.HeightenedVariable(spellLevel * 2, 2) + " temporary Hit Points and heals the same amount."
                        + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "{b}Heightened (+1){/b} The healed hit points and temporary Hit Points each increase by 2."),
                        Target.RangedFriend(6),
                            spellLevel,
                            null
                            ).WithActionCost(2)
                            .WithSoundEffect(SfxName.Healing)
                            .WithEffectOnChosenTargets(async (CombatAction spell, Creature caster, ChosenTargets chosenTargets) =>

                            {
                                Creature target = chosenTargets.ChosenCreature;
                                int EndureTHP = spellLevel * 2;
                                target.GainTemporaryHP(EndureTHP);
                                target.Heal(EndureTHP.ToString(), spell);
                                int RoundsLeft = 3;

                                QEffect EffectOnTarget = new QEffect("Hymn of Healing", "When Hymn of Healing is sustained, " + target.Name + " will gain " + spellLevel * 2 + " temporary Hit Points and heal " + spellLevel * 2 + " Hit Points.", ExpirationCondition.ExpiresAtEndOfSourcesTurn, caster, SpellIllustration)
                                {
                                    CannotExpireThisTurn = true,
                                };


                                QEffect qEffectSustain = QEffect.Sustaining(spell, EffectOnTarget);
                                qEffectSustain.ProvideContextualAction = (QEffect qf) => (!EffectOnTarget.CannotExpireThisTurn) ? new ActionPossibility(new CombatAction(qf.Owner, spell.Illustration, "Sustain " + spell.Name, new Trait[3]
                                {
                                Trait.Concentrate,
                                Trait.SustainASpell,
                                Trait.Basic
                                }, "The duration of " + spell.Name + " continues until the end of your next turn.", Target.Self((Creature self, AI ai) => 1.0737418E+09f)).WithEffectOnSelf(delegate
                                {
                                    EffectOnTarget.CannotExpireThisTurn = true;
                                    int EndureTHP = spell.SpellLevel * 2;
                                    target.GainTemporaryHP(EndureTHP);
                                    target.Heal(EndureTHP.ToString(), spell);
                                    --RoundsLeft;

                                })) : null;
                                qEffectSustain.StateCheck = delegate (QEffect qf)
                                {
                                    if (EffectOnTarget.Owner.Destroyed || !EffectOnTarget.Owner.HasEffect(EffectOnTarget) || RoundsLeft == 0)
                                    {
                                        qf.ExpiresAt = ExpirationCondition.Immediately;
                                    }


                                };

                                caster.AddQEffect(qEffectSustain);
                                target.AddQEffect(EffectOnTarget);

                            }

                            );
    }




    public static void LoadMod()
    {

        Id = ModManager.RegisterNewSpell("Hymn of Healing", 1, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>
        CombatAction(spellcaster, spellLevel, inCombat)
    );
    }

    public static string Link()
    {
        CombatAction _spell = CombatAction(null, 1, false);
        _spell.SpellId = Id;
        string _spellstring = new Spell(_spell).ToSpellLink();
        return _spellstring;
    }

}


