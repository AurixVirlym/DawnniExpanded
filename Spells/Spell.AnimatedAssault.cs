using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.Creatures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Display.Text;
using System;

namespace Dawnsbury.Mods.DawnniExpanded;
public class SpellAnimatedAssualt
{
    public static ModdedIllustration SpellIllustration = new ModdedIllustration("DawnniburyExpandedAssets/AnimatedAssault.png");
    public static SpellId Id;
    public static CombatAction CombatAction(Creature spellcaster, int spellLevel, bool inCombat)
    {

        return Spells.CreateModern(SpellIllustration,
                "Animated Assault",
            new[] { Trait.Evocation, Trait.Arcane, Trait.Occult, DawnniExpanded.DETrait },
                    "You use your mind to manipulate unattended objects in the area, temporarily animating them to attack.",
                    "The objects hover in the air, then hurl themselves at nearby creatures in a chaotic flurry of debris. \n\nThis assault deals " + S.HeightenedVariable(spellLevel, 2) + "d10 bludgeoning damage to each creature in the area.\n\nOn subsequent rounds, the first time each round you Sustain this Spell, it deals " + S.HeightenedVariable((int)Math.Floor(spellLevel / 2.0), 1) + "d10 bludgeoning damage to each creature in the area."
                    + HS.HeightenTextLevels(spellLevel > 2, spellLevel, inCombat, "{b}Heightened (+2){/b} The initial damage increases by 2d10 and the subsequent damage increases by 1d10."),
                    Target.Burst(24, 2),
                        spellLevel,
                        SpellSavingThrow.Basic(Defense.Reflex)
                        ).WithActionCost(2).WithSoundEffect(SfxName.ElementalBlastWood)
                        .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                        {
                            await CommonSpellEffects.DealBasicDamage(spell, caster, target, result, (spellLevel) + "d10", DamageKind.Bludgeoning);


                        }).WithEffectOnChosenTargets(async (CombatAction spell, Creature creature, ChosenTargets chosenTargets) =>
          {
              List<TileQEffect> listOfDependentEffects = new List<TileQEffect>();
              foreach (Tile owner in chosenTargets.ChosenTiles)
              {
                  TileQEffect tileQeffect = new TileQEffect(owner)
                  {
                      Illustration = (Illustration)(new IllustrationName[2]
                    {
                  IllustrationName.Rubble,
                  IllustrationName.Rubble2,
                    }).GetRandom<IllustrationName>(),
                  };
                  listOfDependentEffects.Add(tileQeffect);
                  owner.QEffects.Add(tileQeffect);
              }
              QEffect qeffect = new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
              {
                  Source = creature,
                  CannotExpireThisTurn = true,
                  WhenExpires = qfSelf =>
                  {
                      foreach (TileQEffect tileQeffect in listOfDependentEffects.ToList<TileQEffect>())
                          tileQeffect.Owner.QEffects.Remove(tileQeffect);
                  }

              };

              QEffect qEffect = new QEffect("Sustaining " + spell.Name, "You're sustaining an effect and it will expire if you don't sustain it every turn.", ExpirationCondition.Never, null, IllustrationName.CastASpell)
              {

                  Id = QEffectId.Sustaining,
                  DoNotShowUpOverhead = true,
                  ProvideContextualAction = (QEffect qf) => (!qeffect.CannotExpireThisTurn) ? new ActionPossibility(new CombatAction(qf.Owner, spell.Illustration, "Sustain " + spell.Name, new Trait[3]
                  {
                Trait.Concentrate,
                Trait.SustainASpell,
                Trait.Basic
                  }, "The duration of " + spell.Name + " continues until the end of your next turn.", Target.Self((Creature self, AI ai) => 1.0737418E+09f))
                  .WithEffectOnSelf(async delegate (Creature creature)
                  {
                      qeffect.CannotExpireThisTurn = true;
                      foreach (TileQEffect tileQeffect in listOfDependentEffects.ToList<TileQEffect>())
                      {

                          if (tileQeffect.Owner.PrimaryOccupant != null)
                          {
                              Creature defender = tileQeffect.Owner.PrimaryOccupant;
                              int dmgdice = (int)Math.Floor(spell.SpellLevel / 2.0);
                              CheckResult checkResult = CommonSpellEffects.RollSpellSavingThrow(defender, spell, Defense.Reflex);
                              await CommonSpellEffects.DealBasicDamage(spell, spell.Owner, defender, checkResult, DiceFormula.FromText(dmgdice + "d10", spell.Name), DamageKind.Bludgeoning);
                          }
                      }

                  })) : null,
                  StateCheck = delegate (QEffect qf)
                  {
                      if (qeffect.Owner.Destroyed || !qeffect.Owner.HasEffect(qeffect))
                      {
                          qf.ExpiresAt = ExpirationCondition.Immediately;
                      }
                  }
              };


              creature.AddQEffect(qeffect);
              creature.AddQEffect(qEffect);


          });



    }

    public static void LoadMod()
    {
        Id = ModManager.RegisterNewSpell("AnimatedAssualt", 2, (spellId, spellcaster, spellLevel, inCombat, SpellInformation) => CombatAction(spellcaster, spellLevel, inCombat));

    }

}

