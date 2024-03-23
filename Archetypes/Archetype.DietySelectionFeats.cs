#region Assembly Dawnsbury Days, Version=2.40.2024.312, Culture=neutral, PublicKeyToken=null
// E:\SteamLibrary\steamapps\common\Dawnsbury Days\Data\Dawnsbury Days.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Humanizer;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;


namespace Dawnsbury.Mods.DawnniExpanded;

public class ArchetypeDeitySelectionFeat : Feat
{
    public NineCornerAlignment[] AllowedAlignments { get; }

    public FeatName[] AllowedFonts { get; }

    public FeatName[] AllowedDomains { get; }

    public ItemName FavoredWeapon { get; }

    public ArchetypeDeitySelectionFeat(String deityName,
     string flavorText,
    string editsAndAnathema,
    NineCornerAlignment[]
    allowedAlignments,
    FeatName[] allowedFonts,
    FeatName[] allowedDomains,
    ItemName favoredWeapon,
    SpellId[] extraSpells)
        : base(FeatName.CustomFeat, flavorText, ComposeDeityRulesText(editsAndAnathema, allowedAlignments, allowedFonts, allowedDomains, favoredWeapon, extraSpells), new List<Trait> { DawnniExpanded.DETrait }, null)
    {
        ArchetypeDeitySelectionFeat deitySelectionFeat = this;
        AllowedAlignments = allowedAlignments;
        AllowedFonts = allowedFonts;
        AllowedDomains = allowedDomains;
        FavoredWeapon = favoredWeapon;
        WithPrerequisite((CalculatedCharacterSheetValues values) => deitySelectionFeat.AllowedAlignments.Contains(values.NineCornerAlignment), "You don't meet the alignment restriction of this deity.");
        WithCustomName(deityName);
        WithOnSheet(delegate (CalculatedCharacterSheetValues values)
        {

        });
    }

    private static string ComposeDeityRulesText(string editsAndAnathema, NineCornerAlignment[] allowedAlignments, FeatName[] allowedFonts, FeatName[] allowedDomains, ItemName favoredWeapon, SpellId[] extraSpells)
    {
        return editsAndAnathema + "\n{b}• Allowed alignments{/b} " + Alignments.Describe(allowedAlignments) + "\n{b}• Divine font{/b} " + ((allowedFonts.Length == 1) ? EnumHumanizeExtensions.Humanize((Enum)allowedFonts[0], (LetterCasing)2) : "healing font or harmful font") + "\n{b}• Allowed domains{/b} " + string.Join(", ", allowedDomains.Select((FeatName d) => EnumHumanizeExtensions.Humanize((Enum)d).ToLower())) + "\n{b}• Favored weapon{/b} " + EnumHumanizeExtensions.Humanize((Enum)favoredWeapon).ToLower() + "\n{b}• Extra allowed spells{/b} " + string.Join(", ", extraSpells.Select((SpellId id) => AllSpells.CreateModernSpellTemplate(id, Trait.Cleric).ToSpellLink())) + "\n\nExplanation: {i}You must be one of the allowed alignments. You receive extra {i}heal{/i} or {i}harm{/i} spells based on your deity's divine font. You can select from among the allowed domains with the Domain Initiate feat. You're Trained in your deity's favored weapon. You can choose from the extra allowed spells when you make your daily preparations in addition to the divine spell list.{/i}";
    }
}
