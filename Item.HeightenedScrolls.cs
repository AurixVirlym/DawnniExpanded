using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Display.Illustrations;
using System;
using System.Linq;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core;
using Dawnsbury.Modding;
using System.Collections.Generic;
using Dawnsbury.Core.CombatActions;


namespace Dawnsbury.Mods.DawnniExpanded;
public class GenerateHeightenedScrolls
{
    private static int GetScrollPriceFromSpellLevel(int spellLevel)
    {
      switch (spellLevel)
      {
        case 1:
          return 4;
        case 2:
          return 12;
        case 3:
          return 30;
        case 4:
          return 70;
        case 5:
          return 150;
        case 6:
          return 300;
        case 7:
          return 600;
        case 8:
          return 1300;
        case 9:
          return 3000;
        case 10:
          return 8000;
        default:
          throw new Exception("This spell level does not exist.");
      }
    }

    
    public static void MakeScrollAtLevel(Spell spell1, int scrolllevel){

        Spell spell2 = spell1.Duplicate(null, scrolllevel, inCombat: true);
        spell2.CombatActionSpell.Description = spell2.CombatActionSpell.Description.Replace("Heightened to spell level 2.","Heightened to spell level "+scrolllevel+".");
            string text = "";
                    
                    if (spell2.HasTrait(Trait.Arcane))
                    {
                        text = "arcane";
                    }

                    if (spell2.HasTrait(Trait.Divine))
                    {
                        if (text != "")
                        {
                            text += " or ";
                        }

                        text += "divine";
                    }

                    if (spell2.HasTrait(Trait.Occult))
                    {
                        if (text != "")
                        {
                            text += " or ";
                        }

                        text += "occult";
                    }
            
            String ScrollDescription = "Casts the spell {i}" + spell2.Name.ToLower() + "{/i} once, then disintegrates into dust. Only heroes capable of casting {b}" + text + "{/b} spells can activate this scroll.\n\n" + spell2.CombatActionSpell.Description;
                
            List<Trait> scrolltraits = new List<Trait>();

           
            scrolltraits.AddRange(spell2.Traits);
            scrolltraits.Add(Trait.Scroll);
            scrolltraits.Add(Trait.Consumable);
            scrolltraits.Add(DawnniExpanded.DETrait);

            ModManager.RegisterNewItemIntoTheShop(spell2.Name.ToLower()+"H"+scrolllevel,itemName =>
            new Item(
                itemName,
                (Illustration) IllustrationName.None,
                "",
                scrolllevel * 2 - 1,
                GetScrollPriceFromSpellLevel(scrolllevel),
                DawnniExpanded.DETrait
            )
            {
                Illustration = new ScrollIllustration(IllustrationName.Scroll, spell2.Illustration),
                Name = "Heightened level " +scrolllevel+  " scroll of {i}" + spell2.Name.ToLower() + "{/i}",
                ScrollProperties = new ScrollProperties(spell2),
                Description = ScrollDescription,
                Traits = scrolltraits
               
            }
            );

    }

    public static void LoadMod()
    {
        
          foreach (Spell spell in AllSpells.All.Where<Spell>((Func<Spell, bool>) (sp => 
          !sp.HasTrait(Trait.Cantrip) 
          && !sp.HasTrait(Trait.Focus) 
          && !sp.HasTrait(Trait.Uncommon) 
          && sp.MinimumSpellLevel == 1
          && sp.CombatActionSpell.Description.Contains("Heightened (+1)"))))
          {
            
        MakeScrollAtLevel(spell, 2);
        MakeScrollAtLevel(spell, 3);
        MakeScrollAtLevel(spell, 4);
        }


}
}