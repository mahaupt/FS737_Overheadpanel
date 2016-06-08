using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSInterface;
using FSToolbox;

namespace Overheadpanel
{
    class PNEUMATICS : Panel
    { 

        public PNEUMATICS()
        {
            //debug variable
            is_debug = true;

            //starting FSI Client for IRS
            FSIcm.inst.OnVarReceiveEvent += fsiOnVarReceive;
            /*FSIcm.inst.DeclareAsWanted(new FSIID[]
                {
                    FSIID.MBI_ELT_ARM_SWITCH
                }
            );*/

            //standard values
            LightController.set(FSIID.MBI_PNEUMATICS_ALTN_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_AUTO_FAIL_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_DUAL_BLEED_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_LEFT_BLEED_TRIP_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_LEFT_PACK_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_LEFT_RAM_DOOR_FULL_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_LEFT_WING_BODY_OVERHEAT_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_MANUAL_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_OFF_SCHED_DESCENT_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_RIGHT_BLEED_TRIP_OFF_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_RIGHT_PACK_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_RIGHT_RAM_DOOR_FULL_OPEN_LIGHT, false);
            LightController.set(FSIID.MBI_PNEUMATICS_RIGHT_WING_BODY_OVERHEAT_LIGHT, false);

            FSIcm.inst.MBI_PNEUMATICS_LAMPTEST = false;

            FSIcm.inst.ProcessWrites();
        }


        void fsiOnVarReceive(FSIID id)
        {

        }
    }
}
