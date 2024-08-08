using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Possibilities;
using System.Collections.Generic;
using System;
using Dawnsbury.Display.Text;
using Dawnsbury.Core.Creatures.Parts;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
namespace Dawnsbury.Mods.DawnniExpanded;

public class StaffModification : ItemModification
{
    public StaffModification(int staffCharges, string staffCaster) : base(ItemModificationKind.UsedThisDay)
    {

        StaffCharges = staffCharges;
        StaffCaster = staffCaster;
    }

    public int StaffCharges
    {
        get; set;
    } = 0;

    public string StaffCaster
    {
        get; set;
    }

}


public class SpellStaff : Item
{
    public SpellStaff(IllustrationName illustration, string name, params Trait[] traits) : base(illustration, name, traits)
    {

    }
    public SpellStaff(ItemName itemName, IllustrationName illustration, string name, int level, int price, params Trait[] traits) : base(itemName, (Illustration)illustration, name, level, price, traits)
    {
        this.Traits.Add(ItemSpellStaff.SpellStaffTrait);
        this.Traits.Add(Trait.ItemUsableOncePerDay);
    }

    public List<Trait> StaffTraditions = new List<Trait>();


    public Dictionary<SpellId, int> SpellsInStaff = new Dictionary<SpellId, int>();


    public SpellStaff AddSpelltoStaff(SpellId spellId, int level)
    {
        SpellsInStaff.Add(spellId, level);
        return this;
    }

    public SpellStaff WithSpellTradition(Trait tradition)
    {
        StaffTraditions.Add(tradition);
        Traits.Add(tradition);
        return this;
    }


}

public static class ItemSpellStaff
{
    public static Trait SpellStaffTrait = ModManager.RegisterTrait(
               "Spell Staff",
               new TraitProperties("Spell Staff", true, "\n\nThis magic item holds spells of a particular theme and allows a spellcaster to cast additional spells by preparing the staff.")
               {
               }
               );


    public static Possibility AddSpellToStaff(Item staff, Creature caster, SpellId spellid, int spellLevel)
    {

        CombatAction spellInCombat = AllSpells.CreateSpellInCombat(spellid, caster, spellLevel, caster.PersistentCharacterSheet.Class.ClassTrait);

        spellInCombat.Traits.Add(SpellStaffTrait);
        spellInCombat.SpellcastingSource = caster.Spellcasting.PrimarySpellcastingSource;

        if (spellInCombat.SavingThrow != null)
            spellInCombat.WithSpellSavingThrow(new Defense?(spellInCombat.SavingThrow.Defense));

        spellInCombat.Illustration = new SideBySideIllustration((Illustration)IllustrationName.Quarterstaff, spellInCombat.Illustration);
        spellInCombat.Name = staff.Name + " {i}(" + spellInCombat.Name + " level " + spellLevel + "){/i}";

        Possibility spellPossibility = Dawnsbury.Core.Possibilities.Possibilities.CreateSpellPossibility(spellInCombat);
        spellPossibility.PossibilitySize = PossibilitySize.Half;
        return spellPossibility;

    }

    public static void LoadMod()
    {


        ModManager.RegisterActionOnEachCreature(creature =>
        {

            SpellStaff StartingStaffCheck = (SpellStaff)creature.HeldItems.FirstOrDefault(staff => staff is SpellStaff);

            creature.AddQEffect(
                new QEffect()
                {

                    StateCheck = (QEffect qf) =>
         {
             SpellStaff StaffCheck = (SpellStaff)qf.Owner.HeldItems.FirstOrDefault(staff => staff is SpellStaff);

             if (StaffCheck == null)
             {
                 return;
             }

             Creature owner = qf.Owner;

             if (owner.Spellcasting == null)
             {
                 return;
             }

             if (owner.Name == StaffCheck.Name)
             {
                 return;
             }

             StaffModification StaffMod = (StaffModification)StaffCheck.ItemModifications.FirstOrDefault(x => x is StaffModification);

             if (StaffMod == null)
             {
                 return;
             }

             owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
             {
                 PreventTakingAction = (CombatAction ca) => !ca.HasTrait(Trait.Cantrip) && ca.HasTrait(SpellStaffTrait) && ca.HasTrait(Trait.Spell) && ca.SpellLevel > StaffMod.StaffCharges ? "You don't have enough staff charges" : null,
             });

             List<Possibility> ReadyStaffSpellPossibilites = [];

             foreach (KeyValuePair<SpellId, int> spell in StaffCheck.SpellsInStaff)
             {
                 int SpellLevel = spell.Value;

                 if (SpellLevel == 0)
                 {
                     SpellLevel = owner.MaximumSpellRank;
                 }

                 ReadyStaffSpellPossibilites.Add(AddSpellToStaff(StaffCheck, owner, spell.Key, SpellLevel));
             }

             if (ReadyStaffSpellPossibilites.Count() > 0)
             {


                 owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)

                 {


                     ProvideActionIntoPossibilitySection = (qfB, section) =>
                      {
                          if (section.PossibilitySectionId != PossibilitySectionId.ItemActions)
                              return (Possibility)null;

                          return (Possibility)new SubmenuPossibility(StaffCheck.Illustration, StaffCheck.Name)
                          {
                              Subsections = {
                        new PossibilitySection(StaffCheck.Name + " " + string.Join("", Enumerable.Repeat<string>("{icon:SpontaneousSpellSlot}", StaffMod.StaffCharges)))
                        {
                        Possibilities = ReadyStaffSpellPossibilites
                        }
                              }
                          };
                      },

                     AfterYouTakeAction = async (QEffect qf, CombatAction hostileAction) =>
             {
                 if (hostileAction.HasTrait(SpellStaffTrait) && hostileAction.HasTrait(Trait.Spell) && !hostileAction.HasTrait(Trait.Cantrip) && StaffMod.StaffCharges >= hostileAction.SpellLevel)
                 {
                     StaffMod.StaffCharges -= hostileAction.SpellLevel;
                 }
             }

                 });
             }


         },


                    StartOfCombat = async (QEffect qf) =>
                    {


                        Creature owner = qf.Owner;

                        if (owner.Spellcasting == null)
                        {
                            return;
                        }

                        string StaffChargesString = owner.PersistentUsedUpResources.UsedUpActions.Find(word => word.Contains("StaffCharged"));

                        SpellStaff StartingStaffCheck = (SpellStaff)qf.Owner.HeldItems.FirstOrDefault(staff => staff is SpellStaff);

                        if (StartingStaffCheck == null)
                        {
                            return;
                        }

                        if (StartingStaffCheck == null && StaffChargesString == null)
                        {
                            owner.PersistentUsedUpResources.UsedUpActions.Add("StaffCharged");
                            return;
                        }

                        if (StaffChargesString == null)
                        {




                            int highestSpellLevel = 0;
                            List<Trait> OwnerTrads = [];

                            foreach (SpellcastingSource item in owner.Spellcasting.Sources)
                            {
                                OwnerTrads.Add(item.SpellcastingTradition);

                                for (int i = 1; i < item.SpontaneousSpellSlots.Length; i++)
                                {
                                    if (item.SpontaneousSpellSlots[i] != 0 && i > highestSpellLevel)
                                    {
                                        highestSpellLevel = i;
                                    }
                                }

                            }

                            foreach (KeyValuePair<string, Spell?> item in owner.PersistentCharacterSheet.PreparedSpells)
                            {
                                Spell spell = item.Value;

                                if (spell.SpellLevel > highestSpellLevel && !spell.HasTrait(Trait.Cantrip) && !spell.HasTrait(Trait.Focus))
                                {
                                    highestSpellLevel = spell.SpellLevel;
                                };
                            }


                            owner.PersistentUsedUpResources.UsedUpActions.Add("StaffCharged");

                            if (!OwnerTrads.Intersect(StartingStaffCheck.StaffTraditions).Any())
                            {
                                return;
                            }

                            StartingStaffCheck.ItemModifications.Add(new StaffModification(highestSpellLevel, owner.Name));

                        }







                    }

                });
        });
    }



}
