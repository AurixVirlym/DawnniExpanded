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
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Mechanics.Core;



//FULL CREDIT TO Jacob Dehkordi (beets) for art and code.

namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellSuddenBolt
{


  public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/SuddenBolt.png");

  public static SpellId Id;
  public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
  {



    return Spells.CreateModern((Illustration)SpellIllustration, "Sudden Bolt", new Trait[5]
    {
        Trait.Electricity,
        Trait.Evocation,
        Trait.Arcane,
        Trait.Primal,

        DawnniExpanded.DETrait
    }, "You summon a small bolt of lighting.",
     "Deal " + S.HeightenedVariable(4 + (spellLevel - 2), 4) + "d12 electricity damage." + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "{b}Heightened (+1){/b} The damage increases by 1d12."),
    Target.Ranged(12), spellLevel, SpellSavingThrow.Basic(Defense.Reflex)).
    WithSoundEffect(SfxName.ElectricBlast)
    .WithGoodnessAgainstEnemy(((Target t, Creature a, Creature d) => (float)(2 + t.OwnerAction.SpellLevel) * 6.5f))
    .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>


    await CommonSpellEffects.DealBasicDamage(spell, caster, target, result, (4 + spellLevel - 2) + "d12", DamageKind.Electricity)));
  }



  public static void LoadMod()
  {

    Id = ModManager.RegisterNewSpell("Sudden Bolt", 2, ((SpellId, spellcaster, spellLevel, inCombat, SpellInformation) =>
     CombatAction(spellcaster, spellLevel, inCombat)
 ));
  }

}



