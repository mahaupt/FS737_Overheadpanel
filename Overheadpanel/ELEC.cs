using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

/*  ### FS737 ELEC 0.1 ###
    SIMPLIFIED SIMULATION OF B737-800 ELECTRICAL SYSTEM
    by Marcel Haupt and Arvid Preuss, 2016

    FOLLOWING CONDITIONS ARE SUPPOSED:
    * ALL GENERATORS RTL
    * BATTERY INFINITELY LOADED
    * NO FAILURES

    TO-DO (not in prioritized order):
    * Simulate failures upon receive of IOS fails
    * Create FSI variables for varios new busses and uncomment respective code
    * Create reset() method: all values are to be set to their initial state
    * Create failsafe() method: all values are to be set to "online" values regardless of environmental states and switches
    * Detailed comments and more / more precise debug messages
*/


namespace Overheadpanel
{
    class ELEC : Panel
    {
        private bool bustransfer_auto = true, sby_pwr_auto = true, sby_pwr_bat = false, battery_online = false;
        private static AC_Powersource eng1_gen = new AC_Powersource();
        private static AC_Powersource eng2_gen = new AC_Powersource();
        private AC_Powersource ext_pwr_l = new AC_Powersource();
        private AC_Powersource ext_pwr_r = new AC_Powersource();
        private AC_Powersource apu_gen1 = new AC_Powersource();
        private AC_Powersource apu_gen2 = new AC_Powersource();
        public static AC_Powersource disconnected = new AC_Powersource(); // unpowered dummy to have some value available for the AC_BUS class
        private IDG idg1 = new IDG(eng1_gen);
        private IDG idg2 = new IDG(eng2_gen);
        private AC_BUS ac_bus1 = new AC_BUS();
        private AC_BUS ac_bus2 = new AC_BUS();
        

        public ELEC()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            FSIcm.inst.DeclareAsWanted(new FSIID[]
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
            FSIcm.inst.SLI_APU_GEN_RTL = true;
            FSIcm.inst.IOS_GRD_PWR_CONNECTED = true;
            FSIcm.inst.SLI_GEN_1_RTL = true;
            FSIcm.inst.SLI_GEN_2_RTL = true;

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

            FSIcm.inst.MBI_ELEC_BUS_LAMPTEST = false;
            FSIcm.inst.MBI_ELEC_IND_LAMPTEST = false;
            FSIcm.inst.MBI_ELEC_STBY_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();

            simElectrics();
        }


        private void fsiOnVarReceive(FSIID id)
        {

            //GROUND POWER
            if (id == FSIID.IOS_GRD_PWR_CONNECTED)
            {
                if (FSIcm.inst.IOS_GRD_PWR_CONNECTED)
                {
                    debug("ELEC GND PWR Connected");
                    ext_pwr_l.Available();
                    ext_pwr_r.Available();
                } else
                {
                    debug("ELEC GND PWR Disconnected");
                    ext_pwr_l.Unavailable();
                    ext_pwr_r.Available();
                }

                //GND PWR available
                LightController.set(FSIID.MBI_ELEC_BUS_GRD_PWR_AVAILABLE_LIGHT, FSIcm.inst.IOS_GRD_PWR_CONNECTED);
                simElectrics();
            }

            //GROUND POWER SWITCH
            if (id == FSIID.MBI_ELEC_BUS_GRD_PWR_SWITCH)
            {
                if (FSIcm.inst.MBI_ELEC_BUS_GRD_PWR_SWITCH)
                {
                    debug("ELEC GND PWR SWITCH On");
                    if (ext_pwr_l.SwitchOn()) ac_bus1.select(ext_pwr_l);
                    if (ext_pwr_r.SwitchOn()) ac_bus2.select(ext_pwr_r);
                }
                else
                {
                    debug("ELEC GND PWR SWITCH Off");
                    ext_pwr_l.SwitchOff();
                    ext_pwr_r.SwitchOff();
                }
                simElectrics();
            }

            //battery
            if (id == FSIID.MBI_ELEC_IND_BATTERY_SWITCH)
            {
                if (FSIcm.inst.MBI_ELEC_IND_BATTERY_SWITCH)
                {
                    debug("ELEC DC Bat On");
                    battery_online = true;
                    simElectrics();
                }
                else
                {
                    debug("ELEC DC Bat Off");
                    battery_online = false;
                    simElectrics();
                }

                
            }

            //STBY Power
            if (id == FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS || id == FSIID.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS)
            {
                if (FSIcm.inst.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_AUTO_POS)
                {
                    debug("ELEC STBY PWR AUTO");
                    sby_pwr_auto = true;
                    sby_pwr_bat = false;
                } else if(FSIcm.inst.MBI_ELEC_STBY_STANDBY_POWER_SWITCH_BAT_POS)
                {
                    debug("ELEC STBY PWR BAT");
                    sby_pwr_auto = false;
                    sby_pwr_bat = true;
                } else
                {
                    debug("ELEC STBY PWR OFF");
                    sby_pwr_bat = false;
                    sby_pwr_auto = false;
                }
                simElectrics();
            }

            //IDG_1
            if (id == FSIID.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
            {
                if (!FSIcm.inst.MBI_ELEC_STBY_GEN_1_DISCONNECT_SWITCH)
                {
                    debug("ELEC STBY Gen 1 Connected");
                }
                else
                {
                    debug("ELEC STBY Gen 1 Disconnected");
                    idg1.Disconnect();
                }
                simElectrics();
            }

            //IDG_2
            if (id == FSIID.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
            {
                if (!FSIcm.inst.MBI_ELEC_STBY_GEN_2_DISCONNECT_SWITCH)
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
                if (FSIcm.inst.MBI_ELEC_BUS_APU_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 1 Off");
                    apu_gen1.SwitchOff();
                }
                if (FSIcm.inst.MBI_ELEC_BUS_APU_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 1 On");
                    if (apu_gen1.SwitchOn())
                    {
                        ac_bus1.select(apu_gen1);
                        ext_pwr_l.SwitchOff();
                        ext_pwr_r.SwitchOff();
                    }
                }
                simElectrics();
            }

            //APU GEN 2
            if (id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
            {
                if (FSIcm.inst.MBI_ELEC_BUS_APU_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC APU GEN 2 Off");
                    apu_gen2.SwitchOff();
                }
                if (FSIcm.inst.MBI_ELEC_BUS_APU_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC APU GEN 2 On");
                    if (apu_gen2.SwitchOn())
                    {
                        ac_bus2.select(apu_gen2);
                        ext_pwr_l.SwitchOff();
                        ext_pwr_r.SwitchOff();
                    }
                }
                    
                simElectrics();
            }

            //ENG 1 GEN
            if (id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
            {
                if (FSIcm.inst.MBI_ELEC_BUS_GEN_1_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 1 Off");
                    eng1_gen.SwitchOff();
                }
                if (FSIcm.inst.MBI_ELEC_BUS_GEN_1_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 1 On");
                    if(eng1_gen.SwitchOn()) ac_bus1.select(eng1_gen);
                }
                simElectrics();
            }

            //ENG 2 GEN
            if (id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS || id == FSIID.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
            {
                if (FSIcm.inst.MBI_ELEC_BUS_GEN_2_SWITCH_OFF_POS)
                {
                    debug("ELEC ENG GEN 2 Off");
                    eng2_gen.SwitchOff();
                }
                if (FSIcm.inst.MBI_ELEC_BUS_GEN_2_SWITCH_ON_POS)
                {
                    debug("ELEC ENG GEN 2 On");
                    if (eng2_gen.SwitchOn()) ac_bus2.select(eng2_gen);
                }
                simElectrics();
            }

            //BUS TRANSFER
            if (id == FSIID.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
            {
                if (!FSIcm.inst.MBI_ELEC_BUS_BUS_TRANSFER_SWITCH)
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
                if (FSIcm.inst.SLI_APU_GEN_RTL)
                {
                    debug("ELEC APU ready to load");
                    apu_gen1.Available();
                    apu_gen2.Available();
                }
                else
                {
                    debug("ELEC APU not ready to load");
                    apu_gen1.Unavailable();
                    apu_gen2.Unavailable();
                }
                simElectrics();
            }

            if (id == FSIID.SLI_GEN_1_RTL)
            {
                if (FSIcm.inst.SLI_GEN_1_RTL && idg1.isConnected) {
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
                if (FSIcm.inst.SLI_GEN_2_RTL && idg2.isConnected){
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
        private void simElectrics()
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
                }
                if(!ac_bus2.isPowered)
                {
                    ac_bus2.connect(ac_bus1.powersource);
                }
            }
            else
            {
                if (ac_bus1.powersource != ac_bus1.selected_source) ac_bus1.disconnect();
                if (ac_bus2.powersource != ac_bus2.selected_source) ac_bus2.disconnect();
            }

            // Set LEDs for Busses
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_TRANSFER_BUS_OFF_LIGHT, !ac_bus1.isPowered);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_1_SOURCE_OFF_LIGHT, ac_bus1.sourceOff);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_TRANSFER_BUS_OFF_LIGHT, !ac_bus2.isPowered);
            LightController.set(FSIID.MBI_ELEC_BUS_GEN_2_SOURCE_OFF_LIGHT, ac_bus2.sourceOff);

            // Set LEDs of IDGs
            LightController.set(FSIID.MBI_ELEC_STBY_GEN_1_DRIVE_LIGHT, !eng1_gen.isAvailable);
            LightController.set(FSIID.MBI_ELEC_STBY_GEN_2_DRIVE_LIGHT, !eng2_gen.isAvailable);

            //BAT DISCHARGE LIGHT
            if (FSIcm.inst.MBI_ELEC_IND_BATTERY_SWITCH && !ac_bus1.isPowered && !ac_bus2.isPowered)
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
                FSIcm.inst.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE = 110;

                //displays on
                switchAC1Systems(true);
            } else
            {
                FSIcm.inst.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE = 0;

                //displays off
                switchAC1Systems(false);
            }

            // Check AC Bus 2 Power (essentially same systems as AC Bus 1) 
            // ### TO-DO: Switch single systems depending on which AC Bus is powered
            if (ac_bus2.isPowered)
            {
                FSIcm.inst.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE = 110;

                //displays on
                switchAC2Systems(true);
            }
            else
            {
                FSIcm.inst.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE = 0;

                //displays off
                switchAC2Systems(false);
            }

            // DC BUSSES

            // DC BUS 1
            if (ac_bus1.isPowered)
            {
                //fsi.SLI_DC_BUS_1_VOLTAGE = 28
            }
            else; //fsi.SLI_DC_BUS_1_VOLTAGE = 0;

            // DC BUS 2
            if (ac_bus2.isPowered)
            {
                //fsi.SLI_DC_BUS_2_VOLTAGE = 28
            }
            else; //fsi.SLI_DC_BUS_2_VOLTAGE = 0;

            // DC SWITCHED HOT BATTERY BUS
            if (FSIcm.inst.MBI_ELEC_IND_BATTERY_SWITCH)
            {
                // fsi.SLI_DC_SWITCHED_HOT_BATTERY_BUS_VOLTAGE = 24;
            }
            else; // fsi.SLI_DC_SWITCHED_HOT_BATTERY_BUS_VOLTAGE = 0;

            // DC BATTERY BUS (it is supposed that battery capacity is infinite)
            if (battery_online || ac_bus1.isPowered || ac_bus2.isPowered)
            {
                debug("BATTERY BUS - some power available");
                if (ac_bus1.isPowered || ac_bus2.isPowered)
                {
                    FSIcm.inst.SLI_BAT_BUS_VOLTAGE = 28;
                    LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, false);
                }
                else if (battery_online)
                {
                    FSIcm.inst.SLI_BAT_BUS_VOLTAGE = 24;
                    LightController.set(FSIID.MBI_ELEC_IND_BAT_DISCHARGE_LIGHT, true);
                }              
                switchDCSystems(true);
            }
            else
            {
                debug("BATTERY BUS - no power available");
                FSIcm.inst.SLI_BAT_BUS_VOLTAGE = 0;
                switchDCSystems(false);
            }

            // STBY POWER  (it is supposed that battery capacity is infinite)
            if((sby_pwr_auto && (FSIcm.inst.MBI_ELEC_IND_BATTERY_SWITCH || ac_bus1.isPowered)) || (sby_pwr_bat && FSIcm.inst.MBI_ELEC_IND_BATTERY_SWITCH))
            {
                FSIcm.inst.SLI_AC_STBY_BUS_PHASE_1_VOLTAGE = 110; // AC STANDBY BUS
                if (ac_bus1.isPowered)  // STBY BUS RUNNING ON AC BUS 1
                {
                    FSIcm.inst.SLI_DC_STBY_BUS_SECT_1_VOLTAGE = 28; // DC STANDBY BUS                    
                }
                else // STBY BUS RUNNING ON BATTERY
                {
                    FSIcm.inst.SLI_DC_STBY_BUS_SECT_1_VOLTAGE = 24; // DC STANDBY BUS
                }
            }
            else
            {
                FSIcm.inst.SLI_AC_STBY_BUS_PHASE_1_VOLTAGE = 0; // AC STANDBY BUS
                FSIcm.inst.SLI_DC_STBY_BUS_SECT_1_VOLTAGE = 0; // DC STANDBY BUS
            }

            // STBY PWR OFF LIGHT
            if(FSIcm.inst.SLI_BAT_BUS_VOLTAGE == 0 || FSIcm.inst.SLI_DC_STBY_BUS_SECT_1_VOLTAGE == 0 || FSIcm.inst.SLI_AC_STBY_BUS_PHASE_1_VOLTAGE == 0)
            {
                LightController.set(FSIID.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT, true);
            }
            else LightController.set(FSIID.MBI_ELEC_STBY_STANDBY_PWR_OFF_LIGHT, false);

            FSIcm.inst.ProcessWrites();
        }


        //switch AC Systems on BUS 1 on / Off
        private void switchAC1Systems(bool power)
        {
            //all displays
            FSIcm.inst.INT_POWER_EICAS = power;
            FSIcm.inst.INT_POWER_ISFD = power;
            FSIcm.inst.INT_POWER_LDU = power;
            FSIcm.inst.INT_POWER_ND_CPT = power;
            FSIcm.inst.INT_POWER_ND_FO = power;
            FSIcm.inst.INT_POWER_PFD_CPT = power;
            FSIcm.inst.INT_POWER_PFD_FO = power;
            FSIcm.inst.INT_POWER_SRMI = power;
        }
        
        //switch AC Systems on BUS 2 on / Off
        private void switchAC2Systems(bool power)
        {
        }

        private void switchDCSystems(bool power)
        {
            FSIcm.inst.CPF_MCP_POWER = power;

            //all status lights
            if (power)
            {
                LightController.setLightPower(true);
                debug("DC Systems Power ON");
            } else
            {
                LightController.setLightPower(false);
                debug("DC Systems Power OFF");
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
        public bool isPowered = false, sourceOff = true;
        public AC_Powersource powersource = ELEC.disconnected, selected_source = ELEC.disconnected;

        public void connect(AC_Powersource new_powersource)
        {                      
            if (new_powersource.isAvailable)
            {                
                powersource = new_powersource;
                isPowered = powersource.SwitchOn();
                if (powersource != selected_source) sourceOff = true;
                else sourceOff = false;
            }
        }
        public void disconnect()
        {            
            powersource = ELEC.disconnected;
            sourceOff = false;
            isPowered = false;
        }

        public void select(AC_Powersource new_source)
        {
            if (new_source.isAvailable)
            {
                if (powersource == selected_source) selected_source.SwitchOff();
                selected_source = new_source;
                connect(new_source);
            }
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
