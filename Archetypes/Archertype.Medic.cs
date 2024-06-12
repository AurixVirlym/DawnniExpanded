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

public static class ArchetypeMedic
{

  public static Feat MedicDedicationFeat;
  public static Feat MedicDoctorsVisitation;
  public static void LoadMod()

  {

    MedicDedicationFeat = new TrueFeat(FeatName.CustomFeat,
            2,
            "You've studied countless techniques for providing medical aid, making you a peerless doctor and healer.",
            "You become an expert in Medicine. (Retrain if you perviously had Expert Medicine)\r\n\r\nWhen you succeed with Battle Medicine, the target regains 5 additional HP at DC 20.\r\n\r\nOnce per day, you can use Battle Medicine on a creature that's temporarily immune.",
            new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait })
            .WithCustomName("Medic Dedication")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Medicine) >= Proficiency.Trained, "You must be trained in Medicine.")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(FeatName.BattleMedicine), "You must have the Battle Medicine feat.")
            .WithOnSheet(sheet => sheet.GrantFeat(FeatName.ExpertMedicine));

    Illustration IllustrationDocVisit = new ModdedIllustration("DawnniburyExpandedAssets/DoctorsVisitation.png");

    MedicDoctorsVisitation = new TrueFeat(FeatName.CustomFeat,
            4,
            "You move to provide immediate care to those who need it.",
            "Stride, then use Battle Medicine.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnniExpanded.DETrait, Trait.Flourish, })
            .WithCustomName("Doctor's Visitation{icon:Action}")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(MedicDedicationFeat), "You must have the Battle Medicine feat.")
            .WithPermanentQEffect("Stride, then use Battle Medicine.", (Action<QEffect>)(qf => qf.ProvideMainAction = (Func<QEffect, Possibility>)(qfSelf => (Possibility)new ActionPossibility(new CombatAction(qfSelf.Owner, (Illustration)IllustrationDocVisit, "Doctor's Visitation", new Trait[3]
  {
        Trait.Flourish,
        Trait.Move,
        DawnniExpanded.DETrait,
  }, "Stride, then you can use Battle Medicine.", Target.Self()).WithActionCost(1).WithSoundEffect(SfxName.Footsteps).WithEffectOnSelf((async (CombatAction action, Creature self) =>
  {
    if (!await self.StrideAsync("Choose where to Stride with Doctor's Visitation", allowCancel: true))
    {
      action.RevertRequested = true;
    }
    else
    {
      await FeatBattleMedicine.BattleMedicineAdjacentCreature(self);

    }
  }))))));



    ModManager.AddFeat(MedicDedicationFeat);
    ModManager.AddFeat(MedicDoctorsVisitation);
  }
}