using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.DawnniExpanded
{
    public class HS
    {
        public static string HeightenTextLevels(bool isHeightened, int Level, bool inCombat, string heightenedEffect)
        {
            if (isHeightened)
            {
                return "\n\nHeightened to spell level " + Level + ".";
            }

            if (inCombat)
            {
                return "";
            }

            return "\n\n" + heightenedEffect;
        }
    }
}