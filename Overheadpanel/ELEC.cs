using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;

namespace Overheadpanel
{
    class ELEC : Panel
    {
        private static FSIClient fsi;

        public ELEC()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead ELEC");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.IOS_GRD_PWR_CONNECTED, 

                    FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS,
                    FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS,
                    FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS,
                    FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS,
                    FSIID.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH,
                    FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS,
                    FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS,
                    FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS,
                    FSIID.MBI_ELEC_BUS_GRD_PWR_SWITCH,
                    FSIID.MBI_ELEC_BUS_L_WIPER_SWITCH_HIGH_POS,
                    FSIID.MBI_ELEC_BUS_L_WIPER_SWITCH_INT_POS,
                    FSIID.MBI_ELEC_BUS_L_WIPER_SWITCH_LOW_POS,
                    FSIID.MBI_ELEC_BUS_L_WIPER_SWITCH_PARK_POS,

                    FSIID.MBI_ELEC_IND_AC_SWITCH_APU_GEN_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_GEN_1_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_GEN_2_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_GRD_PWR_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_INV_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_STBY_PWR_POS,
                    FSIID.MBI_ELEC_IND_AC_SWITCH_TEST_POS,
                    FSIID.MBI_ELEC_IND_BATTERY_SWITCH,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_AUX_BAT_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_BAT_BUS_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_BAT_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_STBY_PWR_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_TEST_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_TR1_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_TR2_POS,
                    FSIID.MBI_ELEC_IND_DC_SWITCH_TR3_POS,
                    FSIID.MBI_ELEC_IND_GALLEY_POWER_SWITCH,
                    FSIID.MBI_ELEC_IND_IFE_SWITCH,
                    FSIID.MBI_ELEC_IND_MAINT_SWITCH,

                    FSIID.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH,
                    FSIID.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH,
                    FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS,
                    FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS
                }
            );

            //standard values
            fsi.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {

            //GROUND POWER
            if (id == FSIID.IOS_GRD_PWR_CONNECTED && fsi.IOS_GRD_PWR_CONNECTED == true)
            {
                debug("ELEC GND PWR Connected");

                //GND PWD available
                fsi.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT= true;
                fsi.ProcessWrites();
            }
            if (id == FSIID.IOS_GRD_PWR_CONNECTED && fsi.IOS_GRD_PWR_CONNECTED == false)
            {
                debug("ELEC GND PWR Disconnected");

                fsi.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT = false;
                fsi.ProcessWrites();
            }
        }
    }
}
