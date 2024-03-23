using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Roller;




namespace Dawnsbury.Mods.DawnniExpanded;

public static class ItemMutagenEnergy
{
    public static void LoadMod()
    {

        ModdedIllustration MutagenAcid = new ModdedIllustration("DawnniburyExpandedAssets/EnergyMutagenAcid.png");
        ModdedIllustration MutagenCold = new ModdedIllustration("DawnniburyExpandedAssets/EnergyMutagenCold.png");
        ModdedIllustration MutagenElectricity = new ModdedIllustration("DawnniburyExpandedAssets/EnergyMutagenElectricity.png");
        ModdedIllustration MutagenFire = new ModdedIllustration("DawnniburyExpandedAssets/EnergyMutagenFire.png");

        ItemName EnergyMutagenLesserAcid = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Lesser, Acid)", itemName =>
      new Item(itemName, MutagenAcid, "Energy Mutagen(Lesser, Acid)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
      {
          Description = "When created, this mutagen was attuned to energy type of acid. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (cold, electricity, and fire).\n\n",

          DrinkableEffect = (CombatAction ca, Creature self) =>
          {

              QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Acid)", "You are benefiting from a Energy Mutagen (Acid)", ExpirationCondition.Never, self, MutagenAcid)
              {

                  AddExtraStrikeDamage = (action, target) =>
                  {
                      if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                      {
                          return null;
                      }
                      else return (DiceFormula.FromText("1", "Energy Mutagen"), DamageKind.Acid);
                  },

                  StateCheck = (qf =>
                      {
                          qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Acid, 5);
                          qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                          qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                          qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                      }),

              };

              TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
              self.AddQEffect(EnergyMutagenEffect);

          }
      }
          );
        ItemName EnergyMutagenLesserCold = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Lesser, Cold)", itemName =>
        new Item(itemName, MutagenCold, "Energy Mutagen(Lesser, Cold)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of cold. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, electricity, and fire).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Cold)", "You are benefiting from a Energy Mutagen (Cold)", ExpirationCondition.Never, self, MutagenCold)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1", "Energy Mutagen"), DamageKind.Cold);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );
        ItemName EnergyMutagenLesserElectricity = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Lesser, Electricity)", itemName =>
        new Item(itemName, MutagenElectricity, "Energy Mutagen(Lesser, Electricity)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of electricity. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, cold, and fire).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Electricity)", "You are benefiting from a Energy Mutagen (Electricity)", ExpirationCondition.Never, self, MutagenElectricity)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1", "Energy Mutagen"), DamageKind.Electricity);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );
        ItemName EnergyMutagenLesserFire = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Lesser, Fire)", itemName =>
        new Item(itemName, MutagenFire, "Energy Mutagen(Lesser, Fire)", 1, 4, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of fire. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1 damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, cold, and electricity).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Fire)", "You are benefiting from a Energy Mutagen (Fire)", ExpirationCondition.Never, self, MutagenFire)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1", "Energy Mutagen"), DamageKind.Fire);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );

        ItemName EnergyMutagenModerateAcid = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Moderate, Acid)", itemName =>
        new Item(itemName, MutagenAcid, "Energy Mutagen(Moderate, Acid)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of acid. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1d4 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (cold, electricity, and fire).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Acid)", "You are benefiting from a Energy Mutagen (Acid)", ExpirationCondition.Never, self, MutagenAcid)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1d4", "Energy Mutagen"), DamageKind.Acid);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Acid, 10);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );
        ItemName EnergyMutagenModerateCold = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Moderate, Cold)", itemName =>
        new Item(itemName, MutagenCold, "Energy Mutagen(Moderate, Cold)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of cold. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1d4 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, electricity, and fire).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Cold)", "You are benefiting from a Energy Mutagen (Cold)", ExpirationCondition.Never, self, MutagenCold)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1d4", "Energy Mutagen"), DamageKind.Cold);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Cold, 10);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );
        ItemName EnergyMutagenModerateElectricity = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Moderate, Electricity)", itemName =>
        new Item(itemName, MutagenElectricity, "Energy Mutagen(Moderate, Electricity)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of electricity. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1d4 of damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, cold, and fire).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Electricity)", "You are benefiting from a Energy Mutagen (Electricity)", ExpirationCondition.Never, self, MutagenElectricity)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1d4", "Energy Mutagen"), DamageKind.Electricity);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Electricity, 10);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Fire, 5);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );
        ItemName EnergyMutagenModerateFire = ModManager.RegisterNewItemIntoTheShop("Energy Mutagen(Moderate, Fire)", itemName =>
        new Item(itemName, MutagenFire, "Energy Mutagen(Moderate, Fire)", 3, 12, Trait.Elixir, TraitMutagens.MutagenTrait, TraitMutagens.PolymorphTrait, Trait.Alchemical, DawnniExpanded.DETrait)
        {
            Description = "When created, this mutagen was attuned to energy type of fire. When consumed, the mutagen suffuses your body with energy that spills out of you whenever you attack. \n\n{b}Benefit{/b} You gain resistance to the attuned energy type. Whenever you score a hit with a melee weapon, add 1d4 damage of the attuned energy type.\n\n{b}Drawback{/b} You gain weakness 5 to the other three energy types (acid, cold, and electricity).\n\n",

            DrinkableEffect = (CombatAction ca, Creature self) =>
            {

                QEffect EnergyMutagenEffect = new QEffect("Energy Mutagen (Fire)", "You are benefiting from a Energy Mutagen (Fire)", ExpirationCondition.Never, self, MutagenFire)
                {



                    AddExtraStrikeDamage = (action, target) =>
                    {
                        if (!action.HasTrait(Trait.Melee) || !action.HasTrait(Trait.Weapon) || action.HasTrait(Trait.Unarmed))
                        {
                            return null;
                        }
                        else return (DiceFormula.FromText("1d4", "Energy Mutagen"), DamageKind.Fire);
                    },

                    StateCheck = (qf =>
                        {
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Acid, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Cold, 5);
                            qf.Owner.WeaknessAndResistance.AddWeakness(DamageKind.Electricity, 5);
                            qf.Owner.WeaknessAndResistance.AddResistance(DamageKind.Fire, 10);
                        }),

                };

                TraitMutagens.PreventMutagenDrinking(EnergyMutagenEffect);
                self.AddQEffect(EnergyMutagenEffect);

            }
        }
            );


    }
}