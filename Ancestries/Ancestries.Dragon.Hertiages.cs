using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.CharacterBuilding;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using static Dawnsbury.Delegates;


namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{

  public class DragonHertiageFeat : HeritageSelectionFeat
  {
    public Target BreathTargeting;
    public string ExtraUnarmed1;
    public string ExtraUnarmed2;
    public DamageKind AssociatedDamage;
    public Trait SpellList;

    public DragonHertiageFeat(string name, string flavorText, string rulesText, Target breathTargeting, string extraUnarmed1, string extraUnarmed2, DamageKind associatedDamage, Trait spellList)
        : base(FeatName.CustomFeat, flavorText, rulesText)
    {
      this.ExtraUnarmed1 = extraUnarmed2;
      this.ExtraUnarmed2 = extraUnarmed1;
      this.BreathTargeting = breathTargeting;
      this.AssociatedDamage = associatedDamage;
      this.SpellList = spellList;
      this
          .WithCustomName(name);
    }
  }

  public class DragonHertiages
  {
    public static IEnumerable<Feat> LoadFeats()

    {
      yield return new DragonHertiageFeat("Gold Dragon",
                 "You are a gold dragon, descended from a line of wise and powerful counselors and leaders.",
                 "You gain the Draconic Resistance feat as a bonus feat. Your associated damage type is fire, your breath shape is a cone, and your additional unarmed attacks are tail and horn. You can choose Wisdom instead of Strength for your first ancestry ability boost. Your spells are divine.",
                 Target.Cone(3),
                 "horn",
                 "tail",
                 DamageKind.Fire,
                 Trait.Divine)
             .WithOnSheet((sheet =>
  {
    sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
    {
            (AbilityBoost) new LimitedAbilityBoost(Ability.Strength, Ability.Wisdom),
            (AbilityBoost) new FreeAbilityBoost()
    };
    sheet.AddFeat(AncestryDragon.DragonResistance, null);
  }));

      yield return new DragonHertiageFeat("Brass Dragon",
                     "You are a brass dragon, descended from a line of independent conversationalists with insatiable curiosity and short attention spans.",
                     "You gain the Draconic Resistance feat as a bonus feat. Your associated damage type is fire, your breath shape is a line, and your additional unarmed attack is wing. You can choose Charisma instead of Strength for your first ancestry ability boost. Your spells are arcane.",
                     Target.Line(6),
                     "wing",
                     null,
                     DamageKind.Fire,
                     Trait.Arcana)
                 .WithOnSheet((sheet =>
      {
        sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
        {
            (AbilityBoost) new LimitedAbilityBoost(Ability.Strength, Ability.Charisma),
            (AbilityBoost) new FreeAbilityBoost()
        };
        sheet.AddFeat(AncestryDragon.DragonResistance, null);
      }));

      yield return new DragonHertiageFeat("Silver Dragon",
                     "You are a brass dragon, descended from a line of chivalrous champions of justice, guardians and guides of goodly societies.",
                     "You gain the Draconic Resistance feat as a bonus feat. Your associated damage type is cold, your breath shape is a cone, and your additional unarmed attack is wing. You can choose Charisma instead of Strength for your first ancestry ability boost. Your spells are divine.",
                     Target.Line(6),
                     "wing",
                     null,
                     DamageKind.Cold,
                     Trait.Divine)
                 .WithOnSheet((sheet =>
      {
        sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
        {
            (AbilityBoost) new LimitedAbilityBoost(Ability.Strength, Ability.Charisma),
            (AbilityBoost) new FreeAbilityBoost()
        };
        sheet.AddFeat(AncestryDragon.DragonResistance, null);
      }));

      yield return new DragonHertiageFeat("Astral Dragon",
                       "You are an astral dragon, descended from a line of proud psychic dragons from the Astral Plane.",
                       "You gain the Draconic Resistance feat as a bonus feat. Your associated damage type is mental, your breath shape is a line, and your additional unarmed attack is tail and horn. You can choose Intelligence instead of Strength for your first ancestry ability boost. Your spells are occult.",
                       Target.Line(6),
                       "horn",
                       "tail",
                       DamageKind.Mental,
                       Trait.Occult)
                   .WithOnSheet((sheet =>
        {
          sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
          {
            (AbilityBoost) new LimitedAbilityBoost(Ability.Strength, Ability.Intelligence),
            (AbilityBoost) new FreeAbilityBoost()
          };
          sheet.AddFeat(AncestryDragon.DragonResistance, null);
        }));
      /*
            yield return new DragonHertiageFeat("Cloud Dragon",
                               "You are a cloud dragon, descended from a line of inquisitive air elemental dragons who love to wander and explore.",
                               "You gain the Draconic Resistance feat as a bonus feat. You gain the air and elemental traits, though you still need to breathe. Fog and mist don't impair your vision; you ignore the concealed condition from fog and mist (FOGSIGHT IS NOT IMPLEMENTED). Your associated damage type is mental, your breath shape is a line, and your additional unarmed attack is tail and horn. You can choose Wisdom instead of Strength for your first ancestry ability boost. Your spells are primal.",
                               Target.Cone(3),
                               "horn",
                               "tail",
                               DamageKind.Electricity,
                               Trait.Primal)
                           .WithOnSheet((sheet =>
                {
                  sheet.AbilityBoostsFabric.AncestryBoosts = new List<AbilityBoost>()
                  {
                  (AbilityBoost) new LimitedAbilityBoost(Ability.Strength, Ability.Wisdom),
                  (AbilityBoost) new FreeAbilityBoost()
                  };
                  sheet.Sheet.Calculated.AddFeat(AncestryDragon.DragonResistance, null);
                }));
      */


    }
  }


}

