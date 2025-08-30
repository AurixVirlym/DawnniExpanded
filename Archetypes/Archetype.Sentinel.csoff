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



namespace Dawnsbury.Mods.DawnniExpanded;

public static class ArchetypeSentinel
{

  public static Feat SentinelDedicationFeat;
  public static void LoadMod()

  {

    SentinelDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You have trained carefully to maximize the protective qualities of your armor. ",
            "You become trained in light armor and medium armor.\r\n\r\nIf you already were trained in light armor and medium armor, you gain training in heavy armor as well.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Sentinel Dedication")
            .WithOnSheet(sheet =>
            {

              if (sheet.GetProficiency(Trait.LightArmor) == Proficiency.Trained
                  && sheet.GetProficiency(Trait.MediumArmor) == Proficiency.Trained
                  && sheet.GetProficiency(Trait.HeavyArmor) == Proficiency.Untrained)
              {
                sheet.SetProficiency(Trait.HeavyArmor, Proficiency.Trained);
              }

              if (sheet.GetProficiency(Trait.LightArmor) == Proficiency.Untrained)
              {
                sheet.SetProficiency(Trait.LightArmor, Proficiency.Trained);
              }

              if (sheet.GetProficiency(Trait.MediumArmor) == Proficiency.Untrained)
              {
                sheet.SetProficiency(Trait.MediumArmor, Proficiency.Trained);
              }

            });

    ModManager.AddFeat(SentinelDedicationFeat);
  }
}