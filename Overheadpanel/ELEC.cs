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
        private static bool apu_gen1 = false, apu_gen2 = false, eng1_gen = false, eng2_gen = false;

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
                    FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS,
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
            fsi.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT = false;
            fsi.MBI_ELEC_STBY_GEN_1_DRIVE_LIGHT = false;
            fsi.MBI_ELEC_STBY_GEN_2_DRIVE_LIGHT = false;

            fsi.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {

            //GROUND POWER
            if (id == FSIID.IOS_GRD_PWR_CONNECTED)
            {
                if (fsi.IOS_GRD_PWR_CONNECTED)
                {
                    debug("ELEC GND PWR Connected");
                } else
                {
                    debug("ELEC GND PWR Disconnected");
                }

                //GND PWD available
                fsi.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT= fsi.IOS_GRD_PWR_CONNECTED;
                simElectrics();
            }

            //battery
            if (id == FSIID.MBI_ELEC_IND_BATTERY_SWITCH)
            {
                if (fsi.MBI_ELEC_IND_BATTERY_SWITCH)
                {
                    debug("ELEC DC Bat On");
                    fsi.SLI_BAT_BUS_VOLTAGE = 24;
                }
                else
                {
                    debug("ELEC DC Bat Off");
                    fsi.SLI_BAT_BUS_VOLTAGE = 0;
                }

                simElectrics();
            }

            //STBY Power
            if (id == FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS || id == FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS)
            {
                if (fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS)
                {
                    debug("ELEC STBY PWR AUTO");
                } else if(fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS)
                {
                    debug("ELEC STBY PWR BAT");
                } else
                {
                    debug("ELEC STBY PWR OFF");
                }

                //take changes
                fsi.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT = !fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS && !fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS;
                fsi.ProcessWrites();
            }

            //stby Gen 1
            if (id == FSIID.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
            {
                if (!fsi.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
                {
                    debug("ELEC STBY Gen 1 Connected");
                }
                else
                {
                    debug("ELEC STBY Gen 1 Disconnected");
                }

                //GND PWD available
                fsi.MBI_ELEC_STBY_GEN_1_DRIVE_LIGHT = !fsi.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH;
                fsi.ProcessWrites();
            }

            //stby Gen 2
            if (id == FSIID.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
            {
                if (!fsi.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
                {
                    debug("ELEC STBY Gen 2 Connected");
                }
                else
                {
                    debug("ELEC STBY Gen 2 Disconnected");
                }

                //GND PWD available
                fsi.MBI_ELEC_STBY_GEN_2_DRIVE_LIGHT = !fsi.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH;
                fsi.ProcessWrites();
            }

            //APU gen 1
            if (id == FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 1 Off");
                    apu_gen1 = false;
                }
                if (fsi.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 1 On");
                    apu_gen1 = true;
                }
                simElectrics();
            }

            //APU GEN 2
            if (id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 2 Off");
                    apu_gen2 = false:
                }
                if (fsi.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 2 On");
                    apu_gen2 = true;
                }
                simElectrics();
            }

            //ENG GEN 1
            if (id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 1 Off");
                    eng1_gen = false;
                }
                if (fsi.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 1 On");
                    eng1_gen = true;
                }
                simElectrics();
            }

            //ENG GEN 2
            if (id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 2 Off");
                    eng2_gen = false;
                }
                if (fsi.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 2 On");
                    eng2_gen = true;
                }
                simElectrics();
            }

            //BUS TRANSFER
            if (id == FSIID.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
            {
                if (!fsi.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
                {
                    debug("ELEC BUS TRANSFER AUTO");
                }
                else
                {
                    debug("ELEC BUS TRANSFER OFF");
                }
                simElectrics();
            }
        }



        //
        private static void simElectrics()
        {

            //only battery on -> battery discharge light
            if (fsi.MBI_ELEC_IND_BATTERY_SWITCH && //barrer switch on
                !(fsi.IOS_GRD_PWR_CONNECTED && fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH)) //gnd pwr off
            {
                fsi.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT = true;
            } else
            {
                fsi.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT = false;
            }

            fsi.ProcessWrites();
        }
    }
}
