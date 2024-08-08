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


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemThundering
{
  public static void LoadMod()
  {

    ItemName ThunderingRunestone = ModManager.RegisterNewItemIntoTheShop("thundering runestone", itemName =>
   new Item(itemName, IllustrationName.Rock, "thundering runestone", 8, 500, new Trait[]
  {
            Trait.Invested,
            Trait.Magical,
            Trait.Sonic,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
  }).WithDescription("{i}You hear an echoing thunder if you put this runestone next to your ear.{/i}\n\nYour Strikes deal an additional 1d6 sonic damage. When you critically hit with a Strike, your target must succeed at a DC 24 Fortitude save or be deafened for the rest of the encounter.\n\n")
  .WithWornAt(ItemRunestone.WeaponRunestone)
  .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
  {
    qfrune.AddExtraStrikeDamage = (action, target) =>
                  {
                    return (DiceFormula.FromText("1d6", "Thundering Runestone"), DamageKind.Sonic);
                  };

    qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
    {

      if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
        return;

      var Caster = QEffect.Owner;

      if (Target == null)
      {
        return;
      }

      if (Target.Alive == false)
      {
        return;
      }

      Target.Occupies.Overhead("Thundering Runestone", Color.Yellow, Caster.Name + "'s thundering runestone's critical effect activated against " + Target.Name + ".");

      CombatAction simple = CombatAction.CreateSimple(Caster, "Thundering Runestone Critical Effect");
      simple.SpellLevel = Caster.MaximumSpellRank;
      simple.Traits.Add(Trait.Magical);
      simple.Traits.Add(Trait.Sonic);
      simple.WithActionCost(0);
      CheckResult checkResult = CommonSpellEffects.RollSavingThrow(Target, simple, Defense.Fortitude, (Func<Creature, int>)(caster => 24));
      if (checkResult <= CheckResult.Failure)
        Target.AddQEffect(QEffect.Deafened().WithExpirationNever());

      return;

    };
    qfrune.Name = "Thundering Runestone";
    qfrune.Description = "Your strikes deal extra sonic damage.";
    qfrune.Innate = true;

  }

        )
        );

    ItemName ThunderingLesserRunestone = ModManager.RegisterNewItemIntoTheShop("thundering runestone (lesser)", itemName =>
    new Item(itemName, IllustrationName.Rock, "thundering runestone (lesser)", 3, 50, new Trait[]
   {
            Trait.Invested,
            Trait.Magical,
            Trait.Sonic,
            ItemRunestone.WeaponRunestone,
            DawnniExpanded.DETrait,
            DawnniExpanded.HomebrewTrait
   }).WithDescription("{i}You hear an echoing thunder if you put this runestone next to your ear.{/i}\n\nYour Strikes deal an additional 1 sonic damage. When you critically hit with a Strike, your target must succeed at a DC 18 Fortitude save or be deafened for the rest of the encounter.\n\n")
   .WithWornAt(ItemRunestone.WeaponRunestone)
   .WithPermanentQEffectWhenWorn((QEffect qfrune, Item item) =>
   {
     qfrune.AddExtraStrikeDamage = (action, target) =>
                   {
                     return (DiceFormula.FromText("1", "Thundering Runestone"), DamageKind.Sonic);
                   };

     qfrune.AfterYouTakeActionAgainstTarget = async (QEffect, hostileAction, Target, CheckResult) =>
     {

       if (!hostileAction.HasTrait(Trait.Strike) || CheckResult != CheckResult.CriticalSuccess)
         return;

       var Caster = QEffect.Owner;

       if (Target == null)
       {
         return;
       }

       if (Target.Alive == false)
       {
         return;
       }

       Target.Occupies.Overhead("Thundering Runestone", Color.Yellow, Caster.Name + "'s thundering runestone's critical effect activated against " + Target.Name + ".");

       CombatAction simple = CombatAction.CreateSimple(Caster, "Thundering Runestone Critical Effect");
       simple.SpellLevel = Caster.MaximumSpellRank;
       simple.Traits.Add(Trait.Magical);
       simple.Traits.Add(Trait.Sonic);
       simple.WithActionCost(0);
       CheckResult checkResult = CommonSpellEffects.RollSavingThrow(Target, simple, Defense.Fortitude, (Func<Creature, int>)(caster => 18));
       if (checkResult <= CheckResult.Failure)
         Target.AddQEffect(QEffect.Deafened().WithExpirationNever());

       return;

     };
     qfrune.Name = "Thundering Runestone";
     qfrune.Description = "Your strikes deal extra sonic damage.";
     qfrune.Innate = true;

   }

         )
         );

  }
}