using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

namespace Overheadpanel
{
    class ANTIICE : Panel
    {
        private static FSIClient fsi;

        public ANTIICE()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead ANTIICE");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH,
                    FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH,
                    FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH
                }
            );

            //standard values
            fsi.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT = false;
            fsi.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT = false;
            fsi.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT = false;
            fsi.MBI_ANTI_ICE_ENG_2_COWL_ANTI_ICE_LIGHT = false;
            fsi.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT = false;
            fsi.MBI_ANTI_ICE_ENG_1_COWL_ANTI_ICE_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {

            //WING
            if (id == FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE Wing On");

                fsi.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT = true;
                fsi.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT = true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_WING_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE Wing Off");

                fsi.MBI_ANTI_ICE_WING_L_VALVE_OPEN_LIGHT = false;
                fsi.MBI_ANTI_ICE_WING_R_VALVE_OPEN_LIGHT = false;
                fsi.ProcessWrites();
            }


            //ENG 1
            if (id == FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE ENG 1 On");

                fsi.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT = true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_ENG_1_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE ENG 1 Off");

                fsi.MBI_ANTI_ICE_ENG_1_COWL_VALVE_OPEN_LIGHT = false;
                fsi.ProcessWrites();
            }


            //ENG 2
            if (id == FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH == true)
            {
                debug("ANTI_ICE ENG 2 On");

                fsi.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT = true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH && fsi.MBI_ANTI_ICE_ENG_2_ANTI_ICE_SWITCH == false)
            {
                debug("ANTI_ICE ENG 2 Off");

                fsi.MBI_ANTI_ICE_ENG_2_COWL_VALVE_OPEN_LIGHT = false;
                fsi.ProcessWrites();
            }
        }
        
    }
}
