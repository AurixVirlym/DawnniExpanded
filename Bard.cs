using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;


namespace Dawnsbury.Mods.DawnniExpanded
{


  public class Bard
  {


    public static int LevelBasedDC(int level)
    {
      switch (level)
      {
        case 0:
          return 14;
        case 1:
          return 15;
        case 2:
          return 16;
        case 3:
          return 18;
        case 4:
          return 19;
        case 5:
          return 20;
        case 6:
          return 22;
        case 7:
          return 23;
        case 8:
          return 24;
        case 9:
          return 26;
        case 10:
          return 27;
        default:
          return 10;


      }
    }


    public static string MakeString()
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(397, 4);
      interpolatedStringHandler.AppendLiteral("{b}1. Muses.{/b}As a bard, you select one muse at 1st level. This muse leads you to great things, and might be a physical creature, a deity, a philosophy, or a captivating mystery. A muse grants you one extra spell known and an extra feat.\r\n\r\n{b}2. Spontaneous occult spellcasting.{/b} ");
      interpolatedStringHandler.AppendLiteral("You can cast spells. You can cast 2 spells per day and you can choose the spells from among the spells you know. You learn 2 spells of your choice from the Occult tradition and 1 additional spell from your Muse. You also learn 5 cantrips — weak spells — that automatically heighten as you level up. You can cast any number of cantrips per day. Your spellcasting ability is Charisma.");
      CombatAction CounterPerformanceAction = SpellCounterPerformance.CombatAction(null, 1, false);
      CounterPerformanceAction.SpellId = SpellCounterPerformance.Id;
      string CounterPerformanceSpell = new Spell(CounterPerformanceAction).ToSpellLink();

      CombatAction InspireAction = SpellInspireCourage.CombatAction(null, 1, false);
      InspireAction.SpellId = SpellInspireCourage.Id;
      string InspireSpell = new Spell(InspireAction).ToSpellLink();

      interpolatedStringHandler.AppendLiteral("\r\n\r\n{b}3. Composition Spells{/b} You can infuse your performances with magic to create unique effects called compositions. Compositions are a special type of spell that often require you to use the Performance skill when casting them. Composition spells are a type of focus spell. It costs 1 Focus Point to cast a focus spell, and you start with a focus pool of 1 Focus Point." + " You learn the " + CounterPerformanceSpell + " composition spell and " + InspireSpell + " composition spell cantrip which does not require focus points to be used.");

      interpolatedStringHandler.AppendLiteral("\r\n\r\n{b}At higher levels:{/b}\r\n{b}Level 2:{/b} Bard feat, ");
      interpolatedStringHandler.AppendFormatted(S.ExtraSpontaneousSpellSlot(1));
      interpolatedStringHandler.AppendLiteral("\r\n{b}Level 3:{/b} General feat, Lightning Reflexes, ");
      interpolatedStringHandler.AppendFormatted(S.InitialLevel2SpontaneousSpellSlots("Two slots, two spells known."));
      interpolatedStringHandler.AppendLiteral(", skill increase\r\n{b}Level 4:{/b} Bard feat.");
      interpolatedStringHandler.AppendFormatted(S.ExtraSpontaneousSpellSlot(2));
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      return stringAndClear;

    }


    public static Feat BardClass = new ClassSelectionFeat(FeatName.CustomFeat
    , "You are a master of artistry, a scholar of hidden secrets, and a captivating persuader. Using powerful performances, you influence minds and elevate souls to new levels of heroics. You might use your powers to become a charismatic leader, or perhaps you might instead be a counselor, manipulator, scholar, scoundrel, or virtuoso. While your versatility leads some to consider you a beguiling ne'er=do- well and a jack-of-all-trades, it's dangerous to dismiss you as a master of none."
    , Trait.Bard
    , new EnforcedAbilityBoost(Ability.Charisma)
    , 8
    , new Trait[]
  {
        Trait.Fortitude,
        Trait.Reflex,
        Trait.Simple,
        Trait.Longsword,
        Trait.RogueWeapon,
        Trait.Performance,
        Trait.LightArmor,
        Trait.Unarmed,
        Trait.UnarmoredDefense,
        Trait.Occultism
  }, new Trait[]
  {
        Trait.Will,
        Trait.Perception
  }
  , 4
  , MakeString()
  , new List<Feat>()
  {
        (Feat) new Feat(FeatName.CustomFeat, "Your muse is a virtuoso, inspiring you to greater heights. If it's a creature, it might be a performance-loving creature such as a choral angel or lillend azata. As a bard with a maestro muse, you are an inspiration to your allies and confident of your musical and oratorical abilities.", "You gain the Lingering Composition feat and add "+AllSpells.CreateModernSpellTemplate(SpellId.Soothe,Trait.Bard).ToSpellLink()+" to your spell repertoire. ", new List<Trait>(), null).WithCustomName("Maestro").WithOnSheet( sheet =>
        {
        sheet.AddFeat(LingeringComposition,null);
        sheet.FocusPointCount += 1;
        Spell Spelltoadd = AllSpells.All.FirstOrDefault(spell => spell.SpellId == SpellId.Soothe);
        sheet.SpellRepertoires[Trait.Bard].SpellsKnown.Add(Spelltoadd);
        }),
        (Feat) new Feat(FeatName.CustomFeat, "The battlefield is your stage, the clang of steel, your song. Your muse engages in countless battles, whether reveling in combat or resigned to its necessity. If your muse is a creature, it might be an otherworldly soldier, such as a planetar, archon, cornugon, or purrodaemon. As a bard with a warrior muse, you train for battle in addition to performance, and you prepare your allies for the dangers of battle. You might even wade into the thick of things with them.", "You gain Martial Weapon Proficiency and add "+AllSpells.CreateModernSpellTemplate(SpellId.Fear,Trait.Bard).ToSpellLink()+" to your spell repertoire.", new List<Trait>(), null).WithCustomName("Warrior").WithOnSheet( sheet =>
        {
        sheet.GrantFeat(FeatName.WeaponProficiency,FeatName.WeaponProficiencyMartial);
        Spell Spelltoadd = AllSpells.All.FirstOrDefault(spell => spell.SpellId == SpellId.Fear);
        sheet.SpellRepertoires[Trait.Bard].SpellsKnown.Add(Spelltoadd);
        }),
  }).WithOnSheet(sheet =>
  {
    sheet.GrantFeat(FeatName.Occultism);
    sheet.AddFeat(NewSkills.Performance, null);

    Trait spellList = Trait.Occult;
    sheet.SpellTraditionsKnown.Add(spellList);
    sheet.SpellRepertoires.Add(Trait.Bard, new SpellRepertoire(Ability.Charisma, spellList));
    sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
    SpellRepertoire repertoire = sheet.SpellRepertoires[Trait.Bard];
    CombatAction InspireAction = SpellInspireCourage.CombatAction(null, sheet.MaximumSpellLevel, false);
    InspireAction.SpellId = SpellInspireCourage.Id;
    Spell InspireSpell = new Spell(InspireAction);
    repertoire.SpellsKnown.Add(InspireSpell);

    CombatAction CounterPerformanceAction = SpellCounterPerformance.CombatAction(null, sheet.MaximumSpellLevel, false);
    CounterPerformanceAction.SpellId = SpellCounterPerformance.Id;
    Spell CounterPerformanceSpell = new Spell(CounterPerformanceAction);
    repertoire.SpellsKnown.Add(CounterPerformanceSpell);
    sheet.FocusPointCount += 1;

    sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("BardCantrips", "Cantrips", 1, Trait.Bard, spellList, 0, 5));
    sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("BardSpells", "Level 1 spells", 1, Trait.Bard, spellList, 1, 2));
    sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("BardSpells2", "Level 1 spell", 2, Trait.Bard, spellList, 1, 1));
    sheet.AddSelectionOption((SelectionOption)new SignatureSpellSelectionOption("BardSignatureSpell1", "Signature spell", 3, 1, Trait.Bard));
    sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("BardSpells3", "Level 2 spells", 3, Trait.Bard, spellList, 2, 2));
    sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("BardSpells4", "Level 2 spell", 4, Trait.Bard, spellList, 2, 1));
    repertoire.SpellSlots[1] = 2;
    sheet.AddAtLevel(2, (_ => ++repertoire.SpellSlots[1]));
    sheet.AddAtLevel(3, (_ => repertoire.SpellSlots[2] += 2));
    sheet.AddAtLevel(3, (values => values.SetProficiency(Trait.Reflex, Proficiency.Expert)));
    sheet.AddAtLevel(4, (_ => ++repertoire.SpellSlots[2]));
    /*
  ++sheet.FocusPointCount;
  sheet.FocusSpellsKnown.Add(AllSpells.CreateModernSpell(focusSpell, (Creature) null, sheet.MaximumSpellLevel, false));
  repertoire.SpellsKnown.Add(AllSpells.CreateModernSpell(grantedCantrip, (Creature) null, sheet.MaximumSpellLevel, false));
  repertoire.SpellsKnown.Add(AllSpells.CreateModernSpell(grantedLevel1Spell, (Creature) null, 1, false));
  repertoire.SpellsKnown.Add(AllSpells.CreateModernSpell(grantedLevel2Spell, (Creature) null, 2, false));
  */
  }).WithCustomName("Bard");


    public static Feat CantripExpansion = new TrueFeat(FeatName.CustomFeat, 1, "A greater understanding of your magic broadens your range of simple spells.", "Add two additional cantrips from your spell list to your repertoire.", new Trait[2]
    {
        Trait.Bard,
        DawnniExpanded.DETrait
    }).WithOnSheet((values =>
    {
      if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
        return;
      values.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("CantripExpansionBard", "Cantrip Expansion cantrips", -1, Trait.Bard, values.SpellRepertoires[Trait.Bard].SpellList, 0, 2));
    })).WithCustomName("Bard Cantrip Expansion");

    public static Feat AbundantLevel1 = new TrueFeat(FeatName.CustomFeat, 1, "The wellspring of magic within you coalesces into additional spells.", "You gain an extra level 1 spell slot.", new Trait[3]
    {
        Trait.Bard,
        DawnniExpanded.DETrait,
        DawnniExpanded.HomebrewTrait
    }).WithOnSheet((values =>
    {
      if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
        return;
      ++values.SpellRepertoires[Trait.Bard].SpellSlots[1];
    })).WithCustomName("Bard Abundant Spellcasting 1");

    public static Feat AbundantLevel2 = new TrueFeat(FeatName.CustomFeat, 4, "The wellspring of magic within you coalesces into additional spells.", "You gain an extra level 2 spell slot.", new Trait[3]
    {
        Trait.Bard,
        DawnniExpanded.DETrait,
        DawnniExpanded.HomebrewTrait
    }).WithOnSheet((values =>
    {
      if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
        return;
      ++values.SpellRepertoires[Trait.Bard].SpellSlots[2];
    })).WithCustomName("Bard Abundant Spellcasting 2");

    public static Feat HymnOfHealing = new TrueFeat(FeatName.CustomFeat, 1, "You imbue your music with rich melodies that help your allies recover from harm.",
    "You learn the {i}hymn of healing{/i} composition spell.", new Trait[2]
    {
        Trait.Bard,
        DawnniExpanded.DETrait
    })
    .WithOnSheet((values =>
    {
      if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
        return;
      values.AddFocusSpellAndFocusPoint(Trait.Bard, Ability.Charisma, SpellHymnOfHealing.Id);

    })).WithCustomName("Hymn of Healing")
    .WithIllustration(SpellHymnOfHealing.SpellIllustration)
    .WithRulesBlockForSpell(SpellHymnOfHealing.Id, Trait.Occult);

    public static Feat TripleTime = new TrueFeat(FeatName.CustomFeat, 4, "You imbue your music with melodies which speed up you and your allies.",
    "You learn the {i}triple time{/i} composition cantrip.", new Trait[2]
    {
        Trait.Bard,
        DawnniExpanded.DETrait
    })
    .WithOnSheet((values =>
    {
      if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
        return;
      values.AddFocusSpellAndFocusPoint(Trait.Bard, Ability.Charisma, SpellTripleTime.Id);
    })).WithCustomName("Triple Time")
    .WithIllustration(SpellTripleTime.SpellIllustration)
    .WithRulesBlockForSpell(SpellTripleTime.Id, Trait.Occult);


    public static Feat LingeringComposition = new TrueFeat(FeatName.CustomFeat, 1, "You add a flourish to your composition to extend its benefits.", "If your next action is to cast a cantrip composition with a duration of 1 round, attempt a check using your performance skill. The DC is usually a standard-difficulty DC of a level  The effect depends on the result of your check.", new Trait[7]
    {
        Trait.Enchantment,
        Trait.Uncommon,
        Trait.Bard,
        Trait.Focus,
        Trait.Concentrate,
        Trait.Metamagic,
        DawnniExpanded.DETrait
    }).WithActionCost(0).WithPermanentQEffect("You can extend the duration of your composition cantrips.", (qf => qf.MetamagicProvider = new MetamagicProvider("Lingering Composition", (Func<CombatAction, CombatAction>)(spell =>
    {
      CombatAction metamagicSpell = Spell.DuplicateSpell(spell).CombatActionSpell;
      if (metamagicSpell.ActionCost != 1 || !metamagicSpell.HasTrait(Trait.Cantrip) || !metamagicSpell.HasTrait(Trait.Composition))
        return (CombatAction)null;

      metamagicSpell.Name = "Lingering Composition " + metamagicSpell.Name;
      metamagicSpell.Traits.Add(Trait.Focus);


      return metamagicSpell;


    })))).WithCustomName("Lingering Composition");


    public static void LoadMod()
    {
      BardClass.Traits.Add(DawnniExpanded.DETrait);
      ModManager.AddFeat(BardClass);
      ModManager.AddFeat(CantripExpansion);
      ModManager.AddFeat(AbundantLevel1);
      ModManager.AddFeat(AbundantLevel2);
      ModManager.AddFeat(LingeringComposition);
      ModManager.AddFeat(HymnOfHealing);
      ModManager.AddFeat(TripleTime);


      AllFeats.All.RemoveAll(feat => feat.FeatName == FeatName.ReachSpell);
      ModManager.AddFeat(

      new TrueFeat(FeatName.ReachSpell, 1, "You can extend the range of your spells.", "You can spend an extra action as you cast a spell in order to increase that spell's range by 30 feet. If the spell had a range of touch, you extend its range to 30 feet.", new Trait[7]
    {
        Trait.Sorcerer,
        Trait.Cleric,
        Trait.Wizard,
        Trait.Bard,
        Trait.Druid,
        Trait.Concentrate,
        Trait.Metamagic
    }).WithActionCost(1).WithPermanentQEffect("You can extend the range of your spells.", (Action<QEffect>)(qf => qf.MetamagicProvider = new MetamagicProvider("Reach spell", (Func<CombatAction, CombatAction>)(spell =>
    {
      CombatAction metamagicSpell = Spell.DuplicateSpell(spell).CombatActionSpell;
      if (metamagicSpell.ActionCost == 3 || Constants.IsVariableActionCost(metamagicSpell.ActionCost) || metamagicSpell.ActionCost == -2)
        return (CombatAction)null;
      switch (metamagicSpell.Target)
      {
        case CreatureTarget creatureTarget2:
          if (!IncreaseTarget(creatureTarget2))
            return (CombatAction)null;
          break;
        case MultipleCreatureTargetsTarget creatureTargetsTarget2:
          bool flag = false;
          foreach (CreatureTarget target in creatureTargetsTarget2.Targets)
            flag |= IncreaseTarget(target);
          if (!flag)
            return (CombatAction)null;
          break;
        default:
          return (CombatAction)null;
      }
      metamagicSpell.Name = "Reach " + metamagicSpell.Name;
      ++metamagicSpell.ActionCost;
      int num = metamagicSpell.Target.ToDescription().Count<char>((Func<char, bool>)(c => c == '\n'));
      string[] strArray = metamagicSpell.Description.Split('\n', 4 + num);
      if (strArray.Length >= 4)
        metamagicSpell.Description = strArray[0] + "\n" + strArray[1] + "\n{Blue}" + metamagicSpell.Target.ToDescription() + "{/Blue}\n" + strArray[3 + num];
      return metamagicSpell;

      bool IncreaseTarget(CreatureTarget creatureTarget)
      {
        if (creatureTarget.RangeKind == RangeKind.Melee)
        {
          metamagicSpell.Traits = new Traits(metamagicSpell.Traits.Except<Trait>((IEnumerable<Trait>)new Trait[1]
          {
              Trait.Melee
          }).Concat<Trait>((IEnumerable<Trait>)new Trait[1]
          {
              Trait.Ranged
          }));
          creatureTarget.RangeKind = RangeKind.Ranged;
          creatureTarget.CreatureTargetingRequirements.RemoveAll((Predicate<CreatureTargetingRequirement>)(ctr => ctr is AdjacencyCreatureTargetingRequirement || ctr is AdjacentOrSelfTargetingRequirement));
          creatureTarget.CreatureTargetingRequirements.Add((CreatureTargetingRequirement)new MaximumRangeCreatureTargetingRequirement(6));
          creatureTarget.CreatureTargetingRequirements.Add((CreatureTargetingRequirement)new UnblockedLineOfEffectCreatureTargetingRequirement());
          return true;
        }
        MaximumRangeCreatureTargetingRequirement targetingRequirement = creatureTarget.CreatureTargetingRequirements.OfType<MaximumRangeCreatureTargetingRequirement>().FirstOrDefault<MaximumRangeCreatureTargetingRequirement>();
        if (targetingRequirement == null)
          return false;
        targetingRequirement.Range += 6;
        return true;
      }
    })))));




    }

  }

}