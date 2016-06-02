using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class IRS : Panel
    {
        private static FSIClient fsi;
        private static Irs_mod irs_l, irs_r;

        public IRS ()
        {
            //debug variable
            is_debug = true;

            irs_l = new Irs_mod();
            irs_r = new Irs_mod();

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead IRS");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.SLI_BAT_BUS_VOLTAGE,
                    FSIID.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE,
                    FSIID.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE,

                    FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_OFF_POS,
                    FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_NAV_POS,
                    FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ATT_POS,
                    FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ALIGN_POS,
                    FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_OFF_POS,
                    FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_NAV_POS,
                    FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ATT_POS,
                    FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ALIGN_POS
                }
            );

            //set Lights on the beginning
            LightController.set(FSIID.MBI_IRS_CONTROL_L_DC_FAIL_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_L_FAULT_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_R_DC_FAIL_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_R_FAULT_LIGHT, false);
            LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, false);

            fsi.MBI_IRS_CONTROL_LAMPTEST = false;

            //send Settings to Server
            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }

        static void fsiOnVarReceive(FSIID id)
        {

            //LEFT IRS KNOB or Power
            if (id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_OFF_POS ||
                id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ALIGN_POS || 
                id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_NAV_POS || 
                id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ATT_POS || 
                id == FSIID.SLI_BAT_BUS_VOLTAGE)
            {
                //switch to off if
                if (fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_OFF_POS || fsi.SLI_BAT_BUS_VOLTAGE <= 12)
                {
                    debug("IRS L OFF");

                    irs_l.setPowerStatus(false);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_ALIGN_POS)
                {
                    debug("IRS L ALIGN");

                    irs_l.setPowerStatus(true);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_NAV_POS)
                {
                    debug("IRS L NAV");

                    irs_l.setPowerStatus(true);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_ATT_POS)
                {
                    debug("IRS L ATT");

                    irs_l.setPowerStatus(true);
                    sim_irs();
                }
            }


            //RIGHT IRS KNOB
            if (id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_OFF_POS ||
                id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ALIGN_POS || 
                id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_NAV_POS || 
                id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ATT_POS || 
                id == FSIID.SLI_BAT_BUS_VOLTAGE)
            {
                
                if (fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_OFF_POS || fsi.SLI_BAT_BUS_VOLTAGE <= 12)
                {
                    debug("IRS R OFF");

                    irs_r.setPowerStatus(false);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_ALIGN_POS)
                {
                    debug("IRS R ALIGN");

                    irs_r.setPowerStatus(true);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_NAV_POS)
                {
                    debug("IRS R NAV");

                    irs_r.setPowerStatus(true);
                    sim_irs();
                }
                else if (fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_ATT_POS)
                {
                    debug("IRS R ATT");

                    irs_r.setPowerStatus(true);
                    sim_irs();
                }
            }
           

            //AC Powersources
            if (id == FSIID.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE)
            {
                //no voltage on XFR BUS 2
                if (fsi.SLI_AC_XFR_BUS_2_PHASE_1_VOLTAGE <= 50)
                {
                    irs_r.setACAvailable(false);
                } else
                {
                    irs_r.setACAvailable(true);
                }
                sim_irs();
            }

            //läuft eigentlich über stby bus
            if (id == FSIID.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE)
            {
                if (fsi.SLI_AC_XFR_BUS_1_PHASE_1_VOLTAGE <= 50)
                {
                    irs_l.setACAvailable(false);
                } else
                {
                    irs_l.setACAvailable(true);
                }
                sim_irs();
            }
        }

        public static void sim_irs()
        {
            //IRS L ALIGN LIGHT
            if (irs_l.isOnline && irs_l.isAligning())
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, true);
            } else
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, false);
            }

            //IRS R ALIGN LIGHT
            if (irs_r.isOnline && irs_r.isAligning())
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, true);
            }
            else
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, false);
            }

            //IRS L ON DC
            if (irs_l.isOnline && irs_l.onDC)
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, true);
            }
            else
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, false);
            }

            //IRS R ON DC
            if (irs_r.isOnline && irs_r.onDC)
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, true);
            }
            else
            {
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, false);
            }

            LightController.ProcessWrites();
        }
    }


    //IRS Module
    class Irs_mod
    {
        public bool isOnline = false;
        public bool isAligned = false;
        public bool acAvailable = true;
        public bool onDC = false;

        private Timer alignTimer;
        private Timer dcOffTimer;

        public Irs_mod()
        {
            alignTimer = new Timer(60*3, alignedCallback);
            dcOffTimer = new Timer(3, dcOffCallback);
            TimerManager.addTimer(alignTimer);
            TimerManager.addTimer(dcOffTimer);
        }

        public void setPowerStatus(bool value)
        {
            if (isOnline != value)
            {
                //set to online - start alignment
                if (value)
                {
                    alignTimer.Start();

                    if (acAvailable)
                    {
                        dcOffTimer.Start();
                    }

                    isOnline = true;
                    onDC = true;
                }
                else //set offline
                {
                    alignTimer.Reset();
                    dcOffTimer.Reset();

                    isAligned = false;
                    isOnline = false;
                }
            }
        }

        public void setACAvailable(bool value)
        {
            if (acAvailable != value)
            {
                if (value)
                {
                    if (isOnline)
                    {
                        dcOffTimer.Start();
                    }
                    acAvailable = true;
                }
                else
                {
                    if (isOnline)
                    {
                        dcOffTimer.Reset();
                    }
                    onDC = true;
                    acAvailable = false;
                }
            }
        }

        private void alignedCallback()
        {
            isAligned = true;
            IRS.sim_irs();
        }

        private void dcOffCallback()
        {
            onDC = false;
            IRS.sim_irs();
        }

        public bool isAligning()
        {
            return alignTimer.isEnabled();
        }
    }
}
