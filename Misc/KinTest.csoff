using System;
using System.Collections.Generic;
using System.Linq;

using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Kineticist;

namespace Dawnsbury.Mods.DawnniExpanded
{
    public class KinTest
    {

        public static CombatAction CreateBasicImpulse(
      string featName,
      IllustrationName illustration,
      Trait[] traits,
      int baseLevel,
      string flavorText,
      string rulesText,
      Target target)
        {
            CombatAction basicImpulse = new CombatAction((Creature)null, (Illustration)illustration, featName, ((IEnumerable<Trait>)traits).Concat<Trait>((IEnumerable<Trait>)new Trait[5]
            {
        Trait.Kineticist,
        Trait.Impulse,
        Trait.Primal,
        Trait.Basic,
        Trait.Concentrate
            }).ToArray<Trait>(), "{i}" + flavorText + "{/i}\n\n" + rulesText, target)
            {
                ImpulseInformation = new ImpulseInformation
            (
                FeatName.CustomFeat,
                baseLevel,
                flavorText,
                rulesText
            )
            };
            basicImpulse.WithProjectileCone((Illustration)illustration, 15, ProjectileKind.Cone);
            basicImpulse.WithActionCost(((IEnumerable<Trait>)traits).Contains<Trait>(Trait.Stance) ? 1 : 2);
            return basicImpulse;
        }

        public static void LoadMod()
        {
            ModManager.AddFeat(KineticistImpulses.ToFeat(CreateBasicImpulse("Oceans Balm Example", IllustrationName.OceansBalm, new Trait[4]
          {
            Trait.Water,
            Trait.Healing,
            Trait.Manipulate,
            Trait.Positive
          }, 1, "A blessing of the living sea salves wounds and douses flames.", "You or target adjacent ally regains 1d8 Hit Points and gains resistance 2 to fire for the rest of the encounter. If it has persistent fire damage, it immediately attempts a flat check against DC 10 to remove it {i}(55% success chance){/i}. The target is then temporarily immune to Ocean's Balm for the rest of the encounter.\n\n{b}Level 3:{/b} The healing increases by 1d8, and the resistance increases by 1.", (Target)Target.AdjacentFriendOrSelf()).WithActionId(ActionId.OceansBalm).WithActionCost(1).WithSoundEffect(SfxName.OceansBalm).WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
          {
              target.Heal(caster.MaximumSpellRank.ToString() + "d8", spell);
              target.AddQEffect(QEffect.DamageResistance(DamageKind.Fire, caster.MaximumSpellRank + 1).WithExpirationNever());
              target.AddQEffect(QEffect.ImmunityToTargeting(ActionId.OceansBalm));
              target.QEffects.FirstOrDefault<QEffect>((Func<QEffect, bool>)(qf => qf.Id == QEffectId.PersistentDamage && qf.Key.Substring("PersistentDamage:".Length) == "Fire"))?.RollPersistentDamageRecoveryCheck(true);
          }))));
        }
    }
}