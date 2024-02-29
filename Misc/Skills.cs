using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.DawnniExpanded
{

    public class NewSkills
    {
        public static Feat Performance = new SkillSelectionFeat(FeatName.CustomFeat, Skill.Performance, Trait.Performance).WithCustomName("Performance");
        public static Feat Crafting = new SkillSelectionFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting).WithCustomName("Crafting");
        public static Feat Survival = new SkillSelectionFeat(FeatName.CustomFeat, Skill.Survival, Trait.Survival).WithCustomName("Survival");
        public static Feat ExpertPerformance = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Performance, Trait.Performance).WithCustomName("Expert in Performance");
        public static Feat ExpertCrafting = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting).WithCustomName("Expert in Crafting");
        public static Feat ExpertSurvival = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Survival, Trait.Survival).WithCustomName("Expert in Survival");


        public static void LoadMod()
        {
            ModManager.AddFeat(Performance);
            ModManager.AddFeat(Crafting);
            ModManager.AddFeat(Survival);
            ModManager.AddFeat(ExpertPerformance);
            ModManager.AddFeat(ExpertCrafting);
            ModManager.AddFeat(ExpertSurvival);
        }
    }
}