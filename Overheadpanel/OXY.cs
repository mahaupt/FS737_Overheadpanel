using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

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
            LightController.set(FSIID.MBI_OXYGEN_PASS_OXY_LIGHT, false);

            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            if (id == FSIID.MBI_OXYGEN_PASS_OXY_SWITCH)
            {
                if (fsi.MBI_OXYGEN_PASS_OXY_SWITCH == false)
                {
                    debug("OXY Pass ON");
                }
                else
                {
                    debug("OXY Pass NORM");
                }
                
                //ELT light
                LightController.set(FSIID.MBI_OXYGEN_PASS_OXY_LIGHT, !fsi.MBI_OXYGEN_PASS_OXY_SWITCH);
                LightController.ProcessWrites();
            }
        }
    }
}
