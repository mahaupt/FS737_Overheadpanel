using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

namespace Overheadpanel
{
    class HEAT : Panel
    {
        private static FSIClient fsi;

        public HEAT()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead HEAT");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_HEAT_OVHT_TEST_SWITCH,
                    FSIID.MBI_HEAT_PROBE_HEAT_A_SWITCH,
                    FSIID.MBI_HEAT_PROBE_HEAT_B_SWITCH,
                    FSIID.MBI_HEAT_PWR_TEST_SWITCH,
                    FSIID.MBI_HEAT_WINDOW_LEFT_FWD_SWITCH,
                    FSIID.MBI_HEAT_WINDOW_LEFT_SIDE_SWITCH,
                    FSIID.MBI_HEAT_WINDOW_RIGHT_FWD_SWITCH,
                    FSIID.MBI_HEAT_WINDOW_RIGHT_SIDE_SWITCH
                }
            );

            //standard values
            fsi.MBI_HEAT_AUX_PITOT_LIGHT = false;
            fsi.MBI_HEAT_CAPT_PITOT_LIGHT = false;
            fsi.MBI_HEAT_FO_PITOT_LIGHT = false;
            fsi.MBI_HEAT_L_ALPHA_VANE_LIGHT = false;
            fsi.MBI_HEAT_L_ELEV_PITOT_LIGHT = false;
            fsi.MBI_HEAT_R_ALPHA_VANE_LIGHT = false;
            fsi.MBI_HEAT_R_ELEV_PITOT_LIGHT = false;
            fsi.MBI_HEAT_TEMP_PROBE_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_LEFT_FWD_ON_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_LEFT_FWD_OVERHEAT_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_LEFT_SIDE_ON_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_LEFT_SIDE_OVERHEAT_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_RIGHT_FWD_ON_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_RIGHT_FWD_OVERHEAT_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_ON_LIGHT = false;
            fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_OVERHEAT_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            //OVERHEAT TEST ON
            if (id == FSIID.MBI_HEAT_OVHT_TEST_SWITCH && fsi.MBI_HEAT_OVHT_TEST_SWITCH == true)
            {
                debug("HEAT OVHT TEST On");

                //OVHT tst lights
                fsi.MBI_HEAT_WINDOW_LEFT_FWD_OVERHEAT_LIGHT = true;
                fsi.MBI_HEAT_WINDOW_LEFT_SIDE_OVERHEAT_LIGHT = true;
                fsi.MBI_HEAT_WINDOW_RIGHT_FWD_OVERHEAT_LIGHT = true;
                fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_OVERHEAT_LIGHT = true;
                fsi.ProcessWrites();
            }

            //OVERHEAT TEST OFF
            if (id == FSIID.MBI_HEAT_OVHT_TEST_SWITCH && fsi.MBI_HEAT_OVHT_TEST_SWITCH == false)
            {
                debug("HEAT OVHT TEST Off");

                fsi.MBI_HEAT_WINDOW_LEFT_FWD_OVERHEAT_LIGHT = false;
                fsi.MBI_HEAT_WINDOW_LEFT_SIDE_OVERHEAT_LIGHT = false;
                fsi.MBI_HEAT_WINDOW_RIGHT_FWD_OVERHEAT_LIGHT = false;
                fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_OVERHEAT_LIGHT = false;
                fsi.ProcessWrites();
            }

            //WND LEFT FWD
            if (id == FSIID.MBI_HEAT_WINDOW_LEFT_FWD_SWITCH)
            {
                if (fsi.MBI_HEAT_WINDOW_LEFT_FWD_SWITCH)
                {
                    debug("HEAT WND L FWD On");
                }
                else
                {
                    debug("HEAT WND L FWD Off");
                }

                //set lights
                fsi.MBI_HEAT_WINDOW_LEFT_FWD_ON_LIGHT = fsi.MBI_HEAT_WINDOW_LEFT_FWD_SWITCH;
                fsi.ProcessWrites();
            }


            //WND LEFT SIDE
            if (id == FSIID.MBI_HEAT_WINDOW_LEFT_SIDE_SWITCH)
            {
                if (fsi.MBI_HEAT_WINDOW_LEFT_SIDE_SWITCH)
                {
                    debug("HEAT WND L SIDE On");
                }
                else
                {
                    debug("HEAT WND L SIDE Off");
                }

                //set lights
                fsi.MBI_HEAT_WINDOW_LEFT_SIDE_ON_LIGHT = fsi.MBI_HEAT_WINDOW_LEFT_SIDE_SWITCH;
                fsi.ProcessWrites();
            }

            //WND RIGHT FWD
            if (id == FSIID.MBI_HEAT_WINDOW_RIGHT_FWD_SWITCH)
            {
                if (fsi.MBI_HEAT_WINDOW_RIGHT_FWD_SWITCH)
                {
                    debug("HEAT WND R FWD On");
                }
                else
                {
                    debug("HEAT WND R FWD Off");
                }

                //set lights
                fsi.MBI_HEAT_WINDOW_RIGHT_FWD_ON_LIGHT = fsi.MBI_HEAT_WINDOW_RIGHT_FWD_SWITCH;
                fsi.ProcessWrites();
            }


            //WND RIGHT SIDE
            if (id == FSIID.MBI_HEAT_WINDOW_RIGHT_SIDE_SWITCH)
            {
                if (fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_SWITCH)
                {
                    debug("HEAT WND R SIDE On");
                }
                else
                {
                    debug("HEAT WND R SIDE Off");
                }

                //set lights
                fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_ON_LIGHT = fsi.MBI_HEAT_WINDOW_RIGHT_SIDE_SWITCH;
                fsi.ProcessWrites();
                
            }
        }
    }
}
