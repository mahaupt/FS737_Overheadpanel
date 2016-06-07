using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class DOORS : Panel
    {
        public DOORS()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            /*fsi.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_OFF_POS,
                    FSIID.MBI_LOWER_T_MIDDLE_EMER_EXIT_LIGHTS_SWITCH_ON_POS,
                }
            );*/

            //standard values
            LightController.set(FSIID.MBI_DOOR_AFT_CARGO_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_AFT_ENTRY_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_AFT_SERVICE_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_AIRSTAIR_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_EQUIP_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_FWD_CARGO_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_FWD_ENTRY_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_FWD_SERVICE_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_LEFT_AFT_OVERWING_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_LEFT_FWD_OVERWING_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_RIGHT_AFT_OVERWING_LIGHT, false);
            LightController.set(FSIID.MBI_DOOR_RIGHT_FWD_OVERWING_LIGHT, false);

            FSIcm.inst.MBI_DOOR_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        static void fsiOnVarReceive(FSIID id)
        {
            
        }
    }
}
