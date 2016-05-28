using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

namespace Overheadpanel
{
    class OXY : Panel
    {
        private static FSIClient fsi;

        public OXY()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead OXY");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_OXYGEN_PASS_OXY_SWITCH
                }
            );

            //standard values
            fsi.MBI_OXYGEN_PASS_OXY_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_OXYGEN_PASS_OXY_SWITCH && fsi.MBI_OXYGEN_PASS_OXY_SWITCH == false)
            {
                debug("OXY Pass ON");

                //ELT light
                fsi.MBI_OXYGEN_PASS_OXY_LIGHT = true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_OXYGEN_PASS_OXY_SWITCH && fsi.MBI_OXYGEN_PASS_OXY_SWITCH == true)
            {
                debug("OXY Pass NORM");

                fsi.MBI_OXYGEN_PASS_OXY_LIGHT = false;
                fsi.ProcessWrites();
            }
        }
    }
}
