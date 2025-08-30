using System;
using System.Collections.Generic;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Audio;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Illustrations;
using System.Linq;



namespace Dawnsbury.Mods.DawnniExpanded.Feats
{
    public class Familiars
    {
        public static ModdedIllustration FamiliarFocusIllustration = new ModdedIllustration("DawnniburyExpandedAssets/FamiliarFocus.png");
        public static ModdedIllustration RestorativeFamiliarIllustration = new ModdedIllustration("DawnniburyExpandedAssets/RestorativeFamiliar.png");
        public static Trait FamiliarAbilityTrait = ModManager.RegisterTrait(
                "Familiar Ability",
                new TraitProperties("Familiar Ability", true, "", false)
        {});

        public static Feat Familiar;

        public static Feat EnhancedFamiliar;

        public static void LoadMod(){

        Familiar = new TrueFeat(FeatName.CustomFeat,
            1,
            "You make a pact with a creature that serves you and assists your spellcasting.",
            "You gain a familiar, which grants you two familiar abilities.\n\nModders Note, familiar are not physical creatures in the game due to sharing spaces limitation and limited use of familiars.",
            new Trait[] { Trait.Wizard,Trait.Magus,Trait.Sorcerer,Trait.Druid, DawnniExpanded.DETrait, Trait.Homebrew })
            .WithCustomName("Familiar")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {
        int abilitynumber = 2;
        if (sheet.Sheet.Class?.ClassTrait == SpellHexes.ClassTrait)
        { abilitynumber = 3;}
            sheet.AddSelectionOption(
                new MultipleFeatSelectionOption(
                    "Familiar Ability Selection",
                    "Familiar Ability Selection",
                    -1,
                    (ft) => ft.HasTrait(FamiliarAbilityTrait), abilitynumber));


    });

            EnhancedFamiliar = new TrueFeat(FeatName.CustomFeat,
                    2,
                    "You infuse your familiar with additional magical energy.",
                    "You can select two more familiar abilities.",
                    new Trait[] { SpellHexes.ClassTrait, Trait.Wizard, Trait.Magus, Trait.Sorcerer, DawnniExpanded.DETrait, Trait.Homebrew })
                    .WithCustomName("Enhanced Familiar")
                    .WithEquivalent(values => values.AllFeats.Contains(ArchetypeFamiliarMaster.EnhancedFamiliarArchetype))
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(Familiar), "You must have a familiar.")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

            {
                int abilitynumber = 2;
                sheet.AddSelectionOption(
            new MultipleFeatSelectionOption(
                "Enhanced Familiar Ability Selection",
                "Enhanced Familiar Ability Selection",
                -1,
                (ft) => ft.HasTrait(FamiliarAbilityTrait), abilitynumber));
            });



            Feat FamiliarFocus = new TrueFeat(FeatName.CustomFeat,
                        1,
                        "Your familiar bond can help drawn out deeper reserves of your magical power.",
                        "Once per day, you can commune with your familiar as an action with the concentrate trait to restore 1 Focus Point to your focus pool, up to your usual maximum. You must have a focus pool to select this.",
                        new Trait[] { FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew })
                        .WithCustomName("Familiar Focus")
                        .WithPrerequisite((CalculatedCharacterSheetValues values) => values.FocusPointCount > 0, "You must have a focus pool.")
                        .WithPermanentQEffect("Once per day, you can recover 1 focus point.", (Action<QEffect>)(qf => qf.ProvideMainAction = (Func<QEffect, Possibility>)(qfSelf =>
      {
          Creature owner = qfSelf.Owner;
          if (owner.Spellcasting != null)
          {
              int focusPoints = owner.Spellcasting.FocusPoints;
              int? focusPointCount = owner.PersistentCharacterSheet?.Calculated.FocusPointCount;
              int valueOrDefault = focusPointCount.GetValueOrDefault();
              if (focusPoints < valueOrDefault & focusPointCount.HasValue && !owner.PersistentUsedUpResources.UsedUpActions.Contains("familiarFocus"))
                  return new ActionPossibility(new CombatAction(owner, FamiliarFocusIllustration, "Familiar Focus", new Trait[2]
                  {
              FamiliarAbilityTrait, Trait.Concentrate
                  }, "{b}Frequency{/b} once per day\n\nRecover 1 focus point.", (Target)Target.Self()).WithSoundEffect(SfxName.Bless).WithActionCost(1).WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
                  {
                      if (caster.Spellcasting == null)
                          return;
                      caster.PersistentUsedUpResources.UsedUpActions.Add("familiarFocus");
                      caster.Battle.Log(caster.Name + " communes with their familiar (1 focus point recovered).");
                      ++caster.Spellcasting.FocusPoints;
                  }))).WithPossibilityGroup("Familiar");
          }
          return (Possibility)null;
      }))); ;


            Feat RestorativeFamiliar = new TrueFeat(FeatName.CustomFeat,
                              1,
                              "Your familiar can offer up it's life force to restore your health.",
                              "Once per day, you can commune with your familiar as an action with the concentrate trait to give up some of its animating energy and heal you. It must be in your space to do so. You restore a number of Hit Points equal to 1d8 times half your level (minimum 1d8).",
                              new Trait[] { FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew })
                              .WithCustomName("Restorative Familiar")
                              .WithPermanentQEffect("Once per day, your familiar can recover your hp.", (Action<QEffect>)(qf => qf.ProvideMainAction = (Func<QEffect, Possibility>)(qfSelf =>
            {
                Creature owner = qfSelf.Owner;
                if (owner.Spellcasting != null)
                {
                        return new ActionPossibility(new CombatAction(owner, RestorativeFamiliarIllustration, "Restorative Familiar", new Trait[2]
                        {
              FamiliarAbilityTrait, Trait.Concentrate
                        }, "{b}Frequency{/b} once per day\n\nRestore "+ Math.Max((int)Math.Floor((double) owner.Level / 2), 1) + "d8", (Target)Target.Self()).WithSoundEffect(SfxName.Healing).WithActionCost(1).WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
                        {
                        string number = Math.Max((int)Math.Floor((double)caster.Level / 2),1) + "d8";
                        await caster.HealAsync(number, spell);
                        }))).WithPossibilityGroup("Familiar");
                }
                return (Possibility)null;
            }))); ;


            Feat ThreatDisplay = new TrueFeat(FeatName.CustomFeat, 1, "Your familiar helps you convey wordless threats through body language.", "Whenever you attempt an Intimidation check to Demoralize a creature, your familiar accompanies you with snarls, hisses, or raising its hackles. You don't take the normal -4 penalty on the Intimidation check if your target doesn't understand the language you're speaking.", new Trait[3]
           {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
           })
           .WithCustomName("Threat Display")
           .WithOnCreature((Action<Creature>)(creature => creature.AddQEffect(new QEffect("Threat Display", "You don't take the -4 Demoralize penalty when you demoralize a creature that doesn't understand your language.", ExpirationCondition.Never, creature)
           {
               Innate = true,
               Id = QEffectId.IntimidatingGlare
           })));

            Feat SpecialSense = new TrueFeat(FeatName.CustomFeat, 1, "Your familiar has a special sense such as keen nose or a sensitivity to tremors which helps you uncover secrets, traps or hidden foes.", "You gain a +2 circumstance bonus to the Seek action", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Special Senses")
            .WithOnCreature((Action<Creature>)(creature => creature.AddQEffect(new QEffect("Special Senses", "Due to your familiar, you gain a +2 circumstance bonus to the Seek action", ExpirationCondition.Never, creature)
            {
                BonusToAttackRolls = (qf, attack, target) =>
                    {
                        if (attack.ActionId.Equals(ActionId.Seek))
                        {
                            return new Bonus(2, BonusType.Circumstance, "Special Senses (Familiar)");
                        }
                        else return null;
                    },
            })));

            Feat SkilledTutor = new TrueFeat(FeatName.CustomFeat, 1, "Your familiar has a particular set of skills, which it can share with you.", "You become trained in a skill of your choice.", new Trait[3]
           {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
           }).WithMultipleSelection()
           .WithCustomName("Skilled Tutor")
           .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

            {
            sheet.AddSelectionOption(
                new SingleFeatSelectionOption(
                    "Skilled Tutor",
                    "Skilled Tutor",
                    -1,
                    (ft) => ft is SkillSelectionFeat)

                    );
        
            });




            Feat SorcererCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Sorcerer)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Sorcerer || values.AdditionalClassTraits.Contains(Trait.Sorcerer), "You must have Sorcerer as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                if (!values.SpellRepertoires.ContainsKey(Trait.Sorcerer))
                    return;
                values.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("CantripConnectionSorcerer", "Cantrip Connection cantrips", -1, Trait.Sorcerer, values.SpellRepertoires[Trait.Sorcerer].SpellList, 0, 1));
            }));

            Feat BardCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Bard)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Bard || values.AdditionalClassTraits.Contains(Trait.Bard), "You must have Bard as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                if (!values.SpellRepertoires.ContainsKey(Trait.Bard))
                    return;
                values.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("CantripConnectionBard", "Cantrip Connection cantrips", -1, Trait.Bard, values.SpellRepertoires[Trait.Bard].SpellList, 0, 1));
            }));

            Feat OracleCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Oracle)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Oracle || values.AdditionalClassTraits.Contains(Trait.Oracle), "You must have Oracle as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                if (!values.SpellRepertoires.ContainsKey(Trait.Oracle))
                    return;
                values.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("CantripConnectionOracle", "Cantrip Connection cantrips", -1, Trait.Oracle, values.SpellRepertoires[Trait.Oracle].SpellList, 0, 1));
            }));

            Feat PsychicCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Psychic)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Psychic || values.AdditionalClassTraits.Contains(Trait.Psychic), "You must have Psychic as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                if (!values.SpellRepertoires.ContainsKey(Trait.Psychic))
                    return;
                values.AddSelectionOption((SelectionOption)new AddToSpellRepertoireOption("CantripConnectionPsychic", "Cantrip Connection cantrips", -1, Trait.Psychic, values.SpellRepertoires[Trait.Psychic].SpellList, 0, 1));
            }));



            //prepared
            Feat WitchCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Witch)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == SpellHexes.ClassTrait || values.AdditionalClassTraits.Contains(SpellHexes.ClassTrait), "You must have Witch as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                values.PreparedSpells.GetValueOrDefault<Trait, PreparedSpellSlots>(SpellHexes.ClassTrait)?.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "WitchConnectionExpansion1"));
            }));

            Feat WizardCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Wizard)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Wizard || values.AdditionalClassTraits.Contains(Trait.Wizard), "You must have Wizard as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                values.PreparedSpells.GetValueOrDefault<Trait, PreparedSpellSlots>(Trait.Wizard)?.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "WizardConnectionExpansion1"));
            }));

            Feat MagusCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
           {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
           })
           .WithCustomName("Cantrip Connection (Magus)")
           .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Magus || values.AdditionalClassTraits.Contains(Trait.Magus), "You must have Magus as a Class or Archetype.")
           .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
           {
               values.PreparedSpells.GetValueOrDefault<Trait, PreparedSpellSlots>(Trait.Magus)?.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "MagusConnectionExpansion1"));
           }));

            Feat DruidCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Druid)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Druid || values.AdditionalClassTraits.Contains(Trait.Druid), "You must have Druid as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                values.PreparedSpells.GetValueOrDefault<Trait, PreparedSpellSlots>(Trait.Druid)?.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "DruidConnectionExpansion1"));
            }));

            Feat ClericCantripConnection = new TrueFeat(FeatName.CustomFeat, 1, "The magical bond between your familiar lets you prepare more spells", "Add one additional cantrip from your spell list to your repertoire.", new Trait[3]
            {
                FamiliarAbilityTrait, DawnniExpanded.DETrait, Trait.Homebrew
            })
            .WithCustomName("Cantrip Connection (Cleric)")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait == Trait.Cleric || values.AdditionalClassTraits.Contains(Trait.Cleric), "You must have Cleric as a Class or Archetype.")
            .WithOnSheet((Action<CalculatedCharacterSheetValues>)(values =>
            {
                values.PreparedSpells.GetValueOrDefault<Trait, PreparedSpellSlots>(Trait.Cleric)?.Slots.Add((PreparedSpellSlot)new FreePreparedSpellSlot(0, "ClericConnectionExpansion1"));
            }));



            ModManager.AddFeat(Familiar);
            ModManager.AddFeat(EnhancedFamiliar);
            ModManager.AddFeat(FamiliarFocus);
            ModManager.AddFeat(RestorativeFamiliar);
            ModManager.AddFeat(ThreatDisplay);
            ModManager.AddFeat(SpecialSense);
            ModManager.AddFeat(SorcererCantripConnection);
            ModManager.AddFeat(BardCantripConnection);
            ModManager.AddFeat(OracleCantripConnection);
            ModManager.AddFeat(PsychicCantripConnection);
            ModManager.AddFeat(OracleCantripConnection);
            ModManager.AddFeat(WitchCantripConnection);
            ModManager.AddFeat(WizardCantripConnection);
            ModManager.AddFeat(MagusCantripConnection);
            ModManager.AddFeat(DruidCantripConnection);
            ModManager.AddFeat(ClericCantripConnection);
            ModManager.AddFeat(SkilledTutor);
        }
    }
}