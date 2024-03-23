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




namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellEndure
{


    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/Endure.png");

    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
    {

        return Spells.CreateModern(SpellIllustration,
                    "Endure",
                new[] { Trait.Arcane, Trait.Occult, Trait.Enchantment, Trait.Mental, Trait.DoesNotProvoke, DawnniExpanded.DETrait },
                        "You invigorate the touched creature's mind and urge it to press on.",
                        "You grant the touched creature " + S.HeightenedVariable(spellLevel * 4, 4) + " temporary Hit Points.\n"
                        + HS.HeightenTextLevels(spellLevel > 1, spellLevel, inCombat, "{b}Heightened (+1){/b} Increase the temporary Hit Points by 4."),
                        Target.AdjacentFriendOrSelf(),
                            spellLevel,
                            null
                            ).WithActionCost(1)
                            .WithSoundEffect(SfxName.Mental)
                            .WithEffectOnChosenTargets(async (CombatAction spell, Creature caster, ChosenTargets chosenTargets) =>

                            {
                                Creature target = chosenTargets.ChosenCreature;
                                int EndureTHP = spellLevel * 4;
                                target.GainTemporaryHP(EndureTHP);

                            }

                            );
    }




    public static void LoadMod()
    {

        Id = ModManager.RegisterNewSpell("Endure", 1, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) =>
        CombatAction(spellcaster, spellLevel, inCombat)
    );
    }

}


