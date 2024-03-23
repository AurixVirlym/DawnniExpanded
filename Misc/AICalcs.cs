using System;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;


namespace Dawnsbury.Mods.DawnniExpanded
{
    public class AICalcs
    {
        public static float AttackBonusGoodnessForNPCs(int CreatureLevel, int ExtraToHit, int ExtraDamage, bool FullEncounterDuration = false, int durationInRounds = 1)
        {
            int expectedDamageByLevel = Math.Min(3 + (2 * (1 + CreatureLevel)), 1);
            float impactOnExpected = ((expectedDamageByLevel + ExtraDamage) * (float)((ExtraToHit * 0.1f) + 1f)) - expectedDamageByLevel;
            if (FullEncounterDuration == true)
            {
                durationInRounds = 4;
            }

            impactOnExpected *= (float)Math.Min(durationInRounds, 4);
            return impactOnExpected;
        }


        public static float Fear(Creature target)
        {
            if (target.HasEffect(QEffectId.Fleeing) || target.HasEffect(QEffectId.Frightened))
            {
                return 0f;
            }

            return (float)(target.Level + 2f) * 2f;
        }
    }
}