using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.ThirdParty.SteamApi;
using System;
using System.Linq;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core;



namespace Dawnsbury.Mods.DawnniExpanded;
public static class ArchetypeBarbarian
{

  private static DiceFormula RageDamageIncrease(
      CombatAction action,
      DiceFormula diceFormula,
      int increase,
      string increaseReason)
  {
    return DoesRageApplyToAction(action) ? (DiceFormula)diceFormula.Add(DiceFormula.FromText(increase.ToString(), increaseReason)) : diceFormula;
  }

  private static string CanEnterRage(Creature self)
  {
    if (self.HasEffect(QEffectId.Fatigued))
      return "You can't rage while fatigued.";
    if (self.HasEffect(QEffectId.Rage))
      return "You're already raging.";
    return !self.HasEffect(QEffectId.HasRagedThisEncounter) || self.HasEffect(QEffectId.SecondWind) ? (string)null : "You already raged this encounter.";
  }

  private static void EnterRage(Creature self)
  {
    Steam.CollectAchievement("BARBARIAN");
    Sfxs.Play(self.HasTrait(Trait.Female) ? (R.Coin() ? SfxName.RageFemale1 : SfxName.RageFemale2) : (R.Coin() ? SfxName.RageMale1 : SfxName.RageMale2));
    int thp = self.Level + self.Abilities.Constitution;
    self.GainTemporaryHP(thp);
    if (self.HasEffect(QEffectId.HasRagedThisEncounter))
      self.AddQEffect(new QEffect()
      {
        Id = QEffectId.HasRagedTwiceThisEncounter
      });
    self.AddQEffect(new QEffect()
    {
      Id = QEffectId.HasRagedThisEncounter
    });
    self.AddQEffect(new QEffect("Rage", "You deal +2 additional damage with melee Strike (or +1 with agile weapons).\n\nYou take a -1 penalty to AC.\n\nYou can't use Demoralize or other actions that require concentration.", ExpirationCondition.ExpiresAtStartOfSourcesTurn, self, (Illustration)IllustrationName.Rage)
    {
      Id = QEffectId.Rage,
      RoundsLeft = 10,
      DoNotShowUpOverhead = true,
      PreventTakingAction = (Func<CombatAction, string>)(ca => !ca.HasTrait(Trait.Concentrate) || ca.ActionId == ActionId.Seek || ca.ActionId == ActionId.Demoralize && self.HasEffect(QEffectId.RagingIntimidation) || ca.HasTrait(Trait.Rage) ? (string)null : "This action requires concentration and you can't use it while raging."),
      BonusToDefenses = (Func<QEffect, CombatAction, Defense, Bonus>)((_1, _2, defense) => defense != Defense.AC ? (Bonus)null : new Bonus(-1, BonusType.Untyped, "rage")),
      YouDealDamageWithStrike = (Delegates.YouDealDamageWithStrike)((effect, action, diceFormula, target) => RageDamageIncrease(action, diceFormula, action.HasTrait(Trait.Agile) ? 1 : 2, action.HasTrait(Trait.Agile) ? "Rage (agile)" : "Rage")),
      WhenExpires = (Action<QEffect>)(qfRage =>
      {
        if (!qfRage.Owner.HasEffect(QEffectId.HasRagedTwiceThisEncounter))
          return;
        qfRage.Owner.AddQEffect(QEffect.Fatigued());
      }),
      StateCheck = (Action<QEffect>)(qfRage =>
      {
        if (!qfRage.Owner.HasEffect(QEffectId.Unconscious))
          return;
        qfRage.ExpiresAt = ExpirationCondition.Immediately;
      })
    });
  }

  private static bool DoesRageApplyToAction(CombatAction action)
  {
    if (action.HasTrait(Trait.Melee))
      return true;
    return action.HasTrait(Trait.Thrown) && action.Owner.HasEffect(QEffectId.RagingThrower);
  }

  public static Feat BarbarianDedicationFeat;
  public static Trait BarbarianArchetypeTrait;
  public static void LoadMod()

  {

    BarbarianArchetypeTrait = ModManager.RegisterTrait(
        "BarbarianArchetype",
        new TraitProperties("BarbarianArchetype", false, "", false)
        {
        });

    BarbarianDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have spent time There’s a rage deep inside you that sometimes breaks loose, granting you some of the might of a barbarian in addition to your other abilities.",
            "You become trained in Athletics.\n\nif you were already trained in Athletics, you instead become trained in another skill of your choice.\n\nYou become trained in barbarian class DC.\n\nYou can use the Rage action.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BarbarianArchetypeTrait })
            .WithCustomName("Barbarian Dedication")
            .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Strength) >= 14 && values.FinalAbilityScores.TotalScore(Ability.Constitution) >= 14, "You must have at least 14 Strength and Constitution .")
            .WithPrerequisite(values => values.Sheet.Class?.ClassTrait != Trait.Barbarian, "You already have this archetype as a main class.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

    {

      sheet.AdditionalClassTraits.Add(Trait.Barbarian);

      if (sheet.GetProficiency(Trait.Barbarian) == Proficiency.Untrained)
      {
        sheet.SetProficiency(Trait.Barbarian, Proficiency.Trained);
      }


      if (sheet.GetProficiency(Trait.Athletics) == Proficiency.Untrained)
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Barbarian Dedication Skill",
                "Barbarian Dedication skill",
                -1,
                (ft) => ft.FeatName == FeatName.Athletics)

                );
      }
      else
      {
        sheet.AddSelectionOption(
            new SingleFeatSelectionOption(
                "Barbarian Dedication Skill",
                "Barbarian Dedication skill",
                -1,
                (ft) => ft is SkillSelectionFeat)

                );
      }
    }).WithOnCreature((cr =>
  {
    cr.AddQEffect(new QEffect()
    {
      ProvideMainAction = (Func<QEffect, Possibility>)(qfTechnical =>
      {
        Creature owner = qfTechnical.Owner;
        if (owner.HasEffect(QEffectId.Rage))
          return (Possibility)null;
        int num = owner.Level + owner.Abilities.Constitution;
        return (Possibility)new ActionPossibility(new CombatAction(owner, (Illustration)IllustrationName.Rage, "Rage", new Trait[4]
        {
              Trait.Barbarian,
              Trait.Concentrate,
              Trait.Emotion,
              Trait.Mental
        }, "{i}You tap into your inner fury and begin raging.{/i}\n\nYou gain {Blue}" + num.ToString() + "{/Blue} temporary HP.\n\nUntil the end of the encounter or until you fall unconscious, whichever comes first, you're in a rage and:\n\n- You deal +2 additional damage with melee Strike (or +1 with agile weapons).\n- You take a -1 penalty to AC.\n-You can't use Demoralize or other actions that require concentration.\n\nAfter your rage ends, you lose any temporary HP you have left and you can't Rage again this encounter.", (Target)Target.Self().WithAdditionalRestriction((Func<Creature, string>)(self => CanEnterRage(self))))
        {
          ShortDescription = ("You gain {Blue}" + num.ToString() + "{/Blue} temporary HP, a bonus to damage and a penalty to AC until the end of the encounter.")
        }.WithEffectOnSelf((self => EnterRage(self))));
      })
    });
  }));




    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
              4,
              "Your barbarian rage has made your more resilient.",
              "You gain 3 additional Hit Points for each barbarian archetype class feat you have.\n\nAs you continue selecting barbarian archetype class feats, you continue to gain additional Hit Points in this way.",
              new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BarbarianArchetypeTrait })
              .WithCustomName("Barbarian Resiliency")
              .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BarbarianDedicationFeat), "You must have the Barbarian Dedication feat.")
              .WithPrerequisite((CalculatedCharacterSheetValues values) =>

              values.Sheet.Class?.ClassTrait != Trait.Barbarian


              , "You have a class granting more than Hit Points per level than 10 + your Constitution modifier")
              .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) =>
              {

                int ReslientHP = 3 * sheet.AllFeats.Count(x => x.HasTrait(BarbarianArchetypeTrait));
                cr.MaxHP += ReslientHP;

              }));

    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You are able to learn basic furies.",
            "You gain a 1st- or 2nd-level barbarian feat.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, BarbarianArchetypeTrait })
            .WithCustomName("Basic Fury")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(BarbarianDedicationFeat), "You must have the Barbarian Dedication feat.")
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

{

  sheet.AddSelectionOption(
              new SingleFeatSelectionOption(
                  "Basic Fury",
                  "Basic Fury feat",
                  -1,
                  (Feat ft) =>
            {
              if (ft.HasTrait(Trait.Barbarian) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
              {

                if (ft.CustomName == null)
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  else return false;

                }
                else
                {
                  TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

                  if (FeatwithLevel.Level <= 2)
                  {
                    return true;
                  }
                  return false;
                }
              }
              return false;
            })
                  );
})

    );







    ModManager.AddFeat(BarbarianDedicationFeat);
  }
}