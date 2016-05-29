using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

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
                    FSIID.SLI_APU_GEN_RTL,
                    FSIID.SLI_GEN_1_RTL,
                    FSIID.SLI_GEN_2_RTL,

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
            LightController.set(FSIID.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_STBY_GEN_1_DRIVE_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_STBY_GEN_2_DRIVE_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, true);
            LightController.set(FSIID.MBI_ELEC_BUS_APU_GEN_OFF_BUS_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_GEN_OFF_BUS_LIGHT, true);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_GEN_OFF_BUS_LIGHT, true);
            LightController.set(FSIID.MBI_ELEC_IND_ELEC_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_IND_TR_UNIT_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_SOURCE_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_TRANSFER_BUS_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_TRANSFER_BUS_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_SOURCE_OFF_LIGHT, false);

            fsi.ProcessWrites();
            LightController.ProcessWrites();
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
                LightController.set(FSIID.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT, fsi.IOS_GRD_PWR_CONNECTED);
                simElectrics();
            }

            //GROUND POWER SWITCH
            if (id == FSIID.MBI_ELEC_BUS_GRD_PWR_SWITCH)
            {
                if (fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH)
                {
                    debug("ELEC GND PWR SWITCH On");
                }
                else
                {
                    debug("ELEC GND PWR SWITCH Off");
                }

                simElectrics();
            }

            //battery
            if (id == FSIID.MBI_ELEC_IND_BATTERY_SWITCH)
            {
                if (fsi.MBI_ELEC_IND_BATTERY_SWITCH)
                {
                    debug("ELEC DC Bat On");
                }
                else
                {
                    debug("ELEC DC Bat Off");
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
                LightController.set(FSIID.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT, !fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS && !fsi.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS);
                LightController.ProcessWrites();
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
                LightController.set(FSIID.MBI_ELEC_STBY_GEN_1_DRIVE_LIGHT, !fsi.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH);
                LightController.ProcessWrites();
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
                LightController.set(FSIID.MBI_ELEC_STBY_GEN_2_DRIVE_LIGHT, !fsi.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH);
                LightController.ProcessWrites();
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
                    apu_gen2 = false;
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


            //some changes in generator availability
            if (id == FSIID.SLI_APU_GEN_RTL || id == FSIID.SLI_GEN_1_RTL || id == FSIID.SLI_GEN_2_RTL)
            {
                simElectrics();
            }
        }



        //
        private static void simElectrics()
        {

            //battery discharge light
            if (fsi.MBI_ELEC_IND_BATTERY_SWITCH && //battery switch on
                !(fsi.IOS_GRD_PWR_CONNECTED && fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH) && //gnd pwr off
                !(fsi.SLI_APU_GEN_RTL && (apu_gen1 || apu_gen2)) && //APU GEN off
                !(fsi.SLI_GEN_1_RTL && eng1_gen) && //ENG 1 Pwr off
                !(fsi.SLI_GEN_2_RTL && eng2_gen)) //ENG 2 Pwf off
            {
                LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, true);
            } else
            {
                LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, false);
            }

            //apu gen light on -> no gnd power, no eng power, no apu power
            if (fsi.SLI_APU_GEN_RTL && !apu_gen1 && !apu_gen2 && //APU Gen available and gens off
                !(fsi.IOS_GRD_PWR_CONNECTED && fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH) && //GND PWR off
                !(fsi.SLI_GEN_1_RTL && eng1_gen) && //ENG 1 Pwr off
                !(fsi.SLI_GEN_2_RTL && eng2_gen)) //eng 2 pwr off
            {
                LightController.set(FSIID.MBI_ELEC_BUS_APU_GEN_OFF_BUS_LIGHT, true);
            } else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_APU_GEN_OFF_BUS_LIGHT, false);
            }

            //engine 1 GEN BUS OFF light
            if (eng1_gen && fsi.SLI_GEN_1_RTL)
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_GEN_OFF_BUS_LIGHT, false);
            } else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_GEN_OFF_BUS_LIGHT, true);
            }

            //engine 2 GEN BUS OFF light
            if (eng2_gen && fsi.SLI_GEN_2_RTL)
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_GEN_OFF_BUS_LIGHT, false);
            }
            else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_GEN_OFF_BUS_LIGHT, true);
            }

            //AC Systems Power (heavy load systems e.g. displays)
            if ((fsi.IOS_GRD_PWR_CONNECTED && fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH) || //GND PWR ON
                (fsi.SLI_APU_GEN_RTL && (apu_gen1 || apu_gen2)) || //APU Gen On
                (fsi.SLI_GEN_1_RTL && eng1_gen) ||  //ENG 1 GEN ON
                (fsi.SLI_GEN_2_RTL && eng2_gen))    //ENG 2 GEN ON
            {
                fsi.SLI_AC_STBY_BUS_PHASE_1_VOLTAGE = 110;

                //displays on
                switchACSystems(true);
            } else
            {
                fsi.SLI_AC_STBY_BUS_PHASE_1_VOLTAGE = 0;

                //displays off
                switchACSystems(false);
            }


            //DC Systems Power (Most important systems e.g. warning leds)
            if (fsi.MBI_ELEC_IND_BATTERY_SWITCH || //battery
                (fsi.IOS_GRD_PWR_CONNECTED && fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH) || //GND PWR ON
                (fsi.SLI_APU_GEN_RTL && (apu_gen1 || apu_gen2)) || //APU Gen On
                (fsi.SLI_GEN_1_RTL && eng1_gen) ||  //ENG 1 GEN ON
                (fsi.SLI_GEN_2_RTL && eng2_gen))    //ENG 2 GEN ON
            {
                fsi.SLI_BAT_BUS_VOLTAGE = 24;
                switchDCSystems(true);
            } else
            {
                fsi.SLI_BAT_BUS_VOLTAGE = 0;
                switchDCSystems(false);
            }

            
            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        //switch AC Systems on / Off
        private static void switchACSystems(bool power)
        {
            fsi.INT_POWER_EICAS = power;
            fsi.INT_POWER_ISFD = power;
            fsi.INT_POWER_LDU = power;
            fsi.INT_POWER_ND_CPT = power;
            fsi.INT_POWER_ND_FO = power;
            fsi.INT_POWER_PFD_CPT = power;
            fsi.INT_POWER_PFD_FO = power;
            fsi.INT_POWER_SRMI = power;
        }

        private static void switchDCSystems(bool power)
        {
            fsi.CPF_MCP_POWER = power;

            //all status lights
            if (power)
            {
                LightController.setLightStatus(1);
            } else
            {
                LightController.setLightStatus(0);
            }
        }
    }
}
