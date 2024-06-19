using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using System;
using Dawnsbury.Audio;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.StatBlocks.Description;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemShock
{
  public static void LoadMod()
  {

    ItemName ShockRunestone = ModManager.RegisterNewItemIntoTheShop("shock runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "shock runestone", 8, 500, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Electricity,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}Electric arcs crisscross shock runestones.{/i}\n\nYour Strikes deal an additional 1d6 electricity damage. When you critically hit with a Strike, electricity arcs out to deal 1d6 electricity damage to up to two other creatures of your choice within 10 feet of the target.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                    {
                      return null;
                    }
                    else return (DiceFormula.FromText("1d6", "Shock Runestone"), DamageKind.Electricity);
                  };

    qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, CreatureTarget, CheckResult) =>
    {

      if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
        return;

      var Caster = QEffect.Owner;

      if (CreatureTarget == null)
      {
        return;
      }


      CreatureTarget.Occupies.Overhead("Shock Runestone", Color.Blue, Caster.Name + "'s shock runestone's critical effect activated.");


      CreatureTarget ValidShockTargets = Target.Ranged(9999).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>)((Creature caster, Creature target) => CreatureTarget == target ? Usability.NotUsableOnThisCreature("You must select enemies other than the main target") : target.DistanceTo(CreatureTarget) <= 2 ? Usability.Usable : Usability.NotUsableOnThisCreature("Not within 10ft of main target")));

      CombatAction simple = CombatAction.CreateSimple(Caster, "Shock Runestone Critical Effect");
      simple.SpellLevel = Caster.MaximumSpellRank;
      simple.Traits.Add(Trait.Magical);
      simple.Traits.Add(Trait.Electricity);
      simple.WithActionCost(0);
      simple.Description = "When you critically hit with a Strike, electricity arcs out to deal 1d6 electricity damage to up to two other creatures of your choice within 10 feet of the target";
      simple.Illustration = IllustrationName.ElectricArc;
      simple.WithSoundEffect(SfxName.ElectricArc);
      simple.Target = Target.MultipleCreatureTargets(ValidShockTargets, ValidShockTargets).WithMustBeDistinct().WithMinimumTargets(0).WithSimultaneousAnimation().WithOverriddenTargetLine("1 or 2 enemies", true);
      simple.WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, checkResult) =>
          {
            await CommonSpellEffects.DealBasicDamage(spell, caster, target, CheckResult.Failure, DiceFormula.FromText("1d6", "Shock Runestone"), DamageKind.Electricity);
          }));


      TBattle battle = Caster.Battle;
      await battle.GameLoop.FullCast(simple);
      return;

    };
    qfrune.Name = "Shock Runestone";
    qfrune.Description = "Your strikes deal extra electricity damage.";
    qfrune.Innate = true;
  }

        )
        );

    ItemName ShockLesserRunestone = ModManager.RegisterNewItemIntoTheShop("shock runestone (lesser)", itemName =>
    new Item(itemName, IllustrationName.Rock, "shock runestone (lesser)", 3, 50, new Trait[]
   {
            Trait.Invested,
            Trait.Magical,
            Trait.Electricity,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
   }).WithDescription("{i}Electric arcs crisscross shock runestones.{/i}\n\nYour Strikes deal an additional 1 electricity damage. When you critically hit with a Strike, electricity arcs out to deal 1 electricity damage to up to two other creatures of your choice within 10 feet of the target.\n\n")
   .WithWornAt(ItemRunestone.WeaponRunestone)
   .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
   {
     qfrune.AddExtraStrikeDamage = (action, target) =>
                   {
                     if (action.Owner.HasEffect(QEffectId.AquaticCombat))
                     {
                       return null;
                     }
                     else return (DiceFormula.FromText("1", "Shock Runestone"), DamageKind.Electricity);
                   };

     qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, CreatureTarget, CheckResult) =>
     {

       if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
         return;

       var Caster = QEffect.Owner;

       if (CreatureTarget == null)
       {
         return;
       }


       CreatureTarget.Occupies.Overhead("Shock Runestone", Color.Blue, Caster.Name + "'s shock runestone's critical effect activated.");


       CreatureTarget ValidShockTargets = Target.Ranged(9999).WithAdditionalConditionOnTargetCreature((Func<Creature, Creature, Usability>)((Creature caster, Creature target) => CreatureTarget == target ? Usability.NotUsableOnThisCreature("You must select enemies other than the main target") : target.DistanceTo(CreatureTarget) <= 2 ? Usability.Usable : Usability.NotUsableOnThisCreature("Not within 10ft of main target")));

       CombatAction simple = CombatAction.CreateSimple(Caster, "Shock Runestone Critical Effect");
       simple.SpellLevel = Caster.MaximumSpellRank;
       simple.Traits.Add(Trait.Magical);
       simple.Traits.Add(Trait.Electricity);
       simple.WithActionCost(0);
       simple.Description = "When you critically hit with a Strike, electricity arcs out to deal 1d6 electricity damage to up to two other creatures of your choice within 10 feet of the target";
       simple.Illustration = IllustrationName.ElectricArc;
       simple.WithSoundEffect(SfxName.ElectricArc);
       simple.Target = Target.MultipleCreatureTargets(ValidShockTargets, ValidShockTargets).WithMustBeDistinct().WithMinimumTargets(0).WithSimultaneousAnimation().WithOverriddenTargetLine("1 or 2 enemies", true);
       simple.WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, checkResult) =>
           {
             await CommonSpellEffects.DealBasicDamage(spell, caster, target, CheckResult.Failure, DiceFormula.FromText("1", "Shock Runestone"), DamageKind.Electricity);
           }));


       TBattle battle = Caster.Battle;
       await battle.GameLoop.FullCast(simple);
       return;

     };
     qfrune.Name = "Shock Runestone";
     qfrune.Description = "Your strikes deal extra electricity damage.";
     qfrune.Innate = true;
   }

         )
         );

  }
}