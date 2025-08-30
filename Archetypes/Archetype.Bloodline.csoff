// Decompiled with JetBrains decompiler
// Type: Dawnsbury.Core.CharacterBuilder.Feats.Bloodline
// Assembly: Dawnsbury Days, Version=2.27.2024.229, Culture=neutral, PublicKeyToken=null
// MVID: EEE708E9-2901-41FC-B288-9A8CB93A43DA
// Assembly location: E:\SteamLibrary\steamapps\common\Dawnsbury Days\Data\Dawnsbury Days.dll
// XML documentation location: E:\SteamLibrary\steamapps\common\Dawnsbury Days\Data\Dawnsbury Days.xml

using Dawnsbury.Auxiliary;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.ThirdParty.SteamApi;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core;

#nullable enable
namespace Dawnsbury.Mods.DawnniExpanded;
public class ArchetypeBloodline : Feat
{
  public readonly SpellId focusSpell;
  private readonly SpellId grantedLevel1Spell;
  private readonly SpellId grantedLevel2Spell;

  public ArchetypeBloodline(
    FeatName featName,
#nullable disable
    string flavorText,
    Trait spellList,
    SpellId focusSpell,
    SpellId grantedCantrip,
    SpellId grantedLevel1Spell,
    SpellId grantedLevel2Spell)
    : base(FeatName.CustomFeat, flavorText, "• Spell list: {b}" + spellList.ToString() + "{/b} {i}" + ArchetypeBloodline.ExplainSpellList(spellList) + "{/i}\n• Focus spell: " + AllSpells.CreateModernSpellTemplate(focusSpell, Trait.Sorcerer).ToSpellLink() + "\n• Bloodline-granted spells: cantrip: " + AllSpells.CreateModernSpellTemplate(grantedCantrip, Trait.Sorcerer).ToSpellLink() + ", 1st: " + AllSpells.CreateModernSpellTemplate(grantedLevel1Spell, Trait.Sorcerer).ToSpellLink() + ", 2nd: " + AllSpells.CreateModernSpellTemplate(grantedLevel2Spell, Trait.Sorcerer).ToSpellLink(), new List<Trait>(), (List<Feat>)null)
  {
    this.CustomName = featName.Humanize() + " (Archetype)";
    this.focusSpell = focusSpell;
    this.grantedLevel1Spell = grantedLevel1Spell;
    this.grantedLevel2Spell = grantedLevel2Spell;
    this.WithRulesBlockForSpell(focusSpell, spellList);
    this.OnSheet = (Action<CalculatedCharacterSheetValues>)(sheet =>
    {
      if (sheet.Sheet.Class?.ClassTrait == Trait.Sorcerer) return; // Do nothing if you're already this class. This feat will be removed in the next cycle due to a failed prerequisite anyway.
      sheet.SpellTraditionsKnown.Add(spellList);
      sheet.SpellRepertoires.Add(Trait.Sorcerer, new SpellRepertoire(Ability.Charisma, spellList));
      sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
      SpellRepertoire repertoire = sheet.SpellRepertoires[Trait.Sorcerer];
      sheet.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("SorcererCantripsArchetype", "Cantrips", -1, Trait.Sorcerer, spellList, 0, 2));
    });
  }

  private static string ExplainSpellList(Trait spellList)
  {
    if (spellList == Trait.Arcane)
      return "(You cast arcane spells. Arcane spells are extremely varied, include powerful offensive and debuffing spells, but cannot heal your allies. Only arcane sorcerers and wizards can cast arcane spells.)";
    return spellList != Trait.Occult ? "(You cast divine spells. Divine spells can heal or buff your allies and are powerful against the undead, but they can lack utility or offensive power against natural creatures. Only divine sorcerers and clerics can cast divine spells.)" : "(You can cast occult spells. Occult spells focus on enchantment, emotion and the mind. They inflict debuffs on your opponents and grant buffs to your allies, but generally can't manipulate energy. Only occult sorcerers and psychics can cast occult spells.)";
  }

}