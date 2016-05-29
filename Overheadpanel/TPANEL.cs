using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class TPANEL : Panel
    {
        private static FSIClient fsi;

        public TPANEL()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead TPANEL");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_OFF_POS,
                    FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_ON_POS,
                }
            );

            //standard values
            //LightController.set(FSIID.MBI_UPPER, false);


            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {

        }
    }
}
