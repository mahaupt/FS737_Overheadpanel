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
        private static bool bustransfer_auto = true;
        private static AC_Powersource eng1_gen = new AC_Powersource();
        private static AC_Powersource eng2_gen = new AC_Powersource();
        private static AC_Powersource ext_pwr = new AC_Powersource();
        private static AC_Powersource apu_gen1 = new AC_Powersource();
        private static AC_Powersource apu_gen2 = new AC_Powersource();
        private static AC_Powersource disconnected = new AC_Powersource(); // unpowered dummy to have some value available for the AC_BUS class
        private static IDG idg1 = new IDG(eng1_gen);
        private static IDG idg2 = new IDG(eng2_gen);
        private static AC_BUS ac_bus1 = new AC_BUS(disconnected);
        private static AC_BUS ac_bus2 = new AC_BUS(disconnected);
        

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

            //GND PWR. APU, ENG GEN testweise alle auf ON
            fsi.SLI_APU_GEN_RTL = true;
            fsi.IOS_GRD_PWR_CONNECTED = true;
            fsi.SLI_GEN_1_RTL = true;
            fsi.SLI_GEN_2_RTL = true;

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

            fsi.MBI_ELEC_BUS_LAMPTEST = false;
            fsi.MBI_ELEC_IND_LAMPTEST = false;
            fsi.MBI_ELEC_STBY_LAMPTEST = false;

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
                    ext_pwr.Available();
                } else
                {
                    debug("ELEC GND PWR Disconnected");
                    ext_pwr.Unavailable();
                }

                //GND PWR available
                LightController.set(FSIID.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT, ext_pwr.isAvailable);
                simElectrics();
            }

            //GROUND POWER SWITCH
            if (id == FSIID.MBI_ELEC_BUS_GRD_PWR_SWITCH)
            {
                if (fsi.MBI_ELEC_BUS_GRD_PWR_SWITCH)
                {
                    debug("ELEC GND PWR SWITCH On");
                    ext_pwr.SwitchOn();
                    ac_bus1.connect(ext_pwr);
                    ac_bus2.connect(ext_pwr);                    
                }
                else
                {
                    debug("ELEC GND PWR SWITCH Off");
                    ext_pwr.SwitchOff();
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
                simElectrics();
            }

            //IDG_1
            if (id == FSIID.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
            {
                if (!fsi.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
                {
                    debug("ELEC STBY Gen 1 Connected");
                }
                else
                {
                    debug("ELEC STBY Gen 1 Disconnected");
                    idg2.Disconnect();
                }
                simElectrics();
            }

            //IDG_2
            if (id == FSIID.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
            {
                if (!fsi.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
                {
                    debug("ELEC STBY Gen 2 Connected");                    
                }
                else
                {
                    debug("ELEC STBY Gen 2 Disconnected");
                    idg2.Disconnect();
                }
                simElectrics();
            }

            //APU GEN 1
            if (id == FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 1 Off");
                    apu_gen1.SwitchOff();
                }
                if (fsi.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 1 On");
                    if(apu_gen1.SwitchOn()) ac_bus1.connect(apu_gen1);
                }
                simElectrics();
            }

            //APU GEN 2
            if (id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 2 Off");
                    apu_gen2.SwitchOff();
                }
                if (fsi.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 2 On");
                    if(apu_gen2.SwitchOn()) ac_bus2.connect(apu_gen2);
                }
                simElectrics();
            }

            //ENG 1 GEN
            if (id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 1 Off");
                    eng1_gen.SwitchOff();
                }
                if (fsi.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 1 On");
                    if(eng1_gen.SwitchOn()) ac_bus1.connect(eng1_gen);
                }
                simElectrics();
            }

            //ENG 2 GEN
            if (id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
            {
                if (fsi.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 2 Off");
                    eng2_gen.SwitchOff();
                }
                if (fsi.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 2 On");
                    if (eng2_gen.SwitchOn()) ac_bus2.connect(eng2_gen);
                }
                simElectrics();
            }

            //BUS TRANSFER
            if (id == FSIID.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
            {
                if (!fsi.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
                {
                    debug("ELEC BUS TRANSFER AUTO");
                    bustransfer_auto = true;
                }
                else
                {
                    debug("ELEC BUS TRANSFER OFF");
                    bustransfer_auto = false;
                }                
                simElectrics();
            }


            //some changes in generator availability
            if (id == FSIID.SLI_APU_GEN_RTL)
            {
                if (fsi.SLI_APU_GEN_RTL)
                {
                    apu_gen1.Available();
                    apu_gen2.Available();
                }
                else
                {
                    apu_gen1.Unavailable();
                    apu_gen2.Unavailable();
                }
                simElectrics();
            }

            if (id == FSIID.SLI_GEN_1_RTL)
            {
                if (fsi.SLI_GEN_1_RTL && idg1.isConnected) {
                    eng1_gen.Available();
                    debug("ELEC GEN 1 ready to load");
                } else {
                    eng1_gen.Unavailable();
                    debug("ELEC GEN 1 not ready to load");
                }
                simElectrics();
            }
            
            if (id == FSIID.SLI_GEN_2_RTL)
            {
                if (fsi.SLI_GEN_2_RTL && idg2.isConnected){
                    eng2_gen.Available();
                    debug("ELEC GEN 2 ready to load");
                } else {
                    eng2_gen.Unavailable();
                    debug("ELEC GEN 2 not ready to load");
                }
                simElectrics();
            }
        }



        //
        private static void simElectrics()
        {
            //disconnect powersources if necessary
            if (!ac_bus1.powersource.isOnline)
                ac_bus1.disconnect();

            if (!ac_bus2.powersource.isOnline)
                ac_bus2.disconnect();

            //auto-transfer
            if (bustransfer_auto)
            {
                if(!ac_bus1.isPowered)
                {
                    ac_bus1.connect(ac_bus2.powersource);
                    if (ac_bus1.powersource == ext_pwr) ac_bus1.sourceOff = false;
                    else if (ac_bus1.powersource == eng1_gen) ac_bus1.sourceOff = false;
                    else if (ac_bus1.powersource == apu_gen1) ac_bus1.sourceOff = false;
                    else if (ac_bus1.powersource == eng2_gen) ac_bus1.sourceOff = true;
                    else if (ac_bus1.powersource == apu_gen2) ac_bus1.sourceOff = true;
                    else ac_bus1.sourceOff = true;

                }
                if(!ac_bus2.isPowered)
                {
                    ac_bus2.connect(ac_bus1.powersource);
                    if (ac_bus2.powersource == ext_pwr) ac_bus1.sourceOff = false;
                    else if (ac_bus2.powersource == eng2_gen) ac_bus1.sourceOff = false;
                    else if (ac_bus2.powersource == apu_gen2) ac_bus1.sourceOff = false;
                    else if (ac_bus2.powersource == eng1_gen) ac_bus1.sourceOff = true;
                    else if (ac_bus2.powersource == apu_gen1) ac_bus1.sourceOff = true;
                    else ac_bus2.sourceOff = true;
                }
            }

            // Set LEDs for Busses
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_TRANSFER_BUS_OFF_LIGHT, ac_bus1.isPowered);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_SOURCE_OFF_LIGHT, ac_bus1.sourceOff);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_TRANSFER_BUS_OFF_LIGHT, ac_bus2.isPowered);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_SOURCE_OFF_LIGHT, ac_bus2.sourceOff);

            //BAT DISCHARGE LIGHT
            if (fsi.MBI_ELEC_IND_BATTERY_SWITCH && !ac_bus1.isPowered && !ac_bus2.isPowered)
            {
                LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, true);
            } else
            {
                LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, false);
            }

            //APU OFF BUS LIGHT
            if (apu_gen1.isAvailable && apu_gen2.isAvailable && !apu_gen1.isOnline && !apu_gen2.isOnline)
            {
                LightController.set(FSIID.MBI_ELEC_BUS_APU_GEN_OFF_BUS_LIGHT, true);
            } 
            else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_APU_GEN_OFF_BUS_LIGHT, false);
            }

            //ENG 1 GEN BUS OFF LIGHT
            if (eng1_gen.isOnline)
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_GEN_OFF_BUS_LIGHT, false);
            } else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_GEN_OFF_BUS_LIGHT, true);
            }

            //ENG 2 GEN BUS OFF LIGHT
            if (eng2_gen.isOnline)
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_GEN_OFF_BUS_LIGHT, false);
            }
            else
            {
                LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_GEN_OFF_BUS_LIGHT, true);
            }

            //Check AC Bus 1 Power (heavy load systems e.g. displays)
            if (ac_bus1.isPowered)
            {
                //set SLI Voltage
                fsi.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE = 110;

                //displays on
                switchAC1Systems(true);
            } else
            {
                fsi.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE = 0;

                //displays off
                switchAC1Systems(false);
            }

            // Check AC Bus 2 Power (essentially same systems as AC Bus 1) 
            // ### TO-DO: Switch single systems depending on which AC Bus is powered
            if (ac_bus2.isPowered)
            {
                fsi.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE = 110;

                //displays on
                switchAC2Systems(true);
            }
            else
            {
                fsi.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE = 0;

                //displays off
                switchAC2Systems(false);
            }


            //DC Systems Power (Most important systems e.g. warning leds)
            if (fsi.MBI_ELEC_IND_BATTERY_SWITCH || //battery
                ac_bus1.isPowered || ac_bus2.isPowered) // AC Power
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


        //switch AC Systems on BUS 1 on / Off
        private static void switchAC1Systems(bool power)
        {
            //all displays
            fsi.INT_POWER_EICAS = power;
            fsi.INT_POWER_ISFD = power;
            fsi.INT_POWER_LDU = power;
            fsi.INT_POWER_ND_CPT = power;
            fsi.INT_POWER_ND_FO = power;
            fsi.INT_POWER_PFD_CPT = power;
            fsi.INT_POWER_PFD_FO = power;
            fsi.INT_POWER_SRMI = power;
        }
        
        //switch AC Systems on BUS 2 on / Off
        private static void switchAC2Systems(bool power)
        {
        }

        private static void switchDCSystems(bool power)
        {
            fsi.CPF_MCP_POWER = power;

            //all status lights
            if (power)
            {
                LightController.setLightPower(true);
            } else
            {
                LightController.setLightPower(false);
            }
        }
    }

    public class IDG
    {
        public IDG(AC_Powersource powersource)
        {
            assigned_powersource = powersource;
        }
        //          /Ready to Load /CSD connected 
        public bool isConnected = true;
        private AC_Powersource assigned_powersource;
        public void Disconnect()
        {
            isConnected = false;
            assigned_powersource.Unavailable();
        }        
    }

    public class AC_BUS
    {
        public AC_BUS(AC_Powersource assign_powersource)    // constructor for AC_BUS with default disconnected powersource "disconnect" (see init)
        {
            powersource = assign_powersource;
        }

        public bool isPowered = false, sourceOff = true;
        public AC_Powersource powersource;

        public void connect(AC_Powersource new_powersource)
        {           
            powersource = new_powersource;
            if (powersource.isOnline)
            {
                isPowered = true;                
            }
        }
        public void disconnect()
        {
            powersource = disconnected;
            isPowered = false;
        }
    }

    public class AC_Powersource
    {
        public bool isAvailable = false, isOnline = false;
        public bool SwitchOn()
        {
            if (isAvailable)
            {
                isOnline = true;
                return true;
            }
            else return false;
        }

        public void SwitchOff()
        {
            isOnline = false;
        }
        public void Available()
        {
            isAvailable = true;
        }
        
        public void Unavailable()
        {
            isAvailable = false;
            isOnline = false;
        }
    }
    
}
