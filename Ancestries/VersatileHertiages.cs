using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.CharacterBuilding;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using static Dawnsbury.Delegates;

using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;



namespace Dawnsbury.Mods.DawnniExpanded.Ancestries
{
    public class VersatileHertiages
    {


        public static Feat MakeVHfeat(string AncestryName)
        {
            Feat VersatileHeritageSubFeat = new HeritageSelectionFeat(FeatName.CustomFeat,
                       "Your hertiage is different, stranger, more complex and importantedly, more supernatural than others of your ancestries",
                       "You gain a Versatile Heritage.")
                   .WithCustomName("Versatile Heritage " + AncestryName)
                   .WithOnSheet((sheet =>
        {
            sheet.AddSelectionOption(
           new SingleFeatSelectionOption(
               "Versatile Heritage Selection" + AncestryName,
               "Versatile Heritage Selection " + AncestryName,
               -1,
               (ft) => ft is VersatileHeritageSelectionFeat)

               );


        }));
            VersatileHeritageSubFeat.Traits.Add(DawnniExpanded.DETrait);
            return VersatileHeritageSubFeat;
        }
        public static void LoadMod()

        {

            foreach (Feat AncestryFeat in AllFeats.All.Where(item => item is AncestrySelectionFeat))
            {
                AncestryFeat.Subfeats.Add(MakeVHfeat(AncestryFeat.FeatName.ToString()));
            }

            VersatileHertiageSuli.LoadMod();


        }
    }

    public class VersatileHeritageSelectionFeat : Feat
    {
        public Trait HertiageTrait { get; }

        public VersatileHeritageSelectionFeat(string featName, string flavorText, string description, List<Trait> traits, Trait HertiageTrait)
            : base(FeatName.CustomFeat, flavorText, description, traits, null)
        {
            WithCustomName(featName);
        }
    }


}

