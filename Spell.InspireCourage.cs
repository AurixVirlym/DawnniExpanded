using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Creatures;
using System;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using System.Linq;




namespace Dawnsbury.Mods.DawnniExpanded;



public class SpellInspireCourage{
    
    public static SpellId spellId;

    public static void LoadMod()
    {
        ModdedIllustration illustrationInspireCourage= new ModdedIllustration("DawnniburyExpandedAssets/InspireCourage.png");
        
       
        

        spellId = ModManager.RegisterNewSpell("Inspire Courage", 1, (spellId, spellcaster, spellLevel, inCombat) =>
        {
    
        return Spells.CreateModern((Illustration) illustrationInspireCourage, "Inspire Courage", new Trait[]
          {
            Trait.Cantrip,
            Trait.Uncommon,
            Trait.Emotion,
            Trait.Enchantment,
            Trait.Mental,
            Trait.Composition
          }, "You inspire your allies with words or tunes of encouragement.","You and all allies in the area gain a +1 status bonus to attack rolls, damage rolls, and saves against fear effects.", (Target) Target.Emanation(12), 1, null)
          .WithSoundEffect(SfxName.PositiveMelody)
          .WithActionCost(1)
          .WithGoodness((Func<Target, Creature, Creature, float>) ((t, a, d) => !d.EnemyOf(a) && !a.QEffects.Any(qf => qf.Name == "Inspire Courage") ? (float) AICalcs.AttackBonusGoodnessForNPCs(a.Level,1,1)*2: 0.0f))
          .WithEffectOnEachTarget((Delegates.EffectOnEachTarget) (async (spell, caster, target, result) =>
          {
            if (target.EnemyOf(caster)){
              return;
            }
            target.AddQEffect(new QEffect("Inspire Courage", "You have +1 status bonus to attack rolls, damage rolls and saves against fear effects.", ExpirationCondition.ExpiresAtStartOfSourcesTurn, caster, illustrationInspireCourage)
            {
              BonusToAttackRolls = (((effect, action, arg3) => !action.HasTrait(Trait.Attack) ? (Bonus) null : new Bonus(1, BonusType.Status, "Inspire Courage"))),
              BonusToDamage = (((effect, action, arg3) => new Bonus(1, BonusType.Status, "Inspire Courage"))),
              BonusToDefenses = ( ((effect, action, defense) => !defense.IsSavingThrow() || action == null || !action.HasTrait(Trait.Fear) ? (Bonus) null : new Bonus(1, BonusType.Status, "Inspire Courage")))
            }.WithExpirationAtStartOfSourcesTurn(caster, 1));
          }));
                    
    });
        
    }
}
                
                
