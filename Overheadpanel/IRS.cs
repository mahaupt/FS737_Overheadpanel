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
        private static Timer timer_l_align, timer_l_dc_on, timer_r_align, timer_r_dc_on;

        public IRS ()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            fsi = new FSIClient("Overhead IRS");
            fsi.OnVarReceiveEvent += fsiOnVarReceive;
            fsi.DeclareAsWanted(new FSIID[]
                {
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

            //send Settings to Server
            fsi.ProcessWrites();
            LightController.ProcessWrites();

            //create IRS Timer
            timer_l_align = new Timer(12 * 60, callbackIRS_L_Align);
            timer_r_align = new Timer(12 * 60, callbackIRS_R_Align);
            timer_l_dc_on = new Timer(5, callbackIRS_L_DC_ON);
            timer_r_dc_on = new Timer(5, callbackIRS_R_DC_ON);
            TimerManager.addTimer(timer_l_align);
            TimerManager.addTimer(timer_r_align);
            TimerManager.addTimer(timer_l_dc_on);
            TimerManager.addTimer(timer_r_dc_on);
        }

        static void fsiOnVarReceive(FSIID id)
        {

            //LEFT IRS KNOB
            if (id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_OFF_POS && fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_OFF_POS)
            {
                debug("IRS L OFF");

                //lights
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, false);
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, false);

                LightController.ProcessWrites();

                //stop timers
                timer_l_align.Reset();
                timer_l_dc_on.Reset();
            }
            if (id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ALIGN_POS && fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_ALIGN_POS)
            {
                debug("IRS L ALIGN");

                //lights
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, true);
                LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, true);
                LightController.ProcessWrites();

                //start aligning timers
                timer_l_align.Start();
                timer_l_dc_on.Start();
            }
            if (id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_NAV_POS && fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_NAV_POS)
            {
                debug("IRS L NAV");
            }
            if (id == FSIID.MBI_IRS_CONTROL_L_MODE_SWITCH_ATT_POS && fsi.MBI_IRS_CONTROL_L_MODE_SWITCH_ATT_POS)
            {
                debug("IRS L ATT");
            }



            //RIGHT IRS KNOB
            if (id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_OFF_POS && fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_OFF_POS)
            {
                debug("IRS R OFF");

                //lights
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, false);
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, false);

                LightController.ProcessWrites();

                //stop timers
                timer_r_align.Reset();
                timer_r_dc_on.Reset();
            }
            if (id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ALIGN_POS && fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_ALIGN_POS)
            {
                debug("IRS R ALIGN");

                //lights
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, true);
                LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, true);
                LightController.ProcessWrites();

                //start aligning timers
                timer_r_align.Start();
                timer_r_dc_on.Start();
            }
            if (id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_NAV_POS && fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_NAV_POS)
            {
                debug("IRS R NAV");
            }
            if (id == FSIID.MBI_IRS_CONTROL_R_MODE_SWITCH_ATT_POS && fsi.MBI_IRS_CONTROL_R_MODE_SWITCH_ATT_POS)
            {
                debug("IRS R ATT");
            }

           
        }


        private static void callbackIRS_L_Align()
        {
            //finished aligning - light off
            debug("IRS L Align finished");
            LightController.set(FSIID.MBI_IRS_CONTROL_L_ALIGN_LIGHT, false);
            LightController.ProcessWrites();
        }

        private static void callbackIRS_R_Align()
        {
            debug("IRS R Align finished");
            LightController.set(FSIID.MBI_IRS_CONTROL_R_ALIGN_LIGHT, false);
            LightController.ProcessWrites();
        }

        private static void callbackIRS_L_DC_ON()
        {
            //switching to AC, DC_ON_LIGHT off
            debug("IRS L DC Off");
            LightController.set(FSIID.MBI_IRS_CONTROL_L_ON_DC_LIGHT, false);
            LightController.ProcessWrites();
        }

        private static void callbackIRS_R_DC_ON()
        {
            debug("IRS R DC Off");
            LightController.set(FSIID.MBI_IRS_CONTROL_R_ON_DC_LIGHT, false);
            LightController.ProcessWrites();
        }
    }
}
