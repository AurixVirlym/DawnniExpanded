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
using Dawnsbury.Auxiliary;

namespace Dawnsbury.Mods.DawnniExpanded
{
    public class FlankingIndictor
    {
        public static ModdedIllustration TEST = new ModdedIllustration("DawnniburyExpandedAssets/TEST.png");

        public static void LoadMod()
        {
            ModManager.RegisterActionOnEachCreature(creature =>{

                creature.AddQEffect(new QEffect("Flanking Test","Flanking Test")
                {
                    ExpiresAt = ExpirationCondition.Never,
                    StateCheck = Qf =>
                    {

                        if (!Qf.Owner.HasEffect(QEffectId.FlankedBy)){
                            return;
                        } ;

                        foreach (QEffect FlankingQEffect in Qf.Owner.QEffects.Where<QEffect>(qf =>
                    qf.Id == QEffectId.FlankedBy)
                        )
                        {
                            Tile rectThisTile = FlankingQEffect.Owner.Occupies;
                            Rectangle rectangle1 = new Rectangle(rectThisTile.X, rectThisTile.Y, rectThisTile.Y+258, rectThisTile.X+258);
                            Primitives.DrawImage(TEST, rectangle1);
                        }
                    }
                });

            });
    }
    }
}