using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;

using Dawnsbury.Core.Mechanics.Enumerations;

using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CombatActions;

using Dawnsbury.Core.Possibilities;


using Dawnsbury.Core.Creatures;

using Dawnsbury.Core.Mechanics.Targeting.Targets;

using Dawnsbury.Core.Tiles;

using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Display.Illustrations;



namespace Dawnsbury.Mods.DawnniExpanded;

public static class FeatPowerfulLeap
{

    public static void LoadMod()
    {
    
    ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat, 2,
                "You can leap even greater distances.",
                "When you Leap, you increase the distance you can jump horizontally by 5 feet.",
                new[] { Trait.General, Trait.Skill,DawnniExpanded.DETrait}
                )
            .WithCustomName("Powerful Leap")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.GetProficiency(Trait.Athletics) >= Proficiency.Expert, "You must be Expert in Athletics.")
            .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature creature) =>
            {
                QEffect PowerfulLeap = new QEffect("Powerful Leap", "You can leap horizontally by additional 5 feet", ExpirationCondition.Never, null, IllustrationName.None)
                {
                    
                    Innate = true
                };
                creature.AddQEffect(PowerfulLeap);
            }));
    }
}