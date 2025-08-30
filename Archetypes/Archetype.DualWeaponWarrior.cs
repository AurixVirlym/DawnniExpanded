using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using System.Linq;
using System;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;


namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeDualWeaponWarrior
{

  public static Feat DualWeaponWarriorDedicationFeat;
  public static readonly Trait DualWeaponWarriorTrait = ModManager.RegisterTrait("DE_DualWeaponWarrior", new TraitProperties("Dual Weapon Warrior", false));
  public static void LoadMod()

  {
    DualWeaponWarriorDedicationFeat = ArchetypeFeats.CreateAgnosticArchetypeDedication(DualWeaponWarriorTrait, 
      "You're able to effortlessly fight with multiple weapons simultaneously, weaving your weapons together into a storm of quick attacks. To you, continual offense is the best form of defense, and you leave little room for your foes to avoid your whirlwind of weapons.",
      "You're exceptional in your use of two weapons. You gain the Double Slice fighter feat. This serves as Double Slice for the purpose of meeting prerequisites.").WithOnSheet(sheet =>
    {
      sheet.GrantFeat(FeatName.DoubleSlice);
    });
    DualWeaponWarriorDedicationFeat.Traits.Add(DawnniExpanded.DETrait);
    ModManager.AddFeat(DualWeaponWarriorDedicationFeat);
  }
}
  