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

                    FSIID.FSI_GEAR_ACTUAL_NOSE,
                    FSIID.FSI_GEAR_ACTUAL_RIGHT,
                    FSIID. FSI_GEAR_ACTUAL_LEFT
                }
            );

            //standard values
            LightController.set(FSIID.MBI_LOWER_T_LAVATORY_SMOKE_LIGHT, false);
            LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EQUIP_COOLING_EXHAUST_LIGHT, false);
            LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EQUIP_COOLING_SUPPLY_LIGHT, false);
            LightController.set(FSIID.MBI_LOWER_T_MIDDLE_ATTEND_CALL_LIGHT, false);
            LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_NOT_ARMED_LIGHT, false);

            LightController.set(FSIID.MBI_UPPER_T_LEFT_GEAR_LIGHT, false);
            LightController.set(FSIID.MBI_UPPER_T_NOSE_GEAR_LIGHT, false);
            LightController.set(FSIID.MBI_UPPER_T_RIGHT_GEAR_LIGHT, false);

            LightController.set(FSIID.MBI_UPPER_T_PSEU_LIGHT, false);

            fsi.ProcessWrites();
            LightController.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            //EMER EXIT LIGHTS
            if (id == FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_OFF_POS)
            {
                if (fsi.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_ON_POS)
                {
                    debug("EMER EXIT LIGHTS ON");
                    LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_NOT_ARMED_LIGHT, false);
                } else if (fsi.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_OFF_POS)
                {
                    debug("EMER EXIT LIGHTS OFF");
                    LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_NOT_ARMED_LIGHT, true);
                } else
                {
                    debug("EMER EXIT LIGHTS ARM");
                    LightController.set(FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_NOT_ARMED_LIGHT, false);
                }

                LightController.ProcessWrites();
            }

            //GEAR LIGHTS
            if (id == FSIID.FSI_GEAR_ACTUAL_NOSE)
            {
                if (fsi.FSI_GEAR_ACTUAL_NOSE >= 16383)
                {
                    LightController.set(FSIID.MBI_UPPER_T_NOSE_GEAR_LIGHT, true);
                } else
                {
                    LightController.set(FSIID.MBI_UPPER_T_NOSE_GEAR_LIGHT, false);
                }
                LightController.ProcessWrites();
            }
            if (id == FSIID.FSI_GEAR_ACTUAL_LEFT)
            {
                if (fsi.FSI_GEAR_ACTUAL_LEFT >= 16383)
                {
                    LightController.set(FSIID.MBI_UPPER_T_LEFT_GEAR_LIGHT, true);
                }
                else
                {
                    LightController.set(FSIID.MBI_UPPER_T_LEFT_GEAR_LIGHT, false);
                }
                LightController.ProcessWrites();
            }
            if (id == FSIID.FSI_GEAR_ACTUAL_RIGHT)
            {
                if (fsi.FSI_GEAR_ACTUAL_RIGHT >= 16383)
                {
                    LightController.set(FSIID.MBI_UPPER_T_RIGHT_GEAR_LIGHT, true);
                }
                else
                {
                    LightController.set(FSIID.MBI_UPPER_T_RIGHT_GEAR_LIGHT, false);
                }
                LightController.ProcessWrites();
            }
        }
    }
}
