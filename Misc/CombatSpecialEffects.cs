using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Core.Animations;
using Dawnsbury.Display.Illustrations;
using Microsoft.Xna.Framework;


namespace Dawnsbury.Mods.DawnniExpanded
{
    public class CombatSpecialEffects
    {

        public static void CreateNoTimeConeAnimation(TBattle battle, Vector2 mainBurstOrigin, List<Tile> targetTiles, int projectileCountPerTile, Illustration projectIllustration, Color color)
        {
            List<Particle> newParticles = new List<Particle>();
            foreach (Tile targetTile in targetTiles)
            {
                for (int i = 0; i < projectileCountPerTile; i++)
                {
                    newParticles.AddRange(battle.SpawnOvercreatureProjectileParticles(1, mainBurstOrigin, targetTile, color, projectIllustration));
                }
            }

            if (newParticles.Any())
            {
                foreach (Particle item in newParticles)
                {

                    item.Projectile_TimeRemaining = 0.5F;
                }


                newParticles.Max((Particle pp) => pp.Projectile_TimeRemaining);
            }


        }



        public static ModdedIllustration HitSpark = new ModdedIllustration("DawnniburyExpandedAssets/Hitspark.png");
        public static ModdedIllustration CritSpark = new ModdedIllustration("DawnniburyExpandedAssets/Critspark.png");

        public static void LoadMod()
        {



            ModManager.RegisterActionOnEachCreature(creature =>
            {
                // We add an effect to every single creature...
                creature.AddQEffect(
                    new QEffect()
                    {


                        AfterYouTakeDamage = (Delegates.AfterYouTakeDamage)(async (QEffect effect, int amount, DamageKind damageKind, CombatAction attack, bool isCritical) =>
                        {

                            int numberofparticales = 1;
                            Color ParticleColour = Color.Red;

                            switch (damageKind)
                            {
                                case DamageKind.Fire:
                                case DamageKind.Bleed:
                                    ParticleColour = Color.Red;
                                    break;

                                case DamageKind.Electricity:
                                case DamageKind.Force:
                                case DamageKind.Cold:
                                    ParticleColour = Color.Blue;
                                    break;

                                case DamageKind.Acid:
                                case DamageKind.Poison:
                                    ParticleColour = Color.Green;
                                    break;

                                case DamageKind.Mental:
                                case DamageKind.Evil:
                                case DamageKind.Negative:
                                    ParticleColour = Color.Purple;
                                    break;

                                case DamageKind.Good:
                                case DamageKind.Positive:
                                    ParticleColour = Color.Gold;
                                    break;

                                default:
                                    break;
                            }

                            Creature target1 = effect.Owner;

                            if (isCritical == true || amount >= target1.MaxHP / 1.5)
                            {
                                ++numberofparticales;
                            }

                            if (isCritical == true)
                            {
                                ParticleColour = Color.Gold;
                            }

                            List<Tile> sphereEffect = new List<Tile>();

                            foreach (Edge item in target1.Occupies.Neighbours.ToList())
                            {
                                sphereEffect.Add(item.Tile);
                            }

                            CreateNoTimeConeAnimation(target1.Battle, target1.Occupies.ToCenterVector(), sphereEffect, numberofparticales, HitSpark, ParticleColour);

                        })
                    });

            });


        }
    }
}