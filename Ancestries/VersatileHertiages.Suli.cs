using Dawnsbury.Audio;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using Humanizer;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;



namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{
    public class VersatileHertiageSuli
    {
        public static Trait SuliTrait = ModManager.RegisterTrait(
        "Suli",
        new TraitProperties("Suli", true, "", false)
        {
            IsAncestryTrait = true
        });
        public static ModdedIllustration ElementalIllustration = new ModdedIllustration("DawnniburyExpandedAssets/ElementalAssault.png");
        public static Feat Hertiage = new VersatileHeritageSelectionFeat("Suli",
         "You are descended from a janni or otherwise embody a dichotomy of opposing elemental planar forces.",
         "You gain the suli trait, in addition to the traits from your ancestry. Your vision improves, granting you a +1 bonus to perception. You can choose from suli feats and feats from your ancestry whenever you gain an ancestry feat.", new List<Trait> { DawnniExpanded.DETrait, SuliTrait, Trait.Uncommon }, SuliTrait
         ).WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet => sheet.Ancestries.Add(SuliTrait)))
         .WithOnCreature(delegate (Creature creature)
            {
                creature.Traits.Add(SuliTrait);
                creature.Perception += 1;
            });


        public static Feat ElementalEmbellish = new TrueFeat(FeatName.CustomFeat, 1, "You can summon a harmless but impressive elemental display.", "You become trained in Intimidation. If you would automatically become trained in Intimidation (from your background or class, for example), you instead become trained in a skill of your choice.\n\nYou gain the Intimidating Glare feat.", new Trait[]
  {
        SuliTrait, DawnniExpanded.DETrait
    }).WithOnSheet((sheet =>
  {
      sheet.GrantFeat(FeatName.IntimidatingGlare);
      if (sheet.GetProficiency(Trait.Athletics) == Proficiency.Untrained)
      {
          sheet.GrantFeat(FeatName.Athletics);
      }
      else
      {
          sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Elemental Embellish Skill",
                  "Elemental Embellish skill",
                  -1,
                  (ft) => ft is SkillSelectionFeat)

                  );
      }

  })).WithCustomName("Elemental Embellish");

        public static Feat ElementalAssault = new TrueFeat(FeatName.CustomFeat, 1, "{b}Usage{/b} Once a day.\n\nYou shroud your arms and held weapons in elemental magic.", "Choose one element. Until the end of your next turn, your Strikes deal an additional 1d6 damage of the indicated type corresponding to the element: electricity for air, bludgeoning for earth, fire for fire, or cold for water.", new Trait[]
        {
        SuliTrait, DawnniExpanded.DETrait
          }).WithIllustration(ElementalIllustration)
          .WithOnCreature((Creature =>
        {
            Creature.AddQEffect(new QEffect()
            {
                ProvideActionIntoPossibilitySection = (QEffect qfself, PossibilitySection possibilitySection1) =>
                                     {
                                         if (possibilitySection1.PossibilitySectionId != PossibilitySectionId.MainActions)
                                         {
                                             return null;
                                         }

                                         SubmenuPossibility submenuPossibility = new SubmenuPossibility(ElementalIllustration, "Elemental Assault");
                                         PossibilitySection possibilitySection = new PossibilitySection("Elemental Assault");

                                         ActionPossibility FireAction = ElementalAssaultAction(Creature, DamageKind.Fire, Trait.Fire);
                                         possibilitySection.AddPossibility(FireAction);

                                         ActionPossibility ColdAction = ElementalAssaultAction(Creature, DamageKind.Cold, Trait.Water);
                                         possibilitySection.AddPossibility(ColdAction);

                                         ActionPossibility ElectricityAction = ElementalAssaultAction(Creature, DamageKind.Electricity, Trait.Air);
                                         possibilitySection.AddPossibility(ElectricityAction);

                                         ActionPossibility EarthAction = ElementalAssaultAction(Creature, DamageKind.Acid, Trait.Earth);
                                         possibilitySection.AddPossibility(EarthAction);


                                         submenuPossibility.Subsections.Add(possibilitySection);

                                         return submenuPossibility;
                                     }
            });


        })).WithCustomName("Elemental Assault{icon:Action}");

        private static CombatAction ElementalAssaultAction(Creature creature, DamageKind dmgElement, Trait traitElement)
        {
            SfxName sfxName;

            string ActionName = "Elemental Assault (" + EnumHumanizeExtensions.Humanize((Enum)traitElement) + ")";

            switch (dmgElement)
            {
                case DamageKind.Fire:
                    sfxName = SfxName.Fireball;
                    break;
                case DamageKind.Cold:
                    sfxName = SfxName.RayOfFrost;
                    break;
                case DamageKind.Electricity:
                    sfxName = SfxName.ElectricBlast;
                    break;
                case DamageKind.Acid:
                    sfxName = SfxName.AcidSplash;
                    break;
                default:
                    sfxName = SfxName.SpellFail;
                    break;
            }

            CombatAction ElementalAssaultAction = new CombatAction(creature, ElementalIllustration, ActionName, new Trait[] { Trait.Concentrate, Trait.Arcana, Trait.Evocation, SuliTrait, Trait.Basic, DawnniExpanded.DETrait, traitElement },
                              "{i}You shroud your arms and held weapons in elemental magic.{/i}\n\nUntil the end of your next turn, your Strikes deal an additional 1d6 " + EnumHumanizeExtensions.Humanize((Enum)dmgElement).ToLower() + " damage.",
                                  Target.Self().WithAdditionalRestriction((a) =>
                                  {
                                      if (a.PersistentUsedUpResources.UsedUpActions.Contains("ElementalAssault"))
                                      {
                                          return "You have already used Elemental Assault today.";
                                      }
                                      else return null;
                                  })

                              )
                              .WithSoundEffect(sfxName)
                              .WithActionCost(1)

                          .WithEffectOnSelf(async (CombatAction spell, Creature caster) =>
                          {
                              caster.PersistentUsedUpResources.UsedUpActions.Add("ElementalAssault");
                              caster.AddQEffect(new QEffect(ActionName, "Your Strikes deal an additional 1d6 " + EnumHumanizeExtensions.Humanize((Enum)dmgElement).ToLower() + " damage.", ExpirationCondition.ExpiresAtEndOfYourTurn, caster)
                              {
                                  Illustration = ElementalIllustration,
                                  AddExtraStrikeDamage = (action, target) =>
                      {

                          return (DiceFormula.FromText("1d6", ActionName), dmgElement);
                      },
                                  CannotExpireThisTurn = true,
                              }
                              );

                          });


            return ElementalAssaultAction;
        }



        public static void LoadMod()
        {
            ModManager.AddFeat(Hertiage);
            ModManager.AddFeat(ElementalEmbellish);
            ModManager.AddFeat(ElementalAssault);
        }
    }

}

