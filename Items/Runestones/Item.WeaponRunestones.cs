using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemRunestone
{

  public static Trait WeaponRunestone = ModManager.RegisterTrait(
              "Weapon Runestone",
              new TraitProperties("Weapon Runestone", true, "\n\nRunestones confer typically passive benefits to all your weapons and unarmed strikes. You may only have one Runestone active.\n\nModder's Note: This is a game limitation workaround since there's no property rune system.")
              {
              }
              );
  public static void LoadMod()
  {
    ItemDisrupting.LoadMod();
    ItemFearsome.LoadMod();
    ItemWounding.LoadMod();
    ItemCrushing.LoadMod();
    ItemFrost.LoadMod();
    ItemFlaming.LoadMod();
    ItemShock.LoadMod();
    ItemThundering.LoadMod();
    ItemCorrosive.LoadMod();
    ItemSpellPotency.LoadMod();
  }
}